using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    public GameObject exitButton;

    private bool isFinished = false;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            //Cursor.visible = true;
            //exitButton.SetActive(true);
            anim.SetTrigger("Reset");
            anim.enabled = false;
            isFinished = false;
        }
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    public void Finish()
    {
        isFinished = true;
        anim.enabled = true;
    }
}
