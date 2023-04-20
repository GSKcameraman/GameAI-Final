using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    public float speed = 10.0f;
    public float angularVelocity = 200.0f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float foward = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Tilt");
        float lr = Input.GetAxis("Horizontal");
        transform.position += foward * speed * transform.forward * Time.deltaTime;
        transform.position += lr * speed * transform.right * Time.deltaTime;
        rb.AddTorque(new Vector3(0, turn* angularVelocity*Time.deltaTime * rb.mass, 0));
    }
}
