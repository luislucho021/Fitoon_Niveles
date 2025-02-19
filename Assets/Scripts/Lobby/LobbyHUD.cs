using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHUD : NetworkBehaviour
{
    readonly SyncVar<byte> numberOfPlayers = new SyncVar<byte>();
	[SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] TextMeshProUGUI countDownText;
    public void AddPlayer()
    {
        numberOfPlayers.Value++;
    }

    public void RemovePlayer()
    {
        numberOfPlayers.Value--;
    }

    public override void OnStartNetwork()
    {
        numberOfPlayers.OnChange += SetPlayerNumber;
    }
    [ObserversRpc]
    public void ChangeCountdownText(string s)
    {
		countDownText.text = s;

	}

	public void SetPlayerNumber(byte prev, byte next, bool asServer)
	{
		playerCountText.text = next.ToString() + "/32";
	}
}
