using System.Collections;
using System.Linq;
using Assets.Scripts.PickUps;
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
        [Header("Missile Settings")]
        public int MissileStartAmount = 3;
        public float MissileDamage = 50;
        public GameObject[] InactiveMissiles;

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
        [SyncVar]
        private int _mineAmount;
        [SyncVar]
        private int _missileAmount;
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
            _missileAmount = MissileStartAmount;
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
            if (DisableControls && !isLocalPlayer) return;
            if (Input.GetKey("a"))
                _rb.AddTorque(new Vector3(0, -1 * RotationMagnitude, 0));
            if (Input.GetKey("d"))
                _rb.AddTorque(new Vector3(0, 1 * RotationMagnitude, 0));
            if (Input.GetKey("w"))
                _rb.AddForce(transform.forward * MovementMagnitude);
            if (Input.GetKey("s"))
                _rb.AddForce(transform.forward * -MovementMagnitude);
        }

        [Command]
        public void CmdFireMissile(Vector3 origin, Vector3 direction)
        {
            _missiles.RpcFireMissile(origin, direction);
        }

        [Command]
        public void CmdFireBullet(Vector3 origin, Vector3 direction)
        {
            _weapon.RpcFireWeapon(origin, direction);
        }

        // Update is called once per frame
        [ClientCallback]
        void Update ()
        {
            SetAvailableMines();
            UpdateHealthIndicators();
            if (DisableControls && !isLocalPlayer) return;
            if (Input.GetKeyDown("f"))
            {
                FirstPersonMode = !FirstPersonMode;
                UpdateCameraMode();
            }
            if (Input.GetKeyDown("e"))
            {
                if (_missileAmount < 1)
                    return;
                CmdFireMissile(transform.position, transform.forward);
                CmdAddMissiles(-1);
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
                CmdAddMines(-1);
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

        public bool HitByBullet()
        {
            if (isLocalPlayer)
                CmdTakeDamage(10);
            return isLocalPlayer;
        }

        public bool HitByMine(GameObject mine)
        {
            if (isLocalPlayer)
            {
                CmdTakeDamage(MineDamage);
                CmdExplodeMine(mine);
            }
            return isLocalPlayer;
        }

        [Command]
        public void CmdExplodeMine(GameObject mine)
        {
            mine.GetComponent<MineController>().RpcExplode();
        }

        [Command]
        public void CmdAddMissiles(int amount)
        {
            _missileAmount += amount;
        }

        [Command]
        public void CmdAddMines(int amount)
        {
            _mineAmount += amount;
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
                CmdFireBullet(transform.position, transform.forward);
                yield return new WaitForSeconds(MachineGunCd);
            }
            MachineGunSound.Stop();
        }

        [Command]
        private void CmdTakeDamage(float amount)
        {
            _health -= amount;
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

        private void SetAvailableMines()
        {
            if (isLocalPlayer && FirstPersonMode)
                _mineBar.SetAvailableMines(_mineAmount);
            for (var i = 0; i < InactiveMines.Length; i++)
            {
                InactiveMines[i].SetActive(i < _mineAmount);
            }
        }

        public bool ResetPickUp(GameObject pickUp)
        {
            if (!isLocalPlayer) return false;
            CmdResetPickUp(pickUp);
            return true;
        }

        [Command]
        public void CmdResetPickUp(GameObject pickUp)
        {
            RpcResetPickUp(pickUp);
        }

        [ClientRpc]
        public void RpcResetPickUp(GameObject pickUp)
        {
            StartCoroutine(pickUp.GetComponent<IPickUpController>().PickUp());
        }

    }
}
