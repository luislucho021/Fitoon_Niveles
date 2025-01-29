using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FaceTest : MonoBehaviour
{       
    GameObject face = null;         //Este es el objeto careta que genera el AR cuando detecta una cara
    public Camera cam;              //Esta es la camara que sigue al personaje
    
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    //VARIABLES SIN PUNTERO
    float rotPlayer;            //Es la rotacion que se le aplica al personaje
    float turnSpeed = 60f;      //Velocidad de giro del personaje

    
    public VelocityCalculator velocityCalculator;
    float runSpeed;
    float velocity;//Velocidad del personaje

    float kmh;

    //ELEMENTOS DEL CANVAS
    public Text textoDetectFace;
    public Text textoInfo;
    public Text textoSuavizado;


    #region GAMELOOP
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Obtener la velocidad promedio del VelocityCalculator
        velocity = velocityCalculator.velocityAverage;
        kmh = velocityCalculator.adjustedVelocity * 14.4f; //3.6 es para pasar de m/s a km/h 
        // Obtener la velocidad de movimiento en función de la velocidad promedio
        runSpeed = velocity;

    }

    void FixedUpdate()
    {
        face = GameObject.FindGameObjectWithTag("Face");

        if (face == null)              //Si no detecta la cara
        {
            m_Animator.SetBool("DetFace", false);
            //textoDetectFace.text = "No encuentra cara";
        }
        else                           //Si detecta la cara
        {
            m_Animator.SetBool("DetFace", true);
            //textoDetectFace.text = "Encuentra cara";

            MovePlayer(face.transform.localRotation.y);

        }
    }


    private void OnTriggerEnter(Collider other)
        {
            SceneManager.LoadScene("Adjust Face");   //Vuelvo al menu. Cambio a la escena "Adjust Face" cuando llega al final del mapa
        }
    #endregion

    #region PLAYER MOVEMENT METHODS
    void MovePlayer(float rotation)
    {

        textoSuavizado.text = "Suavizado activo";

        //'rotation' es la rotación de la cabeza y 'rotPlayer' es la rotacóon del personaje.
        //Modifico la rotación del personaje cuando la diferencia entre su rotación actual y la rotación de la cabeza es mayor que 0,07.
        //Entoces cuando la cabeza hace moviemntos de rotación muy pequeóos, estos no se aplican a la rotación del personaje.
        if (Math.Abs(rotation - rotPlayer) > 0.05)
        {
            if (rotPlayer < rotation)
            {
                rotPlayer += 0.008f;
            }
            else if (rotPlayer > rotation)
            {
                rotPlayer -= 0.008f;
            }
        }

        m_Movement.Set(rotPlayer * 6f, 0f, 1f);
        m_Movement.Normalize();
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * runSpeed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
    #endregion


    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 84;
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(100, 100, 500, 200), "runSpeed: " + runSpeed.ToString("F2"), style);        
        GUI.Label(new Rect(100, 250, 500, 200), "kmh: " + kmh.ToString("F0"), style);
    }

}
