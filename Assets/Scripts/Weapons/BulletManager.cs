﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;


namespace Assets.Scripts.Weapons
{
    public class BulletManager : NetworkBehaviour
    {
        public float BulletSpeed = 0.5f;    
        
        private List<GameObject> _bullets;
        private GameObject _bulletPrefab;


        // Use this for initialization
        void Start () {

            _bulletPrefab = (GameObject)Resources.Load("Weapons/Bullet");
            _bullets = new List<GameObject>();
            for (var i = 0; i < 10; i++)
            {
                var b = Instantiate(_bulletPrefab);
                b.transform.parent = transform;
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
                b.transform.parent = transform;
                _bullets.Add(b);
            }
            b.SetActive(true);
            return b;
        }

        [ClientRpc]
        public void RpcFireWeapon(Vector3 origin, Vector3 direction, short owner)
        {
            var bullet = GetBullet();
            bullet.transform.position = origin;

            bullet.GetComponent<BulletController>().FireBullet(direction, BulletSpeed, owner);
        }

        [ClientRpc]
        public void RpcReset()
        {
            foreach (var b in _bullets)
                b.SetActive(false);
        }

    }
}
