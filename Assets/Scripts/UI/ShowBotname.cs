using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowBotname : MonoBehaviour
{
    [SerializeField] TextMeshPro botText;

    private void Start()
    {
        botText.text = transform.parent.name;
    }
}
