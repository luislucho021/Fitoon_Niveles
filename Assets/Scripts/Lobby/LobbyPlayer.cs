using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
	string playerName;
	public override void OnStartClient()
	{
		if (!IsOwner) return;

		Debug.Log("I'm a Client! Owner: " + Owner);
		SaveData.ReadFromJson();
		playerName = SaveData.player.username;
		AddPlayerToRaceRpc(playerName, InstanceFinder.ClientManager.Connection);
	}

	[ServerRpc]
	void AddPlayerToRaceRpc(string name, NetworkConnection connection)
	{
		LobbyManager.Instance.AddPlayerCard(name, connection);
	}
}
