using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;

        private Rigidbody _rb;

        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody>();
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
                GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletController>().FireBullet(transform.position, transform.forward);
        }

        public void HitByBullet()
        {
            //TODO Not Implemented
        }
    }
}
