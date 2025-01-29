using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class VelocityCalculator : MonoBehaviour
{
    public GraphRenderer graphRenderer; // Referencia al objeto GraphRenderer
    public int velocitySampleSize = 50; // Tamaño de la muestra de velocidades para el promedio
    
    public float velocityAverage;

    public float velocityScale = 2.8f; 

    public float minVelocity = 0f; // Valor mínimo de velocidad
    public float maxVelocity = 18f; // Valor máximo de velocidad

    
    private float[] velocityTable = { 0f, 2f, 4f, 6f, 8f, 10f, 14f, 16f }; // Tabla de referencia con los valores de 0 a 16
    public float adjustedVelocity;

    private float currentClosestValue;
    private bool exceededThreshold;    

    private void Start()
    {
        StartCoroutine(AdjustVelocity());
    }
    private void Update()
    {
        UpdateVelocityAverage();
    }

     

    #region VELOCIDAD 
    private void UpdateVelocityAverage()
    {
        int count = graphRenderer.graphPoints.Count;
        if (count < 2)
        {
            velocityAverage = 0f; // No hay suficientes puntos para calcular la velocidad
            return;
        }

        // Obtener los últimos puntos en el eje Y para el cálculo del promedio
        int startIndex = Mathf.Max(0, count - velocitySampleSize);
        int endIndex = count - 1;

        float totalVelocity = 0f;
        float deltaTime = Time.deltaTime;

        for (int i = startIndex; i < endIndex; i++)
        {
            float currentY = graphRenderer.graphPoints[i + 1].y;
            float previousY = graphRenderer.graphPoints[i].y;

            float deltaY = currentY - previousY;
            float velocity = deltaY / deltaTime;

            totalVelocity += velocity;
        }



        // Calcular el promedio de velocidades
        velocityAverage = Mathf.Abs(totalVelocity / velocitySampleSize);
        
        // Ajustar la escala de la velocidad
        velocityAverage *= velocityScale;

        // Limitar la velocidad dentro del rango deseado
        velocityAverage = Mathf.Clamp(velocityAverage, minVelocity, maxVelocity);

    }
    #endregion

    #region VELOCIDAD MÁQUINA
    //AJUSTAR PARA QUE LOS VALORES ESTÉN ENTRE 0/ 16
    private IEnumerator AdjustVelocity()
    {
        while (true)
        {
            float multipliedVelocity = velocityAverage * 14.4f; //cuatrovece 3.6 ~14.4
            float closestValue = FindClosestValue(multipliedVelocity, velocityTable);

            // Comprueba si el valor más cercano supera el valor actual
            if (closestValue > currentClosestValue)
            {
                exceededThreshold = true;
            }
            // Comprueba si el valor más cercano es menor que el valor actual
            else if (closestValue < currentClosestValue)
            {
                exceededThreshold = false;
                currentClosestValue = closestValue;
            }

            // Si se ha superado el umbral y el valor más cercano sigue siendo mayor, selecciona el siguiente valor en la tabla
            if (exceededThreshold && closestValue > currentClosestValue)
            {
                
                int currentIndex = Array.IndexOf(velocityTable, currentClosestValue);
                currentClosestValue = velocityTable[currentIndex+1];
            }
           
            adjustedVelocity = currentClosestValue / 14.4f;
            yield return new WaitForSeconds(2f);

        }
    }

    private float FindClosestValue(float targetValue, float[] referenceTable)
    {
        int minIndex = 0;
        int maxIndex = referenceTable.Length - 1;

        if (targetValue <= referenceTable[minIndex])
            return referenceTable[minIndex];
        if (targetValue >= referenceTable[maxIndex])
            return referenceTable[maxIndex];

        while (minIndex <= maxIndex)
        {
            int midIndex = (minIndex + maxIndex) / 2;
            float midValue = referenceTable[midIndex];

            if (midValue == targetValue)
                return midValue;

            if (midValue < targetValue)
                minIndex = midIndex + 1;
            else
                maxIndex = midIndex - 1;
        }

        float closestValue = referenceTable[minIndex];//OLD


        // Comprobar si el valor más cercano está fuera de los límites de la tabla OLD
        if (Mathf.Abs(targetValue - closestValue) > Mathf.Abs(targetValue - referenceTable[maxIndex]))
          closestValue = referenceTable[maxIndex];
 
        return closestValue;
    }
    #endregion


    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 74;
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(100, 200, 500, 200), "VEL: " + velocityAverage.ToString("F3"), style);
       GUI.Label(new Rect(100, 50, 500, 200), "AJUSTADA: " + adjustedVelocity.ToString("F2"), style);

    }
}



