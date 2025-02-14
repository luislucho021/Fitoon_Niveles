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
	[SerializeField] public GameObject content;

	public static LobbyManager Instance;
	List<NetworkObject> entries = new List<NetworkObject> ();

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
	public override void OnStartServer()
	{
		InstanceFinder.NetworkManager.ClientManager.StartConnection();
		InstanceFinder.NetworkManager.ServerManager.OnAuthenticationResult += OnClientConnected;
		
		Debug.Log("I'm a Server!");
	}

	private void OnClientConnected(NetworkConnection connection, bool arg2)
	{
		if (!arg2)
		{
			Debug.Log("Problem");
			return;
		}
		UpdateListForNewClient(connection);
	}

    public void UpdateList(string name)
    {
        NetworkObject entry = Instantiate(entryPrefab, content.transform).GetComponent<NetworkObject>();
		InstanceFinder.NetworkManager.ServerManager.Spawn(entry);
		entry.name = name;
        entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
		entry.GetComponent<PlayerCard>().SetName(name);
		Canvas.ForceUpdateCanvases();
		//ScrollViewFocusFunctions.FocusOnItem(scrollRect, playerEntry);
		StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, entry.GetComponent<RectTransform>(), 0.5f));
	}
	
	void UpdateListForNewClient(NetworkConnection connection)
	{
		foreach(NetworkObject entry in entries)
		{
			entry.GetComponent<PlayerCard>().SetName(connection, entry.name);
		}
	}
}
