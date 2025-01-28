using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverCinta : MonoBehaviour
{
    public float speed = 0.5f; // Velocidad de movimiento de la textura
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * speed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(0, -offset));
    }
}
