using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class DebugButtons : MonoBehaviour {

    public GameObject botPrefab;
    public GameObject botParent;

    private void OnGUI() {

        GUILayout.BeginArea(new Rect(10, 10, Screen.width/4, Screen.height / 2));
        if (NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Spawn Bot", GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 20)))
            {
                GameObject inst = Instantiate(botPrefab);
                inst.transform.parent = botParent.transform;
            }
        }
        GUILayout.EndArea();
    }
}