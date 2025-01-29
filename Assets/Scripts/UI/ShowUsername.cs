using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowUsername : MonoBehaviour
{
    SaveData save;
    [SerializeField] TextMeshPro userText;

    private void Start()
    {
        save = FindAnyObjectByType<SaveData>();
        userText.text = save.player.username;
    }
}
