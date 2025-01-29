using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Cadencia : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    private bool detectado = false;
    private float verticalPosition;
    private float updatedYPos;
    [SerializeField] TextMeshProUGUI yInitialText;
    [SerializeField] TextMeshProUGUI yUpdatedPositionText;
    

    private float distanciaPaso; //en metros
    public string sexo; //H o M

    [SerializeField] private float segundosMuestra;
    [SerializeField] private float cooldownTime = 0.25f;
    private float cooldownTimer = 0;

    private float timer = 0;
    private int pasos = 0;
    private int pasosTotales = 0;
    private float frecuencia;
    private float velocidad; //metros por segundo
    [SerializeField] TextMeshProUGUI pasosText;

    private void OnEnable()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        faceManager.facesChanged += CaraDetectada;
        CalcularDistanciaPaso();
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }

    private void Update()
    {
        if (detectado)
        {
            CalcularFrecuencia();
        }
    }

    void CalcularDistanciaPaso()
    {
        //Según diversos estudios estadísticos contrastados, la longitud media del paso para las mujeres adultas es de 67 cm, y la de los hombres, de 76,2 cm.
        if (sexo == "M")
        {
            distanciaPaso = 0.67f;
        }
        else if(sexo == "H")
        {
            distanciaPaso = 0.762f;
        }
    }

    void CalcularFrecuencia()
    {
        updatedYPos = (float)Math.Round(face.transform.position.y, 2, MidpointRounding.AwayFromZero);
        yUpdatedPositionText.text = "y = " + updatedYPos.ToString();

        if (cooldownTimer >= cooldownTime)
        {
            if (Mathf.Approximately(updatedYPos, verticalPosition))
            {
                Debug.Log("Paso " + pasos);
                pasos++;
                pasosTotales++;
                pasosText.text = pasosTotales.ToString();
                cooldownTimer = 0;
            }
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
 

        timer += Time.deltaTime;

        if(timer >= segundosMuestra)
        { 
            frecuencia = pasos / segundosMuestra;

            //Transformar pasos/segundos en metros/segundo
            velocidad = distanciaPaso * frecuencia;

            Debug.Log("Velocidad: " + velocidad + " m/s");

            timer = 0;
            pasos = 0;
        }

    }



    //-----------EVENTS---------
    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            verticalPosition = (float)Math.Round(face.transform.position.y, 2, MidpointRounding.AwayFromZero);
            yInitialText.text = "inicial = " + verticalPosition.ToString();
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            detectado = false;
        }
    }
}
