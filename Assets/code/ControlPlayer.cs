using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlPlayer : MonoBehaviour
{
    GameObject weapon;
    bool weaponIsActive = false;

    float speed, rotatioAroundY;
    Animator anim;
    CharacterController controller;
    AnimatorStateInfo info;
    bool isTalking = false;

    // ----- INVENTORY / PICKUP -----
    GameObject objectToPickUp;
    bool itemToPickUpNearBy = false;
    GameObject userMessage;

    GameObject healthUI, skillsUI, shopUI;

    [Header("Health Settings")]
    [Tooltip("Health value between 0 and 100")]
    public int health = 50;

    public bool shopIsDisplayed;

    // -------- HEALTH ----------
    public void IncreaseHealth(int amount)
    {
        health += amount;
        if (health > 100) health = 100;
        print("Health: " + health);

        // Safely try to update the health bar if it exists
        GameObject hb = GameObject.Find("healthBar");
        if (hb != null)
        {
            ManageBar bar = hb.GetComponent<ManageBar>();
            if (bar != null)
            {
                bar.SetValue(health);
            }
            else
            {
                Debug.LogWarning("healthBar found, but no ManageBar component attached.");
            }
        }
        else
        {
            // This just avoids a crash if we haven't made the health bar yet in the book
            Debug.LogWarning("No GameObject named 'healthBar' found. Skipping health bar update.");
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // Safely get user message and hide it
        userMessage = GameObject.Find("userMessage");
        if (userMessage != null)
        {
            userMessage.SetActive(false);
        }

        // Health bar
        GameObject hb = GameObject.Find("healthBar");
        if (hb != null)
        {
            hb.GetComponent<ManageBar>().SetValue(health);
        }

        // Shop UI
        shopUI = GameObject.Find("shopUI");
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }

        // Weapon
        GameObject weaponObj = GameObject.Find("playerWeapon");
        if (weaponObj != null)
        {
            weapon = weaponObj;
            weapon.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ControlPlayer: could not find 'playerWeapon' in the scene.");
        }
    }

    // ----------------- UPDATE -----------------
    void Update()
    {
        if (!shopIsDisplayed)
        {
            if (isTalking) return;

            info = anim.GetCurrentAnimatorStateInfo(0);
            speed = Input.GetAxis("Vertical");
            rotatioAroundY = Input.GetAxis("Horizontal");
            anim.SetFloat("speed", speed);
            gameObject.transform.Rotate(0, rotatioAroundY, 0);
            if (speed > 0) controller.Move(transform.forward * speed * 2.0f * Time.deltaTime);

            // ----- ITEM PICKUP INPUT -----
            if (itemToPickUpNearBy)
            {
                // Press U to collect
                if (Input.GetKeyDown(KeyCode.U))
                {
                    PickUpObject1();
                }

                // Optional: N to cancel / hide prompt
                if (Input.GetKeyDown(KeyCode.N))
                {
                    GameObject msgObj = GameObject.Find("userMessageText");
                    if (msgObj != null)
                    {
                        Text txt = msgObj.GetComponent<Text>();
                        if (txt != null) txt.text = "";
                    }

                    if (userMessage != null)
                        userMessage.SetActive(false);
                }
            }

            // ----- WEAPON TOGGLE -----
            if (Input.GetKeyDown(KeyCode.P))
            {
                weaponIsActive = !weaponIsActive;
                if (weaponIsActive) anim.SetTrigger("useWeapon");
                else anim.SetTrigger("putWeaponBack");
            }

            if (info.IsName("useWeapon"))
            {
                if (info.normalizedTime % 1.0 >= .50f)
                {
                    if (weapon != null)
                        weapon.SetActive(true);
                }
            }

            if (info.IsName("putWeaponBack"))
            {
                if (info.normalizedTime % 1.0 >= .50f)
                {
                    if (weapon != null)
                        weapon.SetActive(false);
                }
                else if (weapon != null)
                {
                    weapon.SetActive(true);
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (weaponIsActive) anim.SetTrigger("attackWithWeapon");
            }
        }

        // if (Input.GetKeyDown(KeyCode.B)) GameObject.Find("shopSystem").GetComponent<ShopSystem>().Init();
    }

    // ------------- COLLISIONS / TRIGGERS -------------

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.name == "Diana" && !isTalking)
        {
            hit.collider.gameObject.GetComponent<DialogueSystem>().startDialogue();
            isTalking = true;
            anim.SetFloat("speed", 0);
            hit.collider.isTrigger = true;
            hit.collider.gameObject.GetComponent<BoxCollider>().size = new Vector3(2, 1, 2);
        }
    }

    public void StartTalking()
    {
        isTalking = true;
        anim.SetFloat("speed", 0);
    }

    public void EndTalking()
    {
        isTalking = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        // ----- ITEM TO BE COLLECTED -----
        if (other.CompareTag("itemToBeCollected"))
        {
            objectToPickUp = other.gameObject;
            itemToPickUpNearBy = true;

            // Just display the message. Actual pickup happens on key press.
            PickUpObject2();
        }

        // ----- SHOP TRIGGER -----
        if (other.gameObject.name == "shop")
        {
            shopIsDisplayed = true;
            anim.SetFloat("speed", 0);
            displayShopUI();
            GameObject.Find("shopSystem").GetComponent<ShopSystem>().Init();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("itemToBeCollected"))
        {
            itemToPickUpNearBy = false;

            if (userMessage != null && userMessage.activeSelf)
            {
                GameObject msgObj = GameObject.Find("userMessageText");
                if (msgObj != null)
                {
                    Text txt = msgObj.GetComponent<Text>();
                    if (txt != null)
                    {
                        txt.text = "";
                    }
                }

                userMessage.SetActive(false);
            }
        }
    }

    // ------------- PICKUP HELPERS -------------

    // Show message when we enter the trigger
    void PickUpObject2()
    {
        if (userMessage == null)
            return;

        userMessage.SetActive(true);

        GameObject msgObj = GameObject.Find("userMessageText");
        if (msgObj == null) return;

        Text txt = msgObj.GetComponent<Text>();
        if (txt == null) return;

        // Try to build a nice message based on item type
        if (objectToPickUp != null)
        {
            ObjectToBeCollected data = objectToPickUp.GetComponent<ObjectToBeCollected>();
            if (data != null)
            {
                Item temp = new Item(data.type);
                string message = "You just found " + temp.name + "\nPress [U] to collect";
                txt.text = message;
                return;
            }
        }

        // Fallback
        txt.text = "Press [U] to collect";
    }

    // Actually add the item to the inventory
    void PickUpObject1()
    {
        if (objectToPickUp == null) return;

        ObjectToBeCollected data = objectToPickUp.GetComponent<ObjectToBeCollected>();
        if (data == null) return;

        // Create a temporary Item so we can get its name for quest notifications
        Item tempItem = new Item(data.type);
        string itemName = tempItem.name;

        InventorySystem inv = GetComponent<InventorySystem>();
        if (inv == null)
        {
            Debug.LogError("InventorySystem component missing on Player!");
            return;
        }

        if (inv.UpdateItem(data.type, 1))
        {
            // Successfully added to inventory
            Destroy(objectToPickUp);
            objectToPickUp = null;
            itemToPickUpNearBy = false;

            if (userMessage != null)
                userMessage.SetActive(false);

            // Notify quest system
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null)
            {
                QuestSystem qs = gm.GetComponent<QuestSystem>();
                if (qs != null)
                {
                    qs.Notify(QuestSystem.possibleActions.acquire_a, itemName);
                }
            }
        }
        else
        {
            // Inventory full for that item
            GameObject msgObj = GameObject.Find("userMessageText");
            if (msgObj != null)
            {
                Text txt = msgObj.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = "You can't pick up this item as you have reached the limit.";
                }
            }
        }
    }

    // ------------- SHOP & HEALTH -------------

    public void displayShopUI()
    {
        if (shopUI != null)
            shopUI.SetActive(true);
    }

    public void DecreaseHealth(int amount)
    {
        health -= amount;
        if (health <= 0) health = 0;

        GameObject.Find("healthBar").GetComponent<ManageBar>().SetValue(health);

        if (health <= 0)
        {
            health = 50;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
