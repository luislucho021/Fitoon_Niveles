using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public IconTienda iconTiendaPropio;

    public void TryToPurchaseBtn()
    {
        TiendaManager.Instance.ActivateConfirmPurchasePanel(iconTiendaPropio);
    }
}
