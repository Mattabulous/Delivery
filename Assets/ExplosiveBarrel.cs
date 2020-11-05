using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float forceRequired;
    public bool vulnerableToHit;
    public float radius = 5.0f;
    public float explosivePower = 10.0f;
    public GameObject bombFX;

    public void Explode()
    {
        Instantiate(bombFX, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosivePower, transform.position, radius, 3f);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Box") && vulnerableToHit)
        {
            if(collision.transform.GetComponent<Rigidbody>().velocity.magnitude >= forceRequired)
            {
                Explode();
            }
        }
    }
}
