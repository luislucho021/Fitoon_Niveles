using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
	public readonly SyncVar<int> readyCount = new SyncVar<int>();
	class Entry
	{
		public PlayerCard card;
		public NetworkConnection connection;
	}
	[SerializeField] PlayerCard entryPrefab;
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] public GameObject content;
	[SerializeField] LobbyHUD lobbyHud;

	bool starting = false;
	bool disconnectOnDisable = false;
	List<Entry> entries = new List<Entry>();
	public static LobbyManager Instance;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public override void OnStartNetwork()
	{
		disconnectOnDisable = true;
	}

	public override void OnStartServer()
	{
		readyCount.Value = 0;
		readyCount.OnChange += CheckReady;
		InstanceFinder.NetworkManager.ClientManager.StartConnection();
		InstanceFinder.NetworkManager.ServerManager.OnRemoteConnectionState += OnClientDisconnect;
		Debug.Log("I'm a Server!");
	}

	private void CheckReady(int prev, int next, bool asServer)
	{
		if (starting)
			return;
		if(entries.Count == 0)
			return;


		starting = next / (float)entries.Count >= 0.6f;
		Debug.Log("CheckReady: " + next.ToString() + "/" + entries.Count.ToString() + " = " + starting.ToString());
		if(starting)
		{
			Debug.Log("Start countdown");
			InstanceFinder.NetworkManager.GetComponent<NetworkDiscovery>().StopSearchingOrAdvertising();
			StartCoroutine(StartGameCountdown());
		}
	}

	private void OnClientDisconnect(NetworkConnection connection, RemoteConnectionStateArgs args)
	{
		if (!(args.ConnectionState == RemoteConnectionState.Stopped)) return;
		GameManager.RemovePlayer();
		lobbyHud.RemovePlayer();
		Entry entry = entries.Find((entry) => entry.connection == connection);
		if (entry.card.IsReady())
			readyCount.Value--;
		Despawn(entry.card.gameObject);
	}

	void OnDisable()
	{
		if (!disconnectOnDisable)
		{
			return;
		}
		
		InstanceFinder.NetworkManager.ClientManager.StopConnection();
		InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
		Destroy(InstanceFinder.NetworkManager.gameObject);
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public void AddPlayerCard(string name, NetworkConnection connection)
    {
		if(!IsServerInitialized)
		{
			return;
		}
		GameManager.AddPlayer();
		lobbyHud.AddPlayer();
        PlayerCard card = Instantiate(entryPrefab, content.transform);
		InstanceFinder.NetworkManager.ServerManager.Spawn(card.gameObject, connection);
		card.name = name;
        card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
		card.GetComponent<PlayerCard>().SetName(name);
		Entry entry = new Entry();
		entry.card = card;
		entry.connection = connection;
		entries.Add(entry);
		Canvas.ForceUpdateCanvases();
		//ScrollViewFocusFunctions.FocusOnItem(scrollRect, playerEntry);
		StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, card.GetComponent<RectTransform>(), 0.5f));
	}

	public void ReadyButton()
	{
		foreach(var entry in entries)
		{
			if(entry.card.IsOwner)
			{
				entry.card.ReadyUp();
			}
		}
	}

	IEnumerator StartGameCountdown()
	{
		for(int i = 5; i >= 0; i--)
		{
			lobbyHud.ChangeCountdownText("STARTING IN " + i.ToString());
			Debug.Log("Countdown: " + i.ToString());
			yield return new WaitForSeconds(1);
		}
		//Cambiar escena
	}
}
