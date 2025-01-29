using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotSpawner : NetworkBehaviour
{
    public GameObject botPrefab;
    public Transform goal;

    private NetworkData netData;
    private string spawnedBotName = "";
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private void Start()
    {
        if (netData == null) netData = FindObjectOfType<NetworkData>();
    }


    public void SpawnBot(Vector3 spawnpointPosition)
    {
        Debug.Log("[SERVER] Bot Spawned");
        GameObject inst = Instantiate(botPrefab, spawnpointPosition, transform.rotation);
        inst.name = spawnedBotName;
        //inst.GetComponent<MoveToTargetPlay>().target = goal;
        //inst.GetComponent<MoveToTargetPlay>().spawnpoint = spawnpointPosition;
        inst.GetComponent<NetworkObject>().Spawn();
        inst.transform.parent = transform;
    }


    public void FillSpawnpointsWithBots()
    {
        if (IsServer){

            for (int i = 0; i < netData.spawnpointData.Count; i++)
            {
                if (!netData.spawnpointData[i]._isOccupied)
                {
                    //Generate unique random bot name
                    GenerateUniqueRandomName(i + 1);

                    //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied = true and the playerId on it
                    netData.UpdateSpawnpointsList(i, netData.spawnpointData[i]._spPosition, true, spawnedBotName);

                    SpawnBot(netData.spawnpointData[i]._spPosition + new Vector3(0, 1, 0));
                }
            }
        }

    }


    private void GenerateUniqueRandomName(int botNum)
    {
        spawnedBotName = "Bot#" + botNum + "-"
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)] 
            + chars[Random.Range(0, chars.Length)] 
            + chars[Random.Range(0, chars.Length)] 
            + chars[Random.Range(0, chars.Length)];
    }

}
