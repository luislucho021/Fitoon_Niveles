using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public List<string> explodeOnTags = new List<string>();
    public GameObject explosionPrefab;

    public void Explode()
    {
        // TODO: generar sistema de partículas
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(explosion, 0.6f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (explodeOnTags.Contains(other.gameObject.tag)) {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Jumpad>().Bounce(collision.gameObject);
        if (explodeOnTags.Contains(collision.gameObject.tag)) {
            Explode();
        }
    }
}
