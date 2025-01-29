using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkButtons : MonoBehaviour {
    private void OnGUI() {

        GUILayout.BeginArea(new Rect(10, 10, Screen.width/4, Screen.height / 2));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Server", GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 20))) NetworkManager.Singleton.StartServer();
            if (GUILayout.Button("Host", GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 20))) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client", GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 20))) NetworkManager.Singleton.StartClient();
        }

        GUILayout.EndArea();
    }

    // private void Awake() {
    //     GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //         packetDelay: 120,
    //         packetJitter: 5,
    //         dropRate: 3);
    // }
}