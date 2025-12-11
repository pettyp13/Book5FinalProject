using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{

    List<Item> playerInventory;
    int currentInventoryIndex = 0;
    bool isVisible = false;
    public GameObject inventoryText;
    public GameObject inventoryImage;
    public GameObject inventoryDescription;
    public GameObject inventoryPanel;

    // Start is called before the first frame update
    void Start()
{
    DisplayUI(false);

    playerInventory = new List<Item>();
    playerInventory.Add(new Item(Item.ItemType.MEAT));
    playerInventory.Add(new Item(Item.ItemType.GOLD));

    playerInventory[1].nb = 300;
}


    // Update is called once per frame
    void Update()
{
    // --------------------------
    // OPEN INVENTORY
    // --------------------------
    if (!isVisible && Input.GetKeyDown(KeyCode.I))
    {
        isVisible = true;
        DisplayUI(true);
        currentInventoryIndex = 0;
        return; // prevent inventory from closing instantly
    }

    // If not visible, stop here
    if (!isVisible) return;

    // --------------------------
    // DISPLAY CURRENT ITEM
    // --------------------------
    if (playerInventory.Count == 0)
    {
        // No items → close inventory
        isVisible = false;
        DisplayUI(false);
        return;
    }

    Item currentItem = playerInventory[currentInventoryIndex];

    inventoryText.GetComponent<TextMeshProUGUI>().text =
        currentItem.name + " [" + currentItem.nb + "]";

    inventoryDescription.GetComponent<TextMeshProUGUI>().text =
        currentItem.description + "\n\nPress [U] to Use";

    inventoryImage.GetComponent<RawImage>().texture =
        currentItem.GetTexture();

    // --------------------------
    // CYCLE ITEMS WITH I
    // --------------------------
    if (Input.GetKeyDown(KeyCode.I))
    {
        currentInventoryIndex++;

        // If we cycle past the end, CLOSE the inventory
        if (currentInventoryIndex >= playerInventory.Count)
        {
            currentInventoryIndex = 0;
            isVisible = false;
            DisplayUI(false);
            return;
        }
    }

    // --------------------------
    // USE / EAT ITEM WITH U
    // --------------------------
    if (Input.GetKeyDown(KeyCode.U))
    {
        if (currentItem.familyType == Item.ItemFamilyType.FOOD)
        {
            ControlPlayer cp = GetComponent<ControlPlayer>();
        if (cp != null)
        {
         cp.IncreaseHealth(currentItem.healthBenefits);
        }
        else
        {
            Debug.LogWarning("ControlPlayer component not found on the player.");
        }


            playerInventory.RemoveAt(currentInventoryIndex);

            // If last item removed → close
            if (playerInventory.Count == 0)
            {
                currentInventoryIndex = 0;
                isVisible = false;
                DisplayUI(false);
                return;
            }

            // Otherwise clamp index
            if (currentInventoryIndex >= playerInventory.Count)
                currentInventoryIndex = playerInventory.Count - 1;
        }
    }
}



    void checkInventory()
    {

        for (int i = 0; i < playerInventory.Count;i++)
        {
            print(playerInventory[i].ItemInfo());

        }

    }


    void DisplayUI(bool toggle)
    {

        inventoryText.SetActive(toggle);
        inventoryPanel.SetActive(toggle);
        inventoryImage.SetActive(toggle);
        inventoryDescription.SetActive(toggle);
                      

    }

    public bool UpdateItem (Item.ItemType type, int nbItemsToAdd)
    {
        bool foundSimilarItem = false;

        for (int i = 0; i < playerInventory.Count;i++)
        {

            if (playerInventory[i].type == type)
            {

                if (playerInventory[i].nb + nbItemsToAdd <= playerInventory[i].maxNb)
                {
                    playerInventory[i].nb += nbItemsToAdd; foundSimilarItem = true;
                    break;

                }
                else return false;
            }

        }

        if (!foundSimilarItem) { playerInventory.Add(new Item(type)); playerInventory[playerInventory.Count - 1].nb = nbItemsToAdd; }
        return true;

    }


    public int GetMoney()
    {

        for (int i = 0; i < playerInventory.Count; i++)
        {

            if (playerInventory[i].type == Item.ItemType.GOLD)
            {

                return (playerInventory[i].nb);
            }

        }
        return 0;

    }

    public void AddPurchasedItems(List<Item> purchasedItems)
    {
        bool t;
        for (int i = 0; i < purchasedItems.Count; i++)
        {
            if (purchasedItems[i].nb > 0) t = UpdateItem(purchasedItems[i].type, purchasedItems[i].nb);

        }

    }

    public void SetMoney(int newAmount)
    {

        for (int i = 0; i < playerInventory.Count;i++)
        {

            if (playerInventory[i].type == Item.ItemType.GOLD)
            {
                playerInventory[i].nb = newAmount;

            }

        }

    }

}
