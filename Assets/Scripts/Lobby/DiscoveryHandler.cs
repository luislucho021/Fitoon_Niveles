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
	[SerializeField] ushort port = 7077;

	void Start()
    {
		netDiscovery = networkManager.GetComponent<NetworkDiscovery>();
		netDiscovery.SearchForServers();
		netDiscovery.ServerFoundCallback += ConnectToServer;
		StartCoroutine(SearchTimeOut());
	}

	private void ConnectToServer(IPEndPoint point)
	{
		netDiscovery.ServerFoundCallback -= ConnectToServer;
		StopAllCoroutines();
		netDiscovery.StopSearchingOrAdvertising();
		networkManager.ClientManager.StartConnection(point.Address.ToString(), port);
	}

	IEnumerator SearchTimeOut()
	{
		yield return new WaitForSeconds(2);
		if (netDiscovery.IsSearching)
		{
			netDiscovery.ServerFoundCallback -= ConnectToServer;
			netDiscovery.StopSearchingOrAdvertising();
			networkManager.ServerManager.StartConnection(port);
			netDiscovery.AdvertiseServer();
		}
	}
}
