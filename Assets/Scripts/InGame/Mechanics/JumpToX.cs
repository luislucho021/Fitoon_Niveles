using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToX : MonoBehaviour
{
    public int jumpForce = 10;
    public int forwardForce = 5;
    Transform landingCross;

    private void Start()
    {
        landingCross = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 landingPosition = landingCross.position;

                if (other.GetComponent<PlayerControl>() != null) other.GetComponent<PlayerControl>().LockMovement(true);
                if (other.GetComponent<BotNoNetwork>() != null) other.GetComponent<BotNoNetwork>().LockMovement(true);
                if(other.GetComponent<NPCController>() != null) other.GetComponent<NPCController>().LockMovement(true);

                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;
                playerRigidbody.rotation = Quaternion.identity;

                Vector3 forwardDirection = (landingPosition - transform.position).normalized; //Direccion salto -> equis
                Vector3 jumpDirection = transform.up * jumpForce + forwardDirection * forwardForce; //Calcular la dirección combinada hacia adelante y hacia arriba

                // Aplicar la fuerza combinada
                playerRigidbody.AddForce(jumpDirection, ForceMode.Impulse);
                Debug.DrawRay(transform.position, jumpDirection, Color.red, 2f);
            }
        }
    }

}
