using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosion;
    Rigidbody rb;
    float launchTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        launchTime = Time.time;
    }
    
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * 30);
        if (Time.time - launchTime > 3)
        {
                Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
