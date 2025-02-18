using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : NetworkBehaviour
{
	[SerializeField] Image check;
	readonly SyncVar<string> cardName = new SyncVar<string>();
	readonly SyncVar<bool> ready = new SyncVar<bool>();
	public override void OnStartClient()
	{
		transform.SetParent(LobbyManager.Instance.content.transform);
		transform.GetChild(0).name = cardName.Value;
		cardName.OnChange += ChangeName;
		ready.OnChange += OnReadyChanged;
		ready.Value = false;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cardName.Value;
		Canvas.ForceUpdateCanvases();
	}

	[ObserversRpc]
	public void SetName(string name)
	{
		cardName.Value = name;
	}

	private void ChangeName(string prev, string next, bool asServer)
	{
		transform.GetChild(0).name = next;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = next;
		Canvas.ForceUpdateCanvases();
	}
	private void OnReadyChanged(bool prev, bool next, bool asServer)
	{
		check.enabled = next;
	}
	public void ReadyUp()
	{
		ready.Value = !ready.Value;
		if(ready.Value)
		{
			LobbyManager.Instance.readyCount.Value++;
		}
		else
		{
			LobbyManager.Instance.readyCount.Value--;
		}
		check.enabled = ready.Value;
	}
	public bool IsReady()
	{
		return ready.Value;
	}
}
