using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class MissileManager : MonoBehaviour, IWeaponController {

        public float MissileSpeed = 0.1f;

        private List<GameObject> _missiles;
        private GameObject _missilePrefab;


        // Use this for initialization
        void Start()
        {

            _missilePrefab = (GameObject)Resources.Load("Weapons/Missile");
            _missiles = new List<GameObject>();
            for (var i = 0; i < 10; i++)
            {
                var b = Instantiate(_missilePrefab);
                b.transform.parent = transform;
                b.SetActive(false);
                _missiles.Add(b);
            }
        }

        private GameObject GetMissile()
        {
            var b = _missiles.FirstOrDefault(s => !s.activeSelf);
            if (!b)
            {
                b = Instantiate(_missilePrefab);
                b.transform.parent = transform;
                _missiles.Add(b);
            }
            b.SetActive(true);
            return b;
        }

        public void FireWeapon(Vector3 origin, Vector3 direction)
        {
            var missile = GetMissile();
            missile.transform.position = origin;
            
            missile.transform.Translate(direction * 3.5f);
            missile.transform.Translate(Vector3.up * 1.5f);
            //missile.transform.localRotation = Quaternion.Euler(direction);
            missile.transform.GetChild(0).localRotation = Quaternion.LookRotation(direction, Vector3.up);
            missile.GetComponent<MissileController>().FireMissile(direction, MissileSpeed);
        }

    }
}
