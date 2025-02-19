using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrivateMatchStarter : MonoBehaviour
{
	[SerializeField] GameObject inputMenu;
    [SerializeField] TMP_InputField inputField;
    public void OpenMenu()
    {
        inputMenu.SetActive(true);
    }
    public void CloseMenu()
    {
        inputMenu.SetActive(false);
    }
    public void StartGame()
    {
        if (inputField.text == "")
            return;

        DiscoveryHandler.Passcode = inputField.text;
        SceneManager.LoadScene("LobbyScene");
    }
}
