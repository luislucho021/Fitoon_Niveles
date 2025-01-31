using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class PunteroUI : MonoBehaviour
{
    ARFaceManager faceManager;
    ARFace face;
    public Camera ARCamera;
    LineRenderer line;
    bool detectado = false;

    public RectTransform canvasRect; // Referencia al RectTransform del Canvas
    public RectTransform pointer; // Referencia al RectTransform del punto
    public float sensibility;
  //  public TMP_InputField enter_sensibility;

  //  [SerializeField] TextMeshProUGUI canvasText;

    void Awake()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
        line = GetComponent<LineRenderer>();
        //Debug.Log($"Canvas min x: {canvasRect.rect.xMin}, max x: {canvasRect.rect.xMax}, min y: {canvasRect.rect.yMin}, max y: {canvasRect.rect.yMax})");
    }

    private void OnEnable()
    {
        faceManager.facesChanged += CaraDetectada;
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= CaraDetectada;
    }
    /*public void ChangeSensibility()
    {
        string sensText = enter_sensibility.text;
        float.TryParse(sensText, out sensibility);
    }*/

    private void Update()
    {
        if (detectado)
        {
            if (face != null)
            {
                MovePointer();
            }
        }

        else
        {
           pointer.localPosition = Vector2.zero;
        }
    }


    void MovePointer()
    {
        /*Vector2 screenPoint = Camera.main.WorldToScreenPoint(face.transform.forward);
        pointer.position = screenPoint;*/

        Vector3 midPoint = face.transform.position;
        Vector3 direction = -face.transform.forward;
        Ray ray = new Ray(midPoint, direction);
        float distance = Vector3.Distance(midPoint, ARCamera.transform.position);
        distance -= ARCamera.nearClipPlane;
        Vector3 lookAtWorldPos = ray.GetPoint(distance);

        line.SetPosition(0, midPoint);
        line.SetPosition(1, lookAtWorldPos);

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(lookAtWorldPos);
        // Clampleamos la posición dentro de los límites del canvas
        screenPoint.x = Mathf.Clamp(screenPoint.x, 0, canvasRect.rect.xMax*2);
        screenPoint.y = Mathf.Clamp(screenPoint.y, 0, canvasRect.rect.yMax*2);


        pointer.position = Vector3.Lerp(pointer.position, screenPoint, sensibility);
        //canvasText.text = $"Canvas min x: {canvasRect.rect.xMin}, max x: {canvasRect.rect.xMax}, min y: {canvasRect.rect.yMin}, max y: {canvasRect.rect.yMax}\nLocal position: {pointer.position}";


    }

    void CaraDetectada(ARFacesChangedEventArgs aRFacesChangedEventArgs)
    {
        //Si existe una cara en "updated" (lista de caras detectadas)
        if (aRFacesChangedEventArgs.updated != null && aRFacesChangedEventArgs.updated.Count > 0 && !detectado)
        {
            //Guardar el objeto de la cara
            face = aRFacesChangedEventArgs.updated[0];
            detectado = true;
            Debug.Log("detectada");
        }

        if (aRFacesChangedEventArgs.removed.Count > 0)
        {
            detectado = false;
            Debug.Log("no detectada");
        }
    }
}
