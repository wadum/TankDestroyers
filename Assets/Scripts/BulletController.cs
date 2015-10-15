﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts
{
    public class BulletController : MonoBehaviour
    {
        public float BulletSpeed = 0.1f;    
        
        private List<GameObject> _bullets;
        private GameObject _bulletPrefab;


        // Use this for initialization
        void Start () {

            _bulletPrefab = (GameObject)Resources.Load("Bullet");
            _bullets = new List<GameObject>();
            for (var i = 0; i < 10; i++)
            {
                var b = Instantiate(_bulletPrefab);
                b.SetActive(false);
                _bullets.Add(b);
            }
        }

        private GameObject GetBullet()
        {
            var b = _bullets.FirstOrDefault(s => !s.activeSelf);
            if (!b)
            {
                b = Instantiate(_bulletPrefab);
                _bullets.Add(b);
            }
            b.SetActive(true);
            return b;
        }

        public void FireBullet(Vector3 origin, Vector3 direction)
        {
            var bullet = GetBullet();
            bullet.transform.position = origin;
           // bullet.transform.Rotate(Quaternion.FromToRotation(bullet.transform.forward, direction).eulerAngles);
            bullet.transform.Translate(direction * 2);
            
            StartCoroutine(MoveBullet(bullet, direction));
        }

        private IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
        {
            while (bullet.activeSelf)
            {
                bullet.transform.Translate(direction * BulletSpeed);
                yield return null;
            }
        }
    }
}
