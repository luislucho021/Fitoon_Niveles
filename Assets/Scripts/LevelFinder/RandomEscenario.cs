using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEscenario : MonoBehaviour
{
    SaveData saveData;
    [SerializeField] List<EscenarioItem> escenarios;
    [SerializeField] List<EscenarioItem> escenariosDisponibles;
    [SerializeField] GameObject container;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] int countdown;
    Image image;
    TextMeshProUGUI text;
    EscenarioItem escenarioElegido;

    public float secondsToChange;
    public float totalSeconds;
    float timer = 0;
    bool timerActive = true;

    void Start()
    {
        saveData = FindAnyObjectByType<SaveData>();
        image = container.GetComponentInChildren<Image>();
        text = container.GetComponentInChildren<TextMeshProUGUI>();
        escenariosDisponibles = new List<EscenarioItem>(escenarios);
        GetAvaliableScenarios();
        StartCoroutine(CambiarImagenAleatoria());
    }

    private void Update()
    {
        if(timerActive) timer += Time.deltaTime;
    }

    IEnumerator CambiarImagenAleatoria()
    {
        while (timer < totalSeconds) 
        {
            yield return new WaitForSeconds(secondsToChange);

            escenarioElegido = escenariosDisponibles[Random.Range(0, escenariosDisponibles.Count)];
            image.sprite = escenarioElegido.imagenEscenario;
            text.text = escenarioElegido.nombreEscenario;
        }

        //timer = 0;
        saveData.player.scenesPlayed.Add(escenarioElegido);
        saveData.SaveToJson();

        timerActive = false;
        countdownText.text = countdown.ToString();
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {       
        while (countdown != 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
            if (countdown == 0)
            {
                GetComponent<ButtonFunctions>().LoadScene(escenarioElegido.nombreEscenario);
            }
            else
            {
                countdownText.text = countdown.ToString();
            }
        }
    }

    private void GetAvaliableScenarios()
    {
        saveData.ReadFromJson();
        foreach (EscenarioItem item in saveData.player.scenesPlayed)
        {
            escenariosDisponibles.Remove(item);
        }
    }
}
