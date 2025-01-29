using UnityEngine;
using System.IO;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;
using TMPro;
public class WriteDataToCSV : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    private bool detectado = false;
   // private StreamWriter writer;
    // Ruta donde guardaremos el archivo CSV
    string filePath;

    private float2 posFiltrada;

    private OneEuroFilter filter = new OneEuroFilter();
    public float betaValue{get; set;}
    public float mincutOffValue { get; set; }
    [SerializeField] TextMeshProUGUI betaText;
    [SerializeField] TextMeshProUGUI cutoffText;


    private void OnEnable()
    {
        posFiltrada = new float2();
        betaValue = 0.01f;
        mincutOffValue = 5f;


        faceManager = FindFirstObjectByType<ARFaceManager>();
        filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "FaceData.csv";
        faceManager.facesChanged += CaraDetectada;
        // Verifica si el archivo ya existe y lo elimina si es así
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine("Posicion X;Posicion Y;Posicion X f; Posicion Y f");
        }
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }

    void Update()
    {
        if (detectado)
        {
            FiltrarDatos();
            EscribirCSV();
        }

        filter.Beta = betaValue;
        filter.MinCutoff = mincutOffValue;
        betaText.text = filter.Beta.ToString();
        cutoffText.text = filter.MinCutoff.ToString();
    }

    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("Datos del transform guardandose en " + filePath);
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            detectado = false;
        }
    }
    private void FiltrarDatos()
    {
        float2 posicion = new float2(face.transform.position.x, face.transform.position.y);
        posFiltrada = filter.Step(Time.time, posicion);
    }
    void EscribirCSV()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Escribimos cada dato en una línea separada
            writer.WriteLine(string.Format("{0};{1};{2};{3}", face.transform.position.x, face.transform.position.y, posFiltrada.x, posFiltrada.y));
        }
    }
}
