using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaController : MonoBehaviour
{
    [SerializeField] private int alphaOnPress = 100; // Valor alfa al pulsar el botón (0-255)
    [SerializeField] private int alphaDefault = 0; // Valor alfa por defecto (0-255)

    private Button[] buttons; // Array de botones
    private Image[] images; // Array de imágenes correspondientes a los botones
    private int activeButtonIndex = 0; // Índice del botón actualmente activo

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>();

        images = new Image[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            images[i] = buttons[i].GetComponent<Image>();
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Captura el índice actual
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }

        // El primer botón empieza activo
        SetImageAlpha(images[1], alphaOnPress);
        activeButtonIndex = 0;

        for (int i = 1; i < images.Length; i++)
        {
            SetImageAlpha(images[i], alphaDefault);
        }
    }

    private void OnButtonPressed(int index)
    {
        // Restablece el alfa del botón activo
        SetImageAlpha(images[activeButtonIndex], alphaDefault);

        // Establece el alfa del nuevo botón activo
        SetImageAlpha(images[index], alphaOnPress);
        activeButtonIndex = index; // Actualiza el índice del botón activo
    }

    private void SetImageAlpha(Image image, int alpha)
    {
        // Normaliza el valor de alfa de 0-255 a 0-1
        float normalizedAlpha = Mathf.Clamp(alpha, 0, 255) / 255f;

        Color color = image.color;
        color.a = normalizedAlpha;
        image.color = color;
    }
}
