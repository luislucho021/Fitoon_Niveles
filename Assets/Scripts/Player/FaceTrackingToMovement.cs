using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;

public class FaceTrackingToMovement : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    Animator animator;
    private GoalController goal;
    private bool detectado = false;
    private FadeCamera fadeCamera;

    [Header("Rotation")]
    private Rigidbody rb;
    [SerializeField] int rotationIntensity = 2;

    [Header("Velocity")]
    [SerializeField] TextMeshProUGUI velocityText;
    [SerializeField] TextMeshProUGUI cadenceText;
    [SerializeField] TextMeshProUGUI animCadenceText;
    List<float> data;
    public float sampleRate; //frames para calcular la velocidad
    public float distanciaPaso;
    DominantFrequencyCounter frequencyCounter;
    MoveThePlayer playerMov;
    float velocidad;

    int pasosAnimacion;
    float timer;
    bool activarTimer = false;
    

    //EVENTOS (Para el movimiento del personaje)
    public delegate void OnCaraDetectada();
    public static event OnCaraDetectada onCaraDetectadaEvent;

    public delegate void OnCaraNoDetectada();
    public static event OnCaraNoDetectada onCaraNoDetectadaEvent;


    private void OnEnable()
    {
        pasosAnimacion = 0;
        faceManager = FindFirstObjectByType<ARFaceManager>();
        fadeCamera = FindFirstObjectByType<FadeCamera>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        frequencyCounter = FindFirstObjectByType<DominantFrequencyCounter>();
        playerMov = GetComponent<MoveThePlayer>();

        faceManager.facesChanged += CaraDetectada;
        if(fadeCamera != null) fadeCamera.StartFade(true);

        goal = FindObjectOfType<GoalController>();
        goal.AddPlayerToList(transform);

        data = new List<float>();

        CuentaPasos.onPasoEvent += ContarPaso;

    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
        CuentaPasos.onPasoEvent -= ContarPaso;
    }


    //----------AR ROTATION--------------------
    void Update()
    {
        if (detectado)
        {
            RotateCharacter(face.transform);
            CalculateVelocity(face.transform);
        }

        if(!playerMov.testing) velocityText.text = $"Velocity: {Math.Round(velocidad, 2, MidpointRounding.AwayFromZero)} ({Math.Round(velocidad * 3.6, 2, MidpointRounding.AwayFromZero)} km/h)";
        
        if (activarTimer) timer += Time.deltaTime;
    }

    private void CalculateVelocity(Transform faceData)
    {
        //Cuando pasen X frames, mandar esa lista a la FFT y sacar la frecuencia
        if(data.Count == sampleRate)
        {

            if(animator == null)
            {
                Debug.Log("animator is null!");
                animator = GetComponentInChildren<Animator>();
            }
                float frecuencia = frequencyCounter.DoFFT(data.ToArray());
                //Calcular la velocidad. Pasos/segundo -> Metros/segundo
                velocidad = distanciaPaso * frecuencia;
                Debug.Log($"Velocidad media: {velocidad} m/s.");

                float cadencia = frecuencia * 60f;
                //La animacion base a velocidad 1 tiene una cadencia de aprox 136.
                float animationSpeed = cadencia / 136;

                animator.SetFloat("playerSpeed", animationSpeed);

                cadenceText.text = $"Cadence: {cadencia}";

            playerMov.moveSpeed = velocidad * 3f;
            
            //Reiniciar lista
            data.Clear();
        }
        else
        {
            //Guardar en una lista el input del face.transform.y
            data.Add(faceData.position.y);
        }

    }

    // Calcular el rango entre el valor mínimo y máximo de una lista
    float CalculateRange(List<float> positions)
    {
        if (positions.Count == 0)
        {
            return 0;
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (float pos in positions)
        {
            if (pos < min)
            {
                min = pos;
            }

            if (pos > max)
            {
                max = pos;
            }
        }

        float range = max - min;
        return range;
    }

    private void RotateCharacter(Transform otherObject)
    {
        // Face forward rotation
        float angle = GetObjectRotation(otherObject);

        // Rotation with intensity multiplier
        float rotationWithMult = angle * rotationIntensity;

        // Clamp bewteen -90 and 90
        float clampedRotation = Mathf.Clamp(rotationWithMult, -90f, 90f);

        // Rotate the character
        Quaternion desiredRotation = Quaternion.Euler(0f, clampedRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 1f);
    }

    private float GetObjectRotation(Transform obj)
    {
        if (obj.transform.eulerAngles.y > 180)
        {
            return obj.transform.eulerAngles.y - 360;
        }
        else
        {
            return obj.transform.eulerAngles.y;
        }
    }
    

    void ContarPaso()
    {       
        activarTimer = true;
        pasosAnimacion++;
        if(timer >= 15)
        {
            Debug.Log($"Cadencia personaje: {pasosAnimacion*4}");
            timer = 0;
            pasosAnimacion = 0;
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
            Debug.Log("CARA DETECTADA!");

            if (onCaraDetectadaEvent != null)
            {
                onCaraDetectadaEvent();
            }
            if (fadeCamera != null) fadeCamera.StartFade(false);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            Debug.Log("Cara quitada");
            detectado = false;

            if(onCaraNoDetectadaEvent != null)
            {
                onCaraNoDetectadaEvent();
            }
            if (fadeCamera != null) fadeCamera.StartFade(true);
        }
    }  
}
