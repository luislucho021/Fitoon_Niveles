using AdultLink;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    //[SerializeField] private HealBar bar;
    public delegate void OnSelectedButtonEvent(bool selection);
    public static event OnSelectedButtonEvent onSelectedButtonEvent;

    Button lastButtonSelected;

    private void OnEnable()
    {
        HealBar.onClickedButtonEvent += UIButtonClicked;
    }

    private void OnDisable()
    {
        HealBar.onClickedButtonEvent -= UIButtonClicked;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colision");
        lastButtonSelected = other.GetComponent<Button>();

        if(onSelectedButtonEvent != null)
        {
            onSelectedButtonEvent(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("no colision");
        lastButtonSelected = null;
        if (onSelectedButtonEvent != null)
        {
            onSelectedButtonEvent(false);
        }
    }

    private void UIButtonClicked()
    {
        lastButtonSelected.onClick.Invoke();
    }

}
