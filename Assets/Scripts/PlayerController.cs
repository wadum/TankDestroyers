﻿using System.Collections;
using Assets.Scripts.UI;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
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
        public GameObject FirstPerson;
        public GameObject ThirdPerson;
        public bool FirstPersonMode = false;

        private Rigidbody _rb;
        private float _health;
        private int _mineAmount;
        private bool _disableControls = false;

        public IWeaponController Weapon;
        private RadialSlider _healthBar;
        private RadialSlider _reloadBar;
        private UiMineController _mineBar;
        private GameObject _firstPersonUI;

        void Start()
        {
            HealthIndicator.SetHealth(100);
            _firstPersonUI = (GameObject)Instantiate(Resources.Load("UI"));
            _healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<RadialSlider>();
            _reloadBar = GameObject.FindGameObjectWithTag("ReloadBar").GetComponent<RadialSlider>();
            _rb = gameObject.GetComponent<Rigidbody>();
            _health = Health;
            _mineAmount = MineStartAmount;
            _mineBar = GameObject.FindGameObjectWithTag("MinesBar").GetComponent<UiMineController>();
            _mineBar.SetAvailableMines(_mineAmount);
            UpdateCameraMode();
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
            if (Input.GetKeyDown("f"))
            {
                FirstPersonMode = !FirstPersonMode;
                UpdateCameraMode();
            }
            if (Input.GetKeyDown("space"))
                Weapon.FireWeapon(transform.position, transform.forward);
            if (Input.GetKeyDown("left ctrl"))
            {
                if (_mineAmount < 1)
                    return;
                var mine = ((GameObject)Instantiate(MinePrefab)).GetComponent<MineController>();
                mine.Place(transform.position, transform.forward);
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

        public void ChangeWeapon(IWeaponController newWeapon)
        {
            Weapon = newWeapon;
        }

        private IEnumerator TakeDamage(float amount)
        {
            _health -= amount;
            UpdateHealthIndicators();
            if (_health >= 1) yield break;
            _disableControls = true;
            yield return new WaitForSeconds(3);
            Application.LoadLevel(Application.loadedLevel);
        }

        private void UpdateHealthIndicators()
        {
            _healthBar.SetValue(_health / Health * 100);
            HealthIndicator.SetHealth(_health / Health * 100);
        }

        private void UpdateCameraMode()
        {
            Camera.main.transform.localPosition = FirstPersonMode
                    ? FirstPerson.transform.localPosition
                    : ThirdPerson.transform.localPosition;
            foreach (var cr in  _firstPersonUI.GetComponentsInChildren<CanvasRenderer>())
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
