using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public string effectName = "Slowness";
    public float effectDuration = 5f;
    public float recoverDuration = 5f;
    public float effectValue = 0.5f;

    internal void applyEffect(GameObject player)
    {
        PlayerController pC = player.GetComponent<PlayerController>();
        BotController bC = player.GetComponent<BotController>();

        //Is a Player
        if (pC != null)
        {
            pC.SetBoost(effectValue);
        }

        //Is a Bot
        if (bC != null)
        {
            bC.SetBoost(effectValue);
        }

        Debug.Log(player.name + " has been affected with " + effectName + " effect.");
    }

    internal void removeEffect(GameObject player)
    {
        PlayerController pC = player.GetComponent<PlayerController>();
        BotController bC = player.GetComponent<BotController>();

        //Is a Player
        if (pC != null)
        {
            pC.SetBoost(1);
        }

        //Is a Bot
        if (bC != null)
        {
            bC.SetBoost(1);
        }

        Debug.Log(player.name + " has been recovered from the " + effectName + " effect.");
    }
}