using System.Collections.Generic;
using UnityEngine;

public class GraphRenderer : MonoBehaviour
{
    public List<Vector2> graphPoints = new List<Vector2>();
    public float scale = 1f;
    public int updateFrequency = 3; // Número de frames entre cada actualización

    private int frameCount = 0;

    public InteractionScript trackableScript;
    const int maxLength = 10; // Valor máximo de la longitud de la lista

    private void FixedUpdate()
    {
        frameCount++;

        if (frameCount >= updateFrequency)
        {
            frameCount = 0;

            // Obtener el tiempo actual
            float currentTime = Time.time;
            //currentTime = currentTime * 0.5f;

            // Obtener el valor de Y (trackable.y)
            float yValue = trackableScript.trackable.y;

            // Actualizar la gráfica
            UpdateGraph(currentTime, yValue);

        }
    }
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.green;


        GUI.Label(new Rect(100, 400, 500, 200), "PosicionesY: " + trackableScript.trackable.y, style);
        GUI.Label(new Rect(100, 500, 500, 200), "Time: " + Time.time, style);

        float offsetX = Mathf.Max(0, graphPoints.Count - maxLength);


        // Dibuja los puntos de la gráfica
        for (float i = 0; i < graphPoints.Count; i += 0.02f)
        {
            GUIStyle style2 = new GUIStyle();
            style2.fontSize = 78;
            style2.normal.textColor = Color.blue;

            //float x = graphPoints[i].x; // Obtener la coordenada X del GraphPoint
            //float y = graphPoints[i].y; // Obtener la coordenada Y del GraphPoint

            int index = Mathf.FloorToInt(i); // Índice entero inferior
            float t = i - index; // Valor fraccional entre los puntos


            // Obtener los dos puntos de la gráfica entre los que se interpolan los valores
            Vector2 point1 = graphPoints[index];
            Vector2 point2 = graphPoints[index + 1];

            // Calcular el valor interpolado entre los dos puntos
            float x = Mathf.Lerp(point1.x, point2.x, t);
            float y = Mathf.Lerp(point1.y, point2.y, t);

            // Calcular la posición en la pantalla a partir de las coordenadas
            float screenX = 320 + (i - offsetX) * 280f;
            float screenY = 600 + y * 4500f;

            GUI.Label(new Rect(screenX, screenY, 500, 400), "·", style2);
        }

    }

    private void UpdateGraph(float x, float y)
    {
        Vector2 point = new Vector2(x, y);
        graphPoints.Add(point);


        // Verificar si se ha excedido el límite de longitud
        if (graphPoints.Count >= maxLength)
        {
            // Eliminar el primer valor de la lista
            graphPoints.RemoveAt(0);

        }
    }
}
