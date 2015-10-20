using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Assets.Scripts.Weapons
{
    public class BulletManager : WeaponController
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

        public override void RpcFireWeapon(Vector3 origin, Vector3 direction)
        {
            var bullet = GetBullet();
            bullet.transform.position = origin;
            bullet.transform.Translate(direction * 4f);
            bullet.transform.Translate(Vector3.up * 1.5f);
            bullet.transform.Translate(Vector3.right * -.35f);

            bullet.GetComponent<BulletController>().FireBullet(direction, BulletSpeed);
        }

        
    }
}
