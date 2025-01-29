using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TiendaManager : MonoBehaviour
{
    public static TiendaManager Instance { get; private set; }
    SaveData saveData;
    GameObject tienda;
    [SerializeField] TextMeshProUGUI shopCoins;
    GameObject panel;
    GameObject lockBackground;
    [SerializeField] IconTienda itemSelected;

    private void Awake()
    {
        //Como Soingleton para acceder desde prefab
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        saveData = FindAnyObjectByType<SaveData>();
        panel = GameObject.Find("ConfirmPurchase");
        lockBackground = GameObject.Find("LockBackground");
        panel.SetActive(false);
        lockBackground.SetActive(false);
        shopCoins.text = saveData.player.normalCoins.ToString();
    }

    public void PurchaseItem()
    {
        //Add item to purchased
        switch (itemSelected.itemType)
        {
            case ItemType.SKIN:
                saveData.player.purchasedSkins.Add(itemSelected.itemID);
                tienda = GameObject.Find("ContentSkins");
                break;
            case ItemType.SHOE:
                saveData.player.purchasedShoes.Add(itemSelected.itemID);
                tienda = GameObject.Find("ContentShoes");
                break;
            case ItemType.COLOR:
                saveData.player.purchasedColors.Add(itemSelected.itemID);
                tienda = GameObject.Find("ContentColors");
                break;
        }

        //Update Money
        saveData.player.normalCoins -= itemSelected.itemPrice;
        shopCoins.text = saveData.player.normalCoins.ToString();

        //Save
        saveData.SaveToJson();

        //Close panel
        panel.SetActive(false);
        lockBackground.SetActive(false);

        //Change the item to purchased aspect
        tienda.transform.GetChild(itemSelected.itemID).GetChild(0).gameObject.SetActive(false);
        tienda.transform.GetChild(itemSelected.itemID).GetComponent<Image>().color = new Color(221f / 255f, 255f / 255f, 90f / 255f);
    }

    public void ActivateConfirmPurchasePanel(IconTienda item)
    {
        panel.SetActive(true);
        lockBackground.SetActive(true);
        itemSelected = item;
        //Read
        saveData.ReadFromJson();

        if (saveData.player.normalCoins >= item.itemPrice)
        {
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "Do you want to buy this item?";
            panel.transform.GetChild(1).gameObject.SetActive(true);
            panel.transform.GetChild(2).gameObject.SetActive(true);
            panel.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "You don't have enough money";
            panel.transform.GetChild(1).gameObject.SetActive(false);
            panel.transform.GetChild(2).gameObject.SetActive(false);
            panel.transform.GetChild(3).gameObject.SetActive(true);
        }
    }
}
