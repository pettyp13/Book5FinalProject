using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // <--- IMPORTANT for TextMeshPro

public class ShopSystem : MonoBehaviour
{
    public List<Item> shopItems;
    [Tooltip("Prefab of the UI for a single shop item (your 'shopItem' prefab).")]
    public GameObject shopItemComponent;

    GameObject[] shopItemComponents;
    int totalPurchase = 0;
    int initialMoney;
    public int moneyLeft;

    // Top-left position (in local space of the shopItems panel)
    float topLeftX, topLeftY;

    void Start()
    {
        // Init() is called when the player enters the shop trigger.
    }

    void Update()
    {
    }

    // ---------------- INIT ----------------
    public void Init()
    {
        Debug.Log("ShopSystem.Init() called");

        // 1) Get Player & InventorySystem
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("ShopSystem: No GameObject named 'Player' found in the scene.");
            return;
        }

        InventorySystem inv = player.GetComponent<InventorySystem>();
        if (inv == null)
        {
            Debug.LogError("ShopSystem: Player has no InventorySystem component.");
            return;
        }

        initialMoney = inv.GetMoney();
        moneyLeft = initialMoney;

        // 2) Layout for first item
        topLeftX = 50f;
        topLeftY = 350f;

        // 3) Build list of items the shop sells
        shopItems = new List<Item>();
        shopItems.Add(new Item(Item.ItemType.YELLOW_DIAMOND));
        shopItems.Add(new Item(Item.ItemType.BLUE_DIAMOND));
        shopItems.Add(new Item(Item.ItemType.RED_DIAMOND));
        shopItems.Add(new Item(Item.ItemType.MEAT));
        shopItems.Add(new Item(Item.ItemType.APPLE));

        shopItemComponents = new GameObject[shopItems.Count];

        // 4) Update "Money Left" UI text (supports Text OR TMP)
        GameObject moneyLeftObj = GameObject.Find("shopMoneyLeftValue");
        if (moneyLeftObj == null)
        {
            Debug.LogError("ShopSystem: Cannot find GameObject 'shopMoneyLeftValue' in the scene.");
        }
        else
        {
            Text t = moneyLeftObj.GetComponent<Text>();
            TMP_Text tmp = moneyLeftObj.GetComponent<TMP_Text>();

            if (t != null)
                t.text = initialMoney.ToString();
            else if (tmp != null)
                tmp.text = initialMoney.ToString();
            else
                Debug.LogError("ShopSystem: 'shopMoneyLeftValue' has neither Text nor TextMeshPro component.");
        }

