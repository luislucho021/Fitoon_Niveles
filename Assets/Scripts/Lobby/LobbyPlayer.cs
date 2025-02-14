using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
	
	public override void OnStartClient()
	{
		if(!IsOwner) return;

		Debug.Log("I'm a Client! Owner: " + Owner);
		SaveData.ReadFromJson();
		AddPlayerToRaceRpc(SaveData.player.playerCharacterData.characterName);
	}
	[ServerRpc]
	void AddPlayerToRaceRpc(string name)
	{
		GameManager.AddPlayer();
		LobbyManager.Instance.UpdateList(name);
	}
}
