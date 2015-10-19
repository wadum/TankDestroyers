using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    public class PlayerNetworkController : NetworkBehaviour
    {

        public GameObject Camera;

        // Use this for initialization
        void Start () {
            if (!isLocalPlayer) return;

            gameObject.GetComponent<Scripts.PlayerController>().DisableControls = false;
            gameObject.GetComponent<AudioSource>().enabled = true;
            Camera.SetActive(true);
        }
    }
}
