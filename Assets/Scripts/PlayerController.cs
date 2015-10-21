using System.Collections;
using System.Linq;
using Assets.Scripts.PickUps;
using Assets.Scripts.UI;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class PlayerController : NetworkBehaviour
    {

        public float Health = 100;
        public HealthIndicator HealthIndicator;

        [SyncVar]
        public string PlayerName;

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

        private short networkId;

        private BulletManager _weapon;
        private MissileManager _missiles;
        private RadialSlider _healthBar;
        private RadialSlider _reloadBar;
        private UiMineController _mineBar;
        private UiMissileController _missileBar;
        private GameObject _firstPersonUi;
        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;
        private RoundKeeper _roundKeeper;

        void Start()
        {
            if (isLocalPlayer)
            {
                DisableControls = false;
                gameObject.GetComponent<AudioSource>().enabled = true;
                MyCamera.SetActive(true);
            }

            _spawnRotation = transform.rotation;
            _spawnPosition = transform.position;
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
            _missileBar = _firstPersonUi.transform.GetComponentsInChildren<UiMissileController>().First(s => s.tag == "MissilesBar");
            _missileAmount = MissileStartAmount;
            //_mineBar = GameObject.FindGameObjectWithTag("MinesBar").GetComponent<UiMineController>();
            _mineBar.SetAvailableMines(_mineAmount);
            UpdateCameraMode();
            if (_weapon == null)
                _weapon = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletManager>();
            if (_missiles == null)
                _missiles = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<MissileManager>();


            networkId = (short)GetComponent<NetworkIdentity>().netId.Value;
            _roundKeeper = GameObject.FindObjectOfType<RoundKeeper>();
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
        public void CmdFireMissile(Vector3 origin, Vector3 direction, short owner)
        {
            _missiles.RpcFireMissile(origin, direction, owner);
        }

        [Command]
        public void CmdFireBullet(Vector3 origin, Vector3 direction, short owner)
        {
            _weapon.RpcFireWeapon(origin, direction, owner);
        }

        // Update is called once per frame
        [ClientCallback]
        void Update ()
        {
            if (_health < 1)
                Respawn();
            SetAvailableMines();
            SetAvailableMissiles();
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
                CmdFireMissile(transform.position, transform.forward, networkId);
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

                Cmd_PlaceMine(networkId);
                CmdAddMines(-1);
            }
        }

        [Command]
        public void Cmd_PlaceMine(short networkId)
        {
            var mine = ((GameObject)Instantiate(MinePrefab)).GetComponent<MineController>();
            mine.transform.position = transform.position;
            mine.transform.Translate(transform.forward * -3.5f);
            mine.transform.position = new Vector3(mine.transform.position.x, 0.2f, mine.transform.position.z);
            mine.Owner = networkId;
            NetworkServer.Spawn(mine.gameObject);
        }

        public bool HitByBullet(int damage, short owner)
        {
            if (isLocalPlayer)
                CmdTakeDamage(10, owner);
            return isLocalPlayer;
        }

        public bool HitByMine(GameObject mine, short owner)
        {
            if (isLocalPlayer)
            {
                CmdTakeDamage(MineDamage, owner);
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
                CmdFireBullet(transform.position, transform.forward, networkId);
                yield return new WaitForSeconds(MachineGunCd);
            }
            MachineGunSound.Stop();
        }

        [Command]
        private void CmdTakeDamage(float amount, short owner)
        {
            _health -= amount;

            if (_health < 1)
            {
                _roundKeeper.AddScoreTo(owner);
                Respawn();
            }
        }

        public void Respawn()
        {
            transform.position = _spawnPosition;
            transform.rotation = _spawnRotation;
            _mineAmount = MineStartAmount;
            _missileAmount = MissileStartAmount;
            _health = Health;
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
            foreach (var cr in _firstPersonUi.GetComponentsInChildren<CanvasRenderer>().Where(s => s.tag != Constants.Tags.TimerBar))
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

        private void SetAvailableMissiles()
        {
            if (isLocalPlayer && FirstPersonMode)
                _missileBar.SetAvailableMissiles(_missileAmount);
            for (var i = 0; i < InactiveMissiles.Length; i++)
            {
                InactiveMissiles[i].SetActive(i < _missileAmount);
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
