using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tienda : MonoBehaviour
{
    SaveData saveData;
    public List<IconTienda> itemsByCategory;
    public GameObject iconPrefab;
    public GameObject gameManager;
    public GameObject container;
    ScrollRect scrollRect;
    RectTransform item;
    void Start()
    {
        saveData = FindAnyObjectByType<SaveData>();
        scrollRect = FindObjectOfType<ScrollRect>();
        item = container.GetComponent<RectTransform>();
        CleanShop();
        CreateShop();
    }

    private void CleanShop()
    {
        for(int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }
    private void CreateShop()
    {
        saveData.ReadFromJson();
        for (int i = 0; i < itemsByCategory.Count; i++)
        {
            GameObject iconoCreado = Instantiate(iconPrefab, container.transform);
            IconTienda iconTiendaActual = itemsByCategory[i];
            //Primer hijo: Boton. Hijos (en orden): Texto monedas y Icono monedas
            iconoCreado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = iconTiendaActual.itemPrice.ToString();
            iconoCreado.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = iconTiendaActual.coinType;
            //segundo hijo: icono 3D
            Transform iconTransform = iconoCreado.transform.GetChild(1).GetChild(0).transform;
            //destruir hijo que ya existe
            Destroy(iconoCreado.transform.GetChild(1).GetChild(0).gameObject);
            Instantiate(iconTiendaActual.objectIcon, iconTransform.position, iconTransform.rotation, iconoCreado.transform.GetChild(1));
            //tercer hijo: nombre
            iconoCreado.transform.GetChild(2).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = iconTiendaActual.itemName;
            //añadir id
            iconTiendaActual.itemID = i;
            iconoCreado.GetComponent<ShopItem>().iconTiendaPropio = iconTiendaActual;

            if ((saveData.player.purchasedSkins.Contains(i) && iconTiendaActual.itemType.ToString() == "SKIN") || 
                (saveData.player.purchasedShoes.Contains(i) && iconTiendaActual.itemType.ToString() == "SHOE") || 
                (saveData.player.purchasedColors.Contains(i) && iconTiendaActual.itemType.ToString() == "COLOR"))
            {
                //Change the item to purchased aspect
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(i).GetComponent<Image>().color = new Color(221f / 255f, 255f / 255f, 90f / 255f);
            }
        }
    }

    public void ScrollUntilItem()
    {
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemToLeftCoroutine(scrollRect, item, 0.5f));
    }

}
