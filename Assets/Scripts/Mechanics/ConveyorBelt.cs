using UnityEngine;
public class ConveyorBelt : MonoBehaviour
{
    public float scrollSpeed = 1.0f; // Velocidad de desplazamiento de la textura
    public Material mat;

    private Material material;
    private float offset = 0;

    void Start()
    {
        foreach(Material matr in GetComponent<Renderer>().materials)
        {
            if (matr.name == (mat.name + " (Instance)")) material = matr;
        }
    }

    void Update()
    {
        // Calcula el desplazamiento en función del tiempo y la velocidad
        offset = Time.time * scrollSpeed;

        // Aplica el desplazamiento al material (ajusta según tus necesidades)
        material.mainTextureOffset = new Vector2(0, offset);
    }
}
