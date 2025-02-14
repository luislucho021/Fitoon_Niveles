using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
	[SerializeField] GameObject entryPrefab;
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] GameObject content;
	[SerializeField] Text testText;

	public override void OnStartClient()
	{
		Debug.Log("I'm a Client!");
		testText.text = "I'm a Client";
		SaveData.ReadFromJson();
		AddPlayerToRaceRpc();
		UpdateList(SaveData.player.playerCharacterData.characterName);

	}

	public override void OnStartServer()
	{
		InstanceFinder.NetworkManager.ClientManager.StartConnection();
		InstanceFinder.NetworkManager.ServerManager.OnAuthenticationResult += OnClientConnected;
		
		Debug.Log("I'm a Server!");
	}

	private void OnClientConnected(NetworkConnection connection, bool arg2)
	{
		if (arg2)
		{
			Debug.Log("Client has connected to me");
		}
		else
			Debug.Log("Something went wrong");
	}


	private void Update()
	{
		if(IsServerInitialized)
		{
			testText.text = "I'm a Server ";
			testText.text += InstanceFinder.NetworkManager.ServerManager.Clients.Count;
		}
		Debug.Log("Client Initialized: " + IsClientInitialized);
		Debug.Log("Server Initialized: " + IsServerInitialized);
		Debug.Log("Host Initialized: " + IsHostInitialized);
	}

	[ServerRpc]
    void AddPlayerToRaceRpc()
    {
        GameManager.AddPlayer();
	}

    [ObserversRpc]
    void UpdateList(string name)
    {
        GameObject entry = Instantiate(entryPrefab, content.transform);
		entry.name = name;
        entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
		Canvas.ForceUpdateCanvases();
		//ScrollViewFocusFunctions.FocusOnItem(scrollRect, playerEntry);
		StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, entry.GetComponent<RectTransform>(), 0.5f));
	}
	
}
