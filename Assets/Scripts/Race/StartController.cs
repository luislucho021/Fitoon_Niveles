using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartController : NetworkBehaviour
{
    public BotSpawner botSpawner;
    public Countdown countdownTimer;
    public NetworkData netData;
    public GameObject startButton;

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.IsServer) startButton.SetActive(false);
    }

    [ClientRpc]
    private void StartCountdownClientRpc()
    {
        countdownTimer.StartCountdown();
    }

    [ContextMenu("Start Race")]
    public void StartRace()
    {
        if (NetworkManager.IsServer)/// ToDo: Add 60s timer and fill with bots if it ends (currently there is a "start" button for the server client).
        {
            //Cursor.visible = false;
            Debug.Log("START");
            botSpawner.FillSpawnpointsWithBots(); //Spawn Bots
            StartCountdownClientRpc();//Send RPC to clients to start countdown aswell
            netData.SetCurrentMaxPlayersPerLobby(netData.GetPlayerCount());
            startButton.SetActive(false);
        }
    }

}
