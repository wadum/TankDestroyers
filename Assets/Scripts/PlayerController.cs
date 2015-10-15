﻿using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        [Header("Controls")]
        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        private Rigidbody _rb;
        private AudioSource _shotSource;

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
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetKey("a"))
                _rb.AddTorque(new Vector3(0,1*RotationMagnitude,0));
            if (Input.GetKey("d"))
                _rb.AddTorque(new Vector3(0, -1 * RotationMagnitude, 0));
            if (Input.GetKey("w"))
                _rb.AddForce(transform.forward * MovementMagnitude);
            if (Input.GetKey("s"))
                _rb.AddForce(transform.forward * -MovementMagnitude);
            if (Input.GetKeyDown("space"))
            {
                GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletController>().FireBullet(transform.position, transform.forward);
                _shotSource.Play();
            }
        }

        public void HitByBullet()
        {
            //TODO Not Implemented
        }
    }
}
