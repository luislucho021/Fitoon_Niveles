using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Object.Synchronizing.Internal;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
	public class PlayerCard
	{
		public string name;
		public bool ready;
	}

	readonly SyncDictionary<NetworkConnection, PlayerCard> playerEntries = new SyncDictionary<NetworkConnection, PlayerCard>();

	[SerializeField] GameObject cardPrefab;
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] public GameObject content;
	[SerializeField] TextMeshProUGUI countDownText;
	[SerializeField] TextMeshProUGUI playerCountText;

	List<GameObject> cardList = new List<GameObject>();

	bool starting = false;
	bool disconnectOnDisable = false;

	public static LobbyManager Instance;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	void OnDisable()
	{
		if (!disconnectOnDisable)
		{
			return;
		}
		if (InstanceFinder.NetworkManager != null)
		{
			InstanceFinder.NetworkManager.ClientManager.StopConnection();
			InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
			Destroy(InstanceFinder.NetworkManager.gameObject);
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	private void OnApplicationQuit()
	{
		InstanceFinder.NetworkManager.ClientManager.StopConnection();
		InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
		Destroy(InstanceFinder.NetworkManager.gameObject);
	}

	private void Update()
	{
		Debug.Log(playerEntries.Keys.Count);
		Debug.Log(playerEntries.Values.Count);
		List<PlayerCard> playerCards = playerEntries.Values.ToList();
		int i;
		for(i = 0; i  < playerCards.Count; i++)
		{
			if(i >= cardList.Count)
			{
				cardList.Add(Instantiate(cardPrefab, content.transform));
			}
			cardList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerCards[i].name;
			cardList[i].transform.GetChild(1).GetComponent<Image>().enabled = playerCards[i].ready;
		}
		for(int j = cardList.Count - 1; j >= i; j--)
		{
			Destroy(cardList[j]);
			cardList.RemoveAt(j);
		}
	}

	public override void OnStartNetwork()
	{
		disconnectOnDisable = true;
	}

	public override void OnStartServer()
	{
		InstanceFinder.NetworkManager.ClientManager.StartConnection();
		InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
		playerEntries.OnChange += OnChangePlayerEntries;
		Debug.Log("I'm a Server!");
	}

	private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
	{
		if(args.ConnectionState == RemoteConnectionState.Stopped)
		{
			playerEntries.Remove(connection);
		}
	}

	private void OnChangePlayerEntries(SyncDictionaryOperation op, NetworkConnection key, PlayerCard value, bool asServer)
	{
		if(!IsServerInitialized)
		{
			return;
		}
		SetPlayerNumber(playerEntries.Count);
		CheckReady();
	}

	public void ReadyButton()
	{
		SetReady(InstanceFinder.ClientManager.Connection);
	}

	public void ExitButton()
	{
		if (InstanceFinder.NetworkManager != null)
		{
			InstanceFinder.NetworkManager.ClientManager.StopConnection();
			InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
		}
		Destroy(InstanceFinder.NetworkManager.gameObject);
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	[ServerRpc(RequireOwnership = false)]
	public void AddPlayer(NetworkConnection key, string name)
	{
		PlayerCard card = new PlayerCard()
		{
			name = name,
			ready = false
		};
		playerEntries.Add(key, card);
		playerEntries.Dirty(key);
	}

	[ServerRpc(RequireOwnership = false)]
	void SetReady(NetworkConnection connection)
	{
		PlayerCard card;
		if (playerEntries.TryGetValue(connection, out card))
			card.ready = !card.ready;
		playerEntries.Dirty(connection);
	}

	[ServerRpc(RequireOwnership = false)]
	public void CheckReady()
	{
		if (!IsServerInitialized)
			return;
		int ready = 0;
		foreach (PlayerCard card in playerEntries.Values)
		{
			if (card.ready)
			{
				ready++;
			}
		}
		if (ready / (float)playerEntries.Count >= 0.6f)
		{
			if (starting)
				return;

			InstanceFinder.NetworkManager.GetComponent<NetworkDiscovery>().StopSearchingOrAdvertising();
			starting = true;
			disconnectOnDisable = false;
			StartCoroutine(StartGameCountdown());
		}
		else
		{
			ChangeCountdownText("WAITING FOR PLAYERS");
			InstanceFinder.NetworkManager.GetComponent<NetworkDiscovery>().AdvertiseServer();
			starting = false;
			disconnectOnDisable = true;
		}
	}

	[ObserversRpc]
	public void ChangeCountdownText(string s)
	{
		countDownText.text = s;
	}

	[ObserversRpc]
	public void SetPlayerNumber(int i)
	{
		playerCountText.text = i.ToString() + "/32";
	}

	IEnumerator StartGameCountdown()
	{
		if (!starting)
			yield break;
		for (int i = 5; i >= 0; i--)
		{


			ChangeCountdownText("STARTING IN " + i.ToString());
			Debug.Log("Countdown: " + i.ToString());
			yield return new WaitForSeconds(1);
			if (!starting)
				yield break;
		}
		//Cambiar escena
	}
}
