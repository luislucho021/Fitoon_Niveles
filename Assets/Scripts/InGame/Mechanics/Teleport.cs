using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject destination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        if (destination != null)
        {
            //if (player.GetComponent<PlayerController>() != null) player.GetComponent<PlayerController>().LockMovement(true);
            //if (player.GetComponent<BotController>() != null) player.GetComponent<BotController>().LockMovement(true);

            //Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();

            //playerRigidbody.rotation = Quaternion.identity;
            //playerRigidbody.velocity = Vector3.zero;
            //playerRigidbody.angularVelocity = Vector3.zero;
            player.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y + 1, destination.transform.position.z);

            //if (player.GetComponent<PlayerController>() != null) player.GetComponent<PlayerController>().LockMovement(false);
            //if (player.GetComponent<BotController>() != null) player.GetComponent<BotController>().LockMovement(false);
        }
        else
        {
            //if (player.GetComponent<PlayerController>() != null) player.GetComponent<PlayerController>().LockMovement(false);
            //if (player.GetComponent<BotController>() != null) player.GetComponent<BotController>().LockMovement(false);
        }
    }
}
