using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Weapons
{
    public abstract class WeaponController: NetworkBehaviour
    {

        [ClientRpc]
        public abstract void RpcFireWeapon(Vector3 origin, Vector3 direction);

        [Command]
        public void CmdFireWeapon(Vector3 origin, Vector3 direction)
        {
            RpcFireWeapon(origin, direction);
        }
    }
}
