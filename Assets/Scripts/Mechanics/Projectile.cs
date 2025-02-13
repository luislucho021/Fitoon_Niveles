using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public List<string> explodeOnLayers = new List<string>();

    public void Explode()
    {
        // TODO: generar sistema de partículas
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (explodeOnLayers.Contains(other.gameObject.tag)) {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explodeOnLayers.Contains(collision.gameObject.tag)) {
            Explode();
        }
    }
}
