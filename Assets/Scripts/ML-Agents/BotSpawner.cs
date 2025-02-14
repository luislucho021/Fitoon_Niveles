using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class BotSpawner : NetworkBehaviour
{
    public GameObject botPrefab;
    public Transform goal;

    private string spawnedBotName = "";
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private void Start()
    {
    }


    public void SpawnBot(Vector3 spawnpointPosition)
    {
        Debug.Log("[SERVER] Bot Spawned");
        GameObject inst = Instantiate(botPrefab, spawnpointPosition, transform.rotation);
        inst.name = spawnedBotName;
        //inst.GetComponent<MoveToTargetPlay>().target = goal;
        //inst.GetComponent<MoveToTargetPlay>().spawnpoint = spawnpointPosition;
        GetComponent<NetworkObject>().Spawn(inst);
        inst.transform.parent = transform;
    }


    public void FillSpawnpointsWithBots()
    {
        

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
