using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraMovement : MonoBehaviour
    {

        private Transform _player;

        // Use this for initialization
        void Start ()
        {
            _player = GameObject.FindGameObjectWithTag(Constants.Tags.Player).transform;
        }
	
        // Update is called once per frame
        void Update () {
            transform.position = new Vector3(_player.position.x, transform.position.y, _player.position.z);
        }
    }
}
