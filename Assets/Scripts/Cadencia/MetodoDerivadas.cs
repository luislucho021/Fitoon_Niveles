using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MetodoDerivadas : MonoBehaviour
{
    public List<float> data;
    string rutaPrueba = "Assets/Scripts/NoNetwork/Cadencia/datosPrueba.json";

    List<int> picos = new List<int>();
    float frecuencia;
    float segundosMuestra;
    float distanciaPaso = 0.67f; //aproximado en metros
    float velocidad;

    bool countingFrames = false;

    private void Start()
    {
        // Lee el archivo JSON y carga los datos en una cadena
        string jsonString = File.ReadAllText(rutaPrueba);

        // Convierte la cadena JSON en una lista de tipo float
        DatosJSON datosJSON = JsonUtility.FromJson<DatosJSON>(jsonString);
        data = new List<float>(datosJSON.data);

        // Determinar segundos de muestra.La camara es 30fps
        segundosMuestra = data.Count / 30;

        //Calculo del coste
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        picos = FindPeaks();
        Debug.Log("Total: " + picos.Count + " picos.");
        CalcularFrecuenciaTotal();

        stopwatch.Stop();
        Debug.Log("Coste del algoritmo: " + stopwatch.ElapsedMilliseconds + " ms");

        

        
    }

    // Método para calcular los picos
    public List<int> FindPeaks()
    {
        int peaksFound = 0;
        int indexFirstPeak = 0;
        

        List<int> peaks = new List<int>();

        // Calcular la primera y segunda derivada
        List<float> firstDerivative = CalculateFirstDerivative();
        List<float> secondDerivative = CalculateSecondDerivative(firstDerivative);

        // Buscar picos en la primera derivada (máximos)
        for (int i = 1; i < firstDerivative.Count - 1; i++)
        {
            // Revisar si la primera derivada cruza por debajo de cero
            if (firstDerivative[i] >= 0 && firstDerivative[i - 1] < 0)
            {
                // Revisar si el máximo en la primera derivada es un punto de inflexión
                // Comparando con la segunda derivada
                if (secondDerivative[i] < 0)
                {
                    // Añadir el índice del máximo a la lista de picos
                    peaks.Add(i);

                    
                    if(!countingFrames)
                    {
                        indexFirstPeak = i;
                        countingFrames = true;
                        Debug.Log("Primer pico en frame " + i);
                    }
                    peaksFound++;
                    if(peaksFound == 8)
                    {
                        Debug.Log("Pico 8 en frame " + i);
                        CalcularFrecuenciaEntrePicos(i-indexFirstPeak);
                        peaksFound = 0;
                    }
                }
            }
        }

        return peaks;
    }

    // Método para calcular la primera derivada utilizando una diferencia finita centrada
    private List<float> CalculateFirstDerivative()
    {
        List<float> result = new List<float>();

        for (int i = 1; i < data.Count - 1; i++)
        {
            float derivative = (data[i + 1] - data[i - 1]) / 2f;
            result.Add(derivative);
        }

        return result;
    }

    // Método para calcular la segunda derivada utilizando una diferencia finita centrada
    private List<float> CalculateSecondDerivative(List<float> firstDerivative)
    {
        List<float> result = new List<float>();

        for (int i = 1; i < firstDerivative.Count - 1; i++)
        {
            float derivative = firstDerivative[i + 1] - 2 * firstDerivative[i] + firstDerivative[i - 1];
            result.Add(derivative);
        }

        return result;
    }

    private void CalcularFrecuenciaTotal()
    {
        Debug.Log("----FRECUENCIA TOTAL FINAL: METODO DERIVADAS-----");
        //Determinar frecuencia de pasos. 1 paso = 1 pico. 
        frecuencia = picos.Count / segundosMuestra;
        Debug.Log("Frecuencia: " + frecuencia + " pasos por segundo");

        //Determinar velocidad. Pasos/segundo -> Metros/segundo
        velocidad = distanciaPaso * frecuencia;
        Debug.Log("Velocidad media: " + velocidad + "m/s.");
    }

    private void CalcularFrecuenciaEntrePicos(int frames)
    {
        Debug.Log("----FRECUENCIA: METODO FRAMES ENTRE CADA 8 PICOS-----");
        //Frames son los frames desde el primer pico hasta el pico 8
        float time = frames / 30f;
        frecuencia = 8 / time;
        Debug.Log("Frecuencia contando 8 picos: " + frecuencia + " pasos por segundo");

        //Determinar velocidad. Pasos/segundo -> Metros/segundo
        velocidad = distanciaPaso * frecuencia;
        Debug.Log("Velocidad media: " + velocidad + " m/s.");

        countingFrames = false;
    }
}

// Clase auxiliar para almacenar los datos JSON
[System.Serializable]
public class DatosJSON
{
    public float[] data;
}
