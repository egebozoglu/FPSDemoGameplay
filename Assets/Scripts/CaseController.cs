using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseController : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Movement();
    }
    private void Update()
    {
        Rotation();
    }

    void Movement()
    {
        rb.AddForce(transform.right * 100f);
        rb.AddForce(transform.up * 50f);
    }

    void Rotation()
    {
        transform.Rotate(transform.right * 500f, Space.Self);
    }
}
