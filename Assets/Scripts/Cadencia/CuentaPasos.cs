using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuentaPasos : MonoBehaviour
{
    //Cada paso de la animacion lanza el evento Paso, pero solo puede ser leido desde un script en el mismo GameObject que el Animator
    //Por esto, este evento a su vez lanza otro evento para ser leido por los demas scripts

    public delegate void OnPasoEvent();
    public static event OnPasoEvent onPasoEvent;

    public void PasoEvent()
    {
        if(onPasoEvent != null)
        {
            onPasoEvent();
        }
    }
}
