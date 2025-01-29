using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkData : NetworkBehaviour
{

    public List<SpawnpointData> spawnpointData;
    public NetworkManager netManager;

    private int playerCount;
    private readonly int maxPlayersPerLobby = 32;
    private int currentMaxPlayersPerLobby = 32;

    public struct SpawnpointData
    {
        public Vector3 _spPosition;
        public bool _isOccupied;
        public string _playerId;

        public SpawnpointData(Vector3 spPosition, bool isOccupied, string playerId)
        {
            _spPosition = spPosition;
            _isOccupied = isOccupied;
            _playerId = playerId;
        }

    }

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 30;
        spawnpointData = new List<SpawnpointData>();
        Debug.Log("NETWORK DATA AWAKE");
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) GetPlayerCount();

        GenerateSpawnpointsList();
    }

    public void UpdateSpawnpointsList(int index, Vector3 spPosition, bool isOccupied, string playerId)
    {
        if (IsServer) spawnpointData[index] = new SpawnpointData(spPosition, isOccupied, playerId);
    }

    public void RemovePlayerFromSpawnpoint(int index, Vector3 spPosition, bool isOccupied, string playerId)
    {
        if (IsServer) spawnpointData[index] = new SpawnpointData(spPosition, isOccupied, playerId);
    }

    public void GenerateSpawnpointsList()
    {
        spawnpointData.Clear();

        if (IsServer)//Server or Host
        {
            foreach (var gO in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (gO.tag == "SpawnPoint")
                {
                    spawnpointData.Add(new SpawnpointData(gO.transform.position, false,""));
                    Debug.Log("[ND]-SP Added");
                }
            }
        }

    }

    public int GetPlayerCount()
    {
        playerCount = netManager.ConnectedClients.Count;
        Debug.Log(playerCount);
        return playerCount;
    }
    
    public int GetMaxPlayersPerLobby()
    {
        return maxPlayersPerLobby;
    }
    
    public int GetCurrentMaxPlayersPerLobby()
    {
        return currentMaxPlayersPerLobby;
    }
    
    public void SetCurrentMaxPlayersPerLobby(int value)
    {
        currentMaxPlayersPerLobby = value;
    }

}
