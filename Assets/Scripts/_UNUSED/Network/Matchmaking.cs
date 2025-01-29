using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Matchmaking : MonoBehaviour
{
    public Transform botsFolder;

    [SerializeField] private NetworkData netData;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private string _defaultRegion = "europe-west2"; //europe-west4

    private Lobby _connectedLobby;
    private UnityTransport _transport;
    private const string JoinCodeKey = "k";
    private const string LobbyName = "Fitoon_Lobby";
    private string _playerId;

    private void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();
        //Debug.Log("TRANSPORT: " + _transport);
        Debug.Log("MATCHMAKING AWAKE");
    } 

    public async void CreateOrJoinLobby() {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby != null)
        {
            _playButton.transform.parent.gameObject.SetActive(false);
            _exitButton.transform.parent.gameObject.SetActive(true);
            _exitButton.transform.gameObject.SetActive(false);
        }

    }

    public void StartRaceLobby()
    {
        //Lock();
    }

    private async Task Lock()
    {
        Debug.Log("Locking");
        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions()
        {
            IsPrivate = true,
            IsLocked = true
        };
        var updatedLobby = await Lobbies.Instance.UpdateLobbyAsync(_connectedLobby.Id, updateOptions);

        Debug.Log($"Finish with lock {updatedLobby.IsLocked}");
        // update the lobby member to be the updated lobby
        _connectedLobby = updatedLobby;
    }


    public void DestroyOrExitLobby()
    {
        Camera.main.enabled = true;

        foreach (Transform child in botsFolder)
        {
            Destroy(child.gameObject);
        }

        LeaveLobby();

        if (_connectedLobby != null)
        {
            Debug.Log("DESTROY");
            _playButton.transform.parent.gameObject.SetActive(true);
            _exitButton.transform.parent.gameObject.SetActive(false);
            _exitButton.SetActive(false);
            _startButton.SetActive(true);
        }
        AuthenticationService.Instance.SignOut();
        NetworkManager.Singleton.Shutdown();
        _connectedLobby = null;
    }

    private async Task Authenticate()
    {
        var options = new InitializationOptions();

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;


        Debug.Log("Auth: " + _playerId);
    }

    private async Task<Lobby> QuickJoinLobby() {
        try {
            // Attempt to join a lobby in progress
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e) {
            Debug.Log($"No lobbies available via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby() {
        try
        {
            int maxPlayers = netData.GetMaxPlayersPerLobby(); //Should be equal to the number of spawnpoints

            // Create a relay allocation and generate a join code to share with the lobby
            Allocation a;

            if (_defaultRegion == "") a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            else a = await RelayService.Instance.CreateAllocationAsync(maxPlayers, _defaultRegion);

            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            // Create a lobby, adding the relay join code to the lobby data
            var options = new CreateLobbyOptions {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await Lobbies.Instance.CreateLobbyAsync(LobbyName, maxPlayers, options);

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Set the game room to use the relay allocation
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            // Start the room. I'm doing this immediately, but maybe you want to wait for the lobby to fill up
            ///todo: 60 secs countdown until the game starts. When the countdown ends, the room is filled up with bots. If the room is filled up before the end of the countdown, start the game either way.
            NetworkManager.Singleton.StartHost();
            //Debug.LogFormat("Connecting to new lobby created: " + lobby.Id);
            return lobby;
        }
        catch (Exception e) {
            Debug.LogFormat("Failed creating a lobby");
            return null;
        }
    }

    private void SetTransformAsClient(JoinAllocation a) {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true) {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void LeaveLobby()
    {
        try
        {
            StopAllCoroutines();

            if (_connectedLobby != null)
            {
                if (_playerId == _connectedLobby.HostId) //If the player leaving the room is the Host
                {
                    ///todo: Migrate other clients to new lobby and assign new host to one of them
                    Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                    Debug.Log("Lobby " + _connectedLobby.Id + " deleted");
                }
                else //If the player leaving the room is a client
                {
                    Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
                    Debug.Log("Player " + _playerId + " removed");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }

    private void OnDestroy()
    {
        LeaveLobby();
    }


}


