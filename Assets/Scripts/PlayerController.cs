using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float RotationMagnitude = 0.5f;
    public float MovementMagnitude = 0.1f;

    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
	void Update () {
        if (Input.GetKey("a"))
            rb.AddTorque(new Vector3(0,1*RotationMagnitude,0));
        if (Input.GetKey("d"))
            rb.AddTorque(new Vector3(0, -1 * RotationMagnitude, 0));
        if (Input.GetKey("w"))
            rb.AddForce(transform.forward * MovementMagnitude);
        if (Input.GetKey("s"))
            rb.AddForce(transform.forward * -MovementMagnitude);
    }
}