        // 5) Create an entry UI for each shop item
        for (int i = 0; i < shopItems.Count; i++)
        {
            SetupShopItemComponent(i);
        }
    }

    // ------------- CREATE ONE SHOP-ITEM UI -------------
    void SetupShopItemComponent(int index)
    {
        if (shopItemComponent == null)
        {
            Debug.LogError("ShopSystem: shopItemComponent prefab is NOT assigned in the Inspector.");
            return;
        }

        if (index < 0 || index >= shopItems.Count)
        {
            Debug.LogError("ShopSystem: SetupShopItemComponent index out of range: " + index);
            return;
        }

        shopItems[index].nb = 0;

        GameObject go = Instantiate(shopItemComponent, transform.position, Quaternion.identity);
        shopItemComponents[index] = go;

        // Assign index on the ShopItem script
        ShopItem si = go.GetComponent<ShopItem>();
        if (si == null)
        {
            Debug.LogError("ShopSystem: Prefab is missing the ShopItem component.");
        }
        else
        {
            si.index = index;
        }

        // ----- Find children by name -----
        Transform labelTr = go.transform.Find("itemLabel");
        Transform qtyTr   = go.transform.Find("itemQty");
        Transform imgTr   = go.transform.Find("itemImage");

        if (labelTr == null)
            Debug.LogError("ShopSystem: child 'itemLabel' not found under prefab " + go.name);
        if (qtyTr == null)
            Debug.LogError("ShopSystem: child 'itemQty' not found under prefab " + go.name);
        if (imgTr == null)
            Debug.LogError("ShopSystem: child 'itemImage' not found under prefab " + go.name);

        if (labelTr == null || qtyTr == null || imgTr == null)
        {
            Debug.LogError("ShopSystem: Missing one of the children: itemLabel / itemQty / itemImage.");
            return;
        }

        // ----- Get components: support Text OR TMP -----
        Text labelText = labelTr.GetComponent<Text>();
        TMP_Text labelTMP = labelTr.GetComponent<TMP_Text>();

        Text qtyText = qtyTr.GetComponent<Text>();
        TMP_Text qtyTMP = qtyTr.GetComponent<TMP_Text>();

        RawImage img = imgTr.GetComponent<RawImage>();
        if (img == null)
        {
            Debug.LogError("ShopSystem: 'itemImage' is missing a RawImage component.");
            return;
        }

        // Set name & quantity text
        string labelString = shopItems[index].name + "($" + shopItems[index].price + ")";
        string qtyString = shopItems[index].nb.ToString();

        if (labelText != null)       labelText.text = labelString;
        else if (labelTMP != null)   labelTMP.text = labelString;

        if (qtyText != null)         qtyText.text = qtyString;
        else if (qtyTMP != null)     qtyTMP.text = qtyString;

        // ----- Layout within the shopItems panel -----
        Transform bg = go.transform.Find("itemBg");
        if (bg == null)
        {
            Debug.LogError("ShopSystem: Could not find child 'itemBg' on shopItem prefab.");
            return;
        }

        float width = bg.GetComponent<RectTransform>().sizeDelta.x;
        float borderAroundEachItem = 1.05f;

        go.name = "shopItem_" + index + "_" + shopItems[index].name;

        // Parent under left panel "shopItems"
        Transform container = GameObject.Find("shopItems")?.transform;
        if (container == null)
        {
            Debug.LogError("ShopSystem: Cannot find GameObject 'shopItems' to parent item UIs under.");
            return;
        }

        go.transform.SetParent(container, false);

        go.transform.localPosition = new Vector3(
            topLeftX + (index % 3) * (width * borderAroundEachItem),
            topLeftY - (index / 3) * (width * borderAroundEachItem),
            0f
        );

        // Set image texture
        img.texture = shopItems[index].GetTexture();
    }

    // ------------- TOTAL & MONEY LEFT -------------
    public void UpdateTotal(int itemIndex, int itemAmount)
{
    // --- SAFETY CHECKS ---
    if (shopItems == null || shopItems.Count == 0)
        return;

    if (itemIndex < 0 || itemIndex >= shopItems.Count)
        return;

    // Update amount
    shopItems[itemIndex].nb = itemAmount;

    // Recalculate total
    int tempTotal = CalculateTotal();

    // ---------------- UPDATE TOTAL TEXT ----------------
    GameObject totalObj = GameObject.Find("shopTotalValue");
    if (totalObj != null)
    {
        Text t = totalObj.GetComponent<Text>();
        TMP_Text tmp = totalObj.GetComponent<TMP_Text>();

        if (t != null) t.text = tempTotal.ToString();
        else if (tmp != null) tmp.text = tempTotal.ToString();
    }

    totalPurchase = tempTotal;
    moneyLeft = initialMoney - tempTotal;

    // ---------------- UPDATE MONEY LEFT TEXT ----------------
    GameObject moneyObj = GameObject.Find("shopMoneyLeftValue");
    if (moneyObj != null)
    {
        Text t = moneyObj.GetComponent<Text>();
        TMP_Text tmp = moneyObj.GetComponent<TMP_Text>();

        if (t != null) t.text = moneyLeft.ToString();
        else if (tmp != null) tmp.text = moneyLeft.ToString();
    }
}
    public int CalculateTotal()
    {
        int temp = 0;
        for (int i = 0; i < shopItems.Count; i++)
        {
            temp += shopItems[i].nb * shopItems[i].price;
        }
        return temp;
    }

    public bool canAddItemsTocart(int index)
    {
        return (moneyLeft >= shopItems[index].price &&
                shopItems[index].nb < shopItems[index].maxNb);
    }
}
