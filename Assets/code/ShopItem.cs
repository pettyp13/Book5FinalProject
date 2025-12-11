using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;   // 🔹 IMPORTANT: for TextMeshProUGUI

public class ShopItem : MonoBehaviour
{
    string name;
    int price, quantity;
    public int index;

    void Start()
    {
        quantity = 0;
        UpdateQuantityLabel();
    }

    public void IncreaseQuantity()
    {
        if (!canClick()) return;

        quantity++;
        UpdateQuantityLabel();
    }

    public void DecreaseQuantity()
    {
        quantity--;

        if (quantity < 0)
            quantity = 0;

        UpdateQuantityLabel();
    }

    void UpdateQuantityLabel()
{
    // Update the little quantity text under the item
    transform.Find("itemQty").GetComponent<TMP_Text>().text = " " + quantity;

    // Also tell the ShopSystem to recalculate TOTAL and MONEY LEFT
    GameObject shopSystemObj = GameObject.Find("shopSystem");
    if (shopSystemObj != null)
    {
        ShopSystem ss = shopSystemObj.GetComponent<ShopSystem>();
        if (ss != null)
        {
            ss.UpdateTotal(index, quantity);
        }
    }
}


    bool canClick()
{
    // Ask the ShopSystem if we are allowed to add one more of this item
    GameObject shopSystemObj = GameObject.Find("shopSystem");
    if (shopSystemObj == null) return true;   // fail-safe so it doesn't crash

    ShopSystem ss = shopSystemObj.GetComponent<ShopSystem>();
    if (ss == null) return true;

    // This uses the method already in your ShopSystem
    return ss.canAddItemsTocart(this.index);
}

}
