using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerNameHolder : NetworkBehaviour
{
    public GameObject textHolder;
    public Vector3 offset;

    private Vector3 startPos;


    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
        new PlayerData
        {
            _playerId = "",
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct PlayerData : INetworkSerializable
    {
        public string _playerId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _playerId);
        }
    }

    private void Start()
    {
        if (IsOwner) textHolder.GetComponent<TextMeshPro>().faceColor = Color.cyan;
        else textHolder.GetComponent<TextMeshPro>().faceColor = Color.red;

        startPos = textHolder.transform.localPosition - new Vector3(0,1,0);
        
        if (IsOwner)
        {
            playerData.Value = new PlayerData
            {
                _playerId = AuthenticationService.Instance.PlayerId,
            };

            textHolder.GetComponent<TextMeshPro>().text = playerData.Value._playerId;
            SendPlayerDataServerRpc(playerData.Value._playerId);
        }
        else
        {
            textHolder.GetComponent<TextMeshPro>().text = playerData.Value._playerId;
        }

    }

    [ServerRpc]
    private void SendPlayerDataServerRpc(string playerName)
    {
        textHolder.GetComponent<TextMeshPro>().text = playerName;

        SendPlayerDataClientRpc(playerName);
    }

    [ClientRpc]
    private void SendPlayerDataClientRpc(string playerName)
    {
        if (!IsOwner) textHolder.GetComponent<TextMeshPro>().text = playerName;
    }

    void Update()
    {
        textHolder.transform.localPosition = startPos + transform.localPosition + offset;
    }

}
