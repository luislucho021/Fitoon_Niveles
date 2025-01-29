using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CameraOcclusionHandler : MonoBehaviour
{
    public LayerMask obstacleMask; // Layer mask for obstacles that can block the view
    public string hideLayerName = "HideFromCamera"; // The layer to switch to when an object blocks the view
    public string gameLayerName = "Game"; // The original layer of the object
    public string playerName = "Player"; // Name of the player prefab

    private Transform player; // Reference to the player's transform
    private GameObject playerObject;
    private int hideLayer;
    private int gameLayer;
    private List<GameObject> hiddenObjects = new List<GameObject>(); // List of objects that are hidden
    private int countdown = 2;
    private GameObject hiddenObject;

    void Start()
    {
        // Find the player object by name and get its transform
        playerObject = GameObject.Find(playerName);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError($"Player object with name '{playerName}' not found.");
        }

        // Get the layer indices
        hideLayer = LayerMask.NameToLayer(hideLayerName);
        gameLayer = LayerMask.NameToLayer(gameLayerName);
    }

    void Update()
    {
        if (player == null)
        {
            // Attempt to find the player object again in case it was not found initially
            playerObject = GameObject.Find(playerName);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // Calculate the direction from the camera to the player
        Vector3 direction = player.position - transform.position;

        // Log player's current position
        //Debug.Log($"Player position: {player.position}");

        // Draw a debug line to visualize the raycast
        Debug.DrawLine(transform.position, player.position, Color.red);

        // Raycast to detect objects between the camera and the player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude, obstacleMask))
        {
            Debug.Log("He detectado algo");
            GameObject hitObject = hit.collider.gameObject;

            // Change the object's layer to hide it from the camera
            if (hitObject.layer != hideLayer)
            {
                Debug.Log($"Changing layer of {hitObject.name} to {hideLayerName}");
                hitObject.layer = hideLayer;
                hiddenObjects.Add(hitObject);
            }
        }

        // Revert the layer of previously hidden objects that are no longer blocking the view
        for (int i = hiddenObjects.Count - 1; i >= 0; i--)
        {
            hiddenObject = hiddenObjects[i];
            if (hiddenObject.layer == hideLayer)
            {
                StartCoroutine(Countdown()); // Volver a hacer visible en 2s
                hiddenObjects.RemoveAt(i);
            }
        }
    }

    IEnumerator Countdown()
    {
        while (countdown != 0)
        {
            yield return new WaitForSeconds(2f);
            countdown--;
            if (countdown == 0)
            {
                Debug.Log($"Reverting layer of {hiddenObject.name} to {gameLayerName}");
                hiddenObject.layer = gameLayer;
            }
        }

    }
}