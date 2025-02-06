using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowUsername : MonoBehaviour
{
    [SerializeField] TextMeshPro userText;

    private void Start()
    {
        userText.text = SaveData.player.username;
    }
}
