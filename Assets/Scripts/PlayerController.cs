using System.Collections;
using System.Linq;
using Assets.Scripts.UI;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class PlayerController : NetworkBehaviour
    {

        public float Health = 100;
        public HealthIndicator HealthIndicator;

        [Header("Controls")]
        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;
        
        [Space(10)]
        [Header("Mine Settings")]
        public GameObject MinePrefab;
        public int MineStartAmount = 3;
        public float MineDamage = 50;
        public GameObject[] InactiveMines;

        [Space(10)]
        [Header("Camera")]
        public GameObject MyCamera;
        public GameObject FirstPerson;
        public GameObject ThirdPerson;
        public bool FirstPersonMode = false;

        [Space(10)]
        [Header("Weapons")]
        public float MachineGunCd = 0.1f;
        public AudioSource MachineGunSound;

        private Rigidbody _rb;
        [SyncVar]
        private float _health;
        private int _mineAmount;
        public bool DisableControls = true;

        private BulletManager _weapon;
        private MissileManager _missiles;
        private RadialSlider _healthBar;
        private RadialSlider _reloadBar;
        private UiMineController _mineBar;
        private GameObject _firstPersonUi;

        void Start()
        {
            if (isLocalPlayer)
            {
                DisableControls = false;
                gameObject.GetComponent<AudioSource>().enabled = true;
                MyCamera.SetActive(true);
            }


            HealthIndicator.SetHealth(100);
            _firstPersonUi = (GameObject)Instantiate(Resources.Load("UI"));
            _healthBar =
                _firstPersonUi.transform.GetComponentsInChildren<RadialSlider>().First(s => s.tag == "HealthBar");
            //_healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<RadialSlider>();
            //_reloadBar = GameObject.FindGameObjectWithTag("ReloadBar").GetComponent<RadialSlider>();
            _reloadBar =
                _firstPersonUi.transform.GetComponentsInChildren<RadialSlider>().First(s => s.tag == "ReloadBar");
            _rb = gameObject.GetComponent<Rigidbody>();
            _health = Health;
            _mineAmount = MineStartAmount;
            _mineBar = _firstPersonUi.transform.GetComponentsInChildren<UiMineController>().First(s => s.tag == "MinesBar");
            //_mineBar = GameObject.FindGameObjectWithTag("MinesBar").GetComponent<UiMineController>();
            _mineBar.SetAvailableMines(_mineAmount);
            UpdateCameraMode();
            if (_weapon == null)
                _weapon = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletManager>();
            if (_missiles == null)
                _missiles = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<MissileManager>();
        }

        void FixedUpdate()
        {
            if (DisableControls) return;
            if (Input.GetKey("a"))
                _rb.AddTorque(new Vector3(0, -1 * RotationMagnitude, 0));
            if (Input.GetKey("d"))
                _rb.AddTorque(new Vector3(0, 1 * RotationMagnitude, 0));
            if (Input.GetKey("w"))
                _rb.AddForce(transform.forward * MovementMagnitude);
            if (Input.GetKey("s"))
                _rb.AddForce(transform.forward * -MovementMagnitude);
        }

        // Update is called once per frame
        [ClientCallback]
        void Update ()
        {
            UpdateHealthIndicators();
            if (DisableControls) return;
            if (Input.GetKeyDown("f"))
            {
                FirstPersonMode = !FirstPersonMode;
                UpdateCameraMode();
            }
            if (Input.GetKeyDown("e"))
            {
                _missiles.CmdFireMissile(transform.position, transform.forward);
            }
            if (Input.GetKeyDown("space"))
            {
                StartCoroutine(FireMachineGun());
            }
            if (Input.GetKeyDown("left ctrl"))
            {
                if (_mineAmount < 1)
                    return;

                Cmd_PlaceMine();
                _mineAmount--;
                SetAvailableMines(_mineAmount);
            }
        }

        [Command]
        public void Cmd_PlaceMine()
        {
            var mine = ((GameObject)Instantiate(MinePrefab)).GetComponent<MineController>();
            mine.transform.position = transform.position;
            mine.transform.Translate(transform.forward * -3.5f);
            mine.transform.position = new Vector3(mine.transform.position.x, 0.2f, mine.transform.position.z);
            NetworkServer.Spawn(mine.gameObject);
        }

        public void HitByBullet()
        {
            StartCoroutine(TakeDamage(10));
        }

        public void HitByMine()
        {
            StartCoroutine(TakeDamage(MineDamage));
        }

        public void AddMines(int amount)
        {
            _mineAmount += amount;
            SetAvailableMines(_mineAmount);
        }

        public void AddHealth(int amount)
        {
            _health = Mathf.Min(_health + amount, Health);
            UpdateHealthIndicators();
        }

        private IEnumerator FireMachineGun()
        {
            MachineGunSound.Play();
            while (Input.GetKey("space"))
            {
                _weapon.CmdFireWeapon(transform.position, transform.forward);
                yield return new WaitForSeconds(MachineGunCd);
            }
            MachineGunSound.Stop();
        }

        private IEnumerator TakeDamage(float amount)
        {
            _health -= amount;
            UpdateHealthIndicators();
            if (_health >= 1) yield break;
            DisableControls = true;
            yield return new WaitForSeconds(3);
            Application.LoadLevel(Application.loadedLevel);
        }

        private void UpdateHealthIndicators()
        {
            if(isLocalPlayer)
                _healthBar.SetValue(_health / Health * 100);
            HealthIndicator.SetHealth(_health / Health * 100);
        }

        private void UpdateCameraMode()
        {
            MyCamera.transform.localPosition = FirstPersonMode
                    ? FirstPerson.transform.localPosition
                    : ThirdPerson.transform.localPosition;
            foreach (var cr in  _firstPersonUi.GetComponentsInChildren<CanvasRenderer>())
            {
                cr.SetAlpha(FirstPersonMode ? 1 : 0);
            }
        }

        private void SetAvailableMines(int amount)
        {
            _mineBar.SetAvailableMines(_mineAmount);
            for (var i = 0; i < InactiveMines.Length; i++)
            {
                InactiveMines[i].SetActive(i < amount);
            }
        }



    }
}
