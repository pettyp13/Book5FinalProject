using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayScript : MonoBehaviour
{

    public void LeaveShop()
    {

        List<Item> purchasedItems = new List<Item>();
        purchasedItems = GameObject.Find("shopSystem").GetComponent<ShopSystem>().shopItems;
        int moneyLeft = GameObject.Find("shopSystem").GetComponent<ShopSystem>().moneyLeft;
        GameObject.Find("Player").GetComponent<InventorySystem>().SetMoney(moneyLeft);

        GameObject.Find("Player").GetComponent<InventorySystem>().AddPurchasedItems(purchasedItems);
        GameObject.Find("Player").GetComponent<ControlPlayer>().shopIsDisplayed = false;
        GameObject.Find("Player").transform.position = GameObject.Find("Player").transform.forward * 4;
        GameObject.Find("shopUI").SetActive(false);


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
