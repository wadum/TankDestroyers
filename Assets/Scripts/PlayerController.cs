using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        public int Health = 3;

        [Header("Controls")]
        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        [Space(10)]
        [Header("Mine Settings")]
        public GameObject MinePrefab;
        public int MineStartAmount = 3;


        private Rigidbody _rb;
        private AudioSource _shotSource;
        private int _health;
        private int _mineAmount;
        private bool _disableControls = false;

        public IWeaponController Weapon;

        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody>();
            _shotSource = gameObject.AddComponent<AudioSource>();
            _shotSource.clip = TankShotSound;
            _shotSource.loop = false;
            _shotSource.playOnAwake = false;
            _shotSource.volume = TankShotVolume;
            _shotSource.minDistance = 5;
            _shotSource.maxDistance = 20;
            _shotSource.rolloffMode = AudioRolloffMode.Linear;
            _shotSource.spatialBlend = 1;
            _health = Health;
            _mineAmount = MineStartAmount;
            if (Weapon == null)
                Weapon = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletController>();
        }

        // Update is called once per frame
        void Update ()
        {
            if (_disableControls) return;
            if (Input.GetKey("a"))
                _rb.AddTorque(new Vector3(0, -1*RotationMagnitude,0));
            if (Input.GetKey("d"))
                _rb.AddTorque(new Vector3(0, 1*RotationMagnitude, 0));
            if (Input.GetKey("w"))
                _rb.AddForce(transform.forward * MovementMagnitude);
            if (Input.GetKey("s"))
                _rb.AddForce(transform.forward * -MovementMagnitude);
            if (Input.GetKeyDown("space"))
            {
                Weapon.FireWeapon(transform.position, transform.forward);
                _shotSource.Play();
            }
            if (Input.GetKeyDown("left ctrl"))
            {
                if (_mineAmount < 1)
                    return;
                var mine = (GameObject)Instantiate(MinePrefab);
                mine.transform.position = transform.position;
                mine.transform.Translate(transform.forward*-3.5f);
                _mineAmount--;
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
        }

        public void ChangeWeapon(IWeaponController newWeapon)
        {
            Weapon = newWeapon;
        }

        private IEnumerator TakeDamage(int amount)
        {
            _health -= amount;
            if (_health >= 1) yield break;
            _disableControls = true;
            yield return new WaitForSeconds(3);
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
