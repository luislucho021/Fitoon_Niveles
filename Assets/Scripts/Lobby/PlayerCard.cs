using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : NetworkBehaviour
{
	readonly SyncVar<string> _name = new SyncVar<string>();
	public override void OnStartClient()
	{
		transform.SetParent(LobbyManager.Instance.content.transform);
		transform.GetChild(0).name = _name.Value;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _name.Value;
		Canvas.ForceUpdateCanvases();
	}

	[ObserversRpc]
	public void SetName(string name)
	{
		_name.OnChange += ChangeName;
		_name.Value = name;
	}

	private void ChangeName(string prev, string next, bool asServer)
	{
		transform.GetChild(0).name = next;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = next;
		Canvas.ForceUpdateCanvases();
	}

	[TargetRpc]
	public void SetName(NetworkConnection connection, string name)
	{
		Debug.Log("SetNamed");
		transform.GetChild(0).name = name;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
		Canvas.ForceUpdateCanvases();
	}
}
