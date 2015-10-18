using System.Collections;
using Assets.Scripts.UI;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        public float Health = 3;

        [Header("Controls")]
        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;

        [Space(10)]
        [Header("Mine Settings")]
        public GameObject MinePrefab;
        public int MineStartAmount = 3;
        public GameObject[] InactiveMines;


        private Rigidbody _rb;
        private AudioSource _shotSource;
        private float _health;
        private int _mineAmount;
        private bool _disableControls = false;

        public IWeaponController Weapon;
        private RadialSlider _healthBar;
        private RadialSlider _reloadBar;
        private UiMineController _mineBar;

        void Start()
        {
            Instantiate(Resources.Load("UI"));
            _healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<RadialSlider>();
            _reloadBar = GameObject.FindGameObjectWithTag("ReloadBar").GetComponent<RadialSlider>();
            _rb = gameObject.GetComponent<Rigidbody>();
            _health = Health;
            _mineAmount = MineStartAmount;
            _mineBar = GameObject.FindGameObjectWithTag("MinesBar").GetComponent<UiMineController>();
            _mineBar.SetAvailableMines(_mineAmount);
            if (Weapon == null)
                Weapon = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<MissileManager>();
        }

        void FixedUpdate()
        {
            if (_disableControls) return;
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
        void Update ()
        {
            if (_disableControls) return;
            if (Input.GetKeyDown("space"))
                Weapon.FireWeapon(transform.position, transform.forward);
            if (Input.GetKeyDown("left ctrl"))
            {
                if (_mineAmount < 1)
                    return;
                var mine = (GameObject)Instantiate(MinePrefab);
                mine.transform.position = transform.position;
                mine.transform.Translate(transform.forward*-3.5f);
                mine.transform.position = new Vector3(mine.transform.position.x, 0.05f, mine.transform.position.z);
                _mineAmount--;
                SetAvailableMines(_mineAmount);
            }
        }

        public void HitByBullet()
        {
            StartCoroutine(TakeDamage(1));
        }

        public void HitByMine()
        {
            StartCoroutine(TakeDamage(1));
        }

        public void AddMines(int amount)
        {
            _mineAmount += amount;
            SetAvailableMines(_mineAmount);
        }

        public void ChangeWeapon(IWeaponController newWeapon)
        {
            Weapon = newWeapon;
        }

        private IEnumerator TakeDamage(float amount)
        {
            _health -= amount;
            _healthBar.SetValue(_health/Health * 100);
            if (_health >= 1) yield break;
            _disableControls = true;
            yield return new WaitForSeconds(3);
            Application.LoadLevel(Application.loadedLevel);
        }

        private void SetAvailableMines(int amount)
        {
            _mineBar.SetAvailableMines(_mineAmount);
            amount = Mathf.Min(amount, InactiveMines.Length);
            for (var i = 0; i < InactiveMines.Length; i++)
            {
                InactiveMines[i].SetActive(i < amount);
            }
        }
    }
}
