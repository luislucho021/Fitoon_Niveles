using FishNet.Discovery;
using FishNet.Managing;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DiscoveryHandler : MonoBehaviour
{
	NetworkDiscovery netDiscovery;
	[SerializeField] NetworkManager networkManager;
	[SerializeField] NetworkBehaviour lobbyHandler;

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Start");
		netDiscovery = networkManager.GetComponent<NetworkDiscovery>();
		netDiscovery.SearchForServers();
		netDiscovery.ServerFoundCallback += ConnectToServer;
		StartCoroutine(SearchTimeOut());
	}

	private void ConnectToServer(IPEndPoint point)
	{
		netDiscovery.ServerFoundCallback -= ConnectToServer;
		Debug.Log("Server Found : " + point.Address.ToString() + (ushort)point.Port);
		StopAllCoroutines();
		netDiscovery.StopSearchingOrAdvertising();
		networkManager.ClientManager.StartConnection(point.Address.ToString(), 7077);
	}

	private void Update()
	{
		Debug.Log("Is client: " + networkManager.ClientManager.Started + " ");
		Debug.Log("Is server: " + networkManager.ServerManager.Started + " ");
	}

	IEnumerator SearchTimeOut()
	{
		Debug.Log("Searching");
		yield return new WaitForSeconds(2);
		if (netDiscovery.IsSearching)
		{
			netDiscovery.ServerFoundCallback -= ConnectToServer;
			netDiscovery.StopSearchingOrAdvertising();
			networkManager.ServerManager.StartConnection(7077);
			netDiscovery.AdvertiseServer();
		}
	}
}
