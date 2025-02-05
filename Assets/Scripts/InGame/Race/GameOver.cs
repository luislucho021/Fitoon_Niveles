using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameOver : MonoBehaviour
{
    //Debe de ejecutarse este script cuando el jugador acaba la partida por completo, ganando (1 puesto) o perdiendo

    //Primer puesto: Medalla de oro
    //Puesto 20% o superior: Medalla de plata
    //Puesto 50% o superior: Medalla de bronce 
    //Puesto menor al 50%: Sin medalla (loser points) -> Perder en la primera carrera


    [SerializeField] int goldPoints, silverPoints, bronzePoints, loserPoints;
    [SerializeField] int goldMoney, silverMoney, bronzeMoney;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] TextMeshProUGUI moneyText;
    SaveData saveData;
    private void Awake()
    {
        saveData = GetComponent<SaveData>();
    }
    void Start()
    {
        saveData.ReadFromJson();
        CalculateReward();
        RaceManager.Instance.Reset();
        saveData.SaveToJson();
    }

    private void CalculateReward()
    {
        if (RaceManager.Instance.numberOfRace == 3)
        {
            if (RaceManager.Instance.playerWon)
            {
                //Primer puesto
                saveData.player.points += goldPoints;
                saveData.player.normalCoins += goldMoney;
                rewardText.text = $"#1 position!\nYou won {goldPoints} points!";
                moneyText.text = $"+{goldMoney}";
            }
            else
            {
                saveData.player.points += silverPoints;
                saveData.player.normalCoins += silverMoney;
                rewardText.text = $"You won {silverPoints} points!";
                moneyText.text = $"+{silverMoney}";
            }

        }
        else if (RaceManager.Instance.numberOfRace == 2)
        {
            saveData.player.points += bronzePoints;
            saveData.player.normalCoins += bronzeMoney;
            rewardText.text = $"You won {bronzePoints} points!";
            moneyText.text = $"+{bronzeMoney}";
        }
        else if (RaceManager.Instance.numberOfRace == 1)
        {
            saveData.player.points += loserPoints;
            rewardText.text = $"You won {loserPoints} points!";
            moneyText.text = "+0";
        }
        else
        {
            Debug.Log("ERROR: No se pueden calcular los puntos finales");
        }
    }

    public void PlayAgain()
    {
        RaceManager.Instance.Reset();
        GetComponent<ButtonFunctions>().LoadScene("FindingScenario");
    }
}
