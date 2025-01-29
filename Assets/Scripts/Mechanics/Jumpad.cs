using UnityEngine;

public class Jumpad : MonoBehaviour
{
    public bool isRepel = false;

    public float jumpForce = 10f;       // Fuerza vertical
    public float forwardForce = 5f;     // Fuerza horizontal hacia adelante

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {

                Vector3 playerPosition = other.transform.position;
                Vector3 directionToPlayer = new Vector3(playerPosition.x - transform.position.x,0, playerPosition.z - transform.position.z);
                //directionToPlayer.Normalize();

                if (other.GetComponent<PlayerControl>() != null) other.GetComponent<PlayerControl>().LockMovement(true);
                if (other.GetComponent<BotNoNetwork>() != null) other.GetComponent<BotNoNetwork>().LockMovement(true);
                if (other.GetComponent<NPCController>() != null) other.GetComponent<NPCController>().LockMovement(true);

                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;

                Vector3 jumpDirection;

                if (isRepel)
                {
                    jumpDirection = transform.up * jumpForce + (transform.right * directionToPlayer.x + transform.forward * directionToPlayer.z) * forwardForce; //Calcular la dirección combinada hacia el Player y hacia arriba
                }
                else
                {
                    playerRigidbody.rotation = Quaternion.identity;
                    jumpDirection = transform.up * jumpForce + transform.forward * forwardForce; //Calcular la dirección combinada hacia adelante y hacia arriba
                }

                // Aplicar la fuerza combinada
                playerRigidbody.AddForce(jumpDirection, ForceMode.VelocityChange);
            }
        }
    }
}
