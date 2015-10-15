using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public interface IWeaponController
    {

        void FireWeapon(Vector3 origin, Vector3 direction);
    }
}
