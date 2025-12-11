using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToBeCollected : MonoBehaviour
{
    // Prefabs for the different 3D objects that can represent an inventory item
    public GameObject baton;
    public GameObject sword;
    public GameObject apple;
    public GameObject meat;
    public GameObject gold;
    public GameObject redDiamond;
    public GameObject yellowDiamond;
    public GameObject blueDiamond;

    // Which type of Item this pickup represents
    public Item.ItemType type;

    // The actual Item object that will be added to the inventory
    public Item item;

    void Start()
    {
        // Create the corresponding Item instance for this pickup
        item = new Item(type);

        // Decide which 3D model to spawn based on the type
        GameObject g = null;

        switch (type)
        {
            case Item.ItemType.BATON:
                g = baton;
                break;

            case Item.ItemType.SWORD:
                g = sword;
                break;

            case Item.ItemType.APPLE:
                g = apple;
                break;

            case Item.ItemType.MEAT:
                g = meat;
                break;

            case Item.ItemType.GOLD:
                g = gold;
                break;

            case Item.ItemType.RED_DIAMOND:
                g = redDiamond;
                break;

            case Item.ItemType.YELLOW_DIAMOND:
                g = yellowDiamond;
                break;

            case Item.ItemType.BLUE_DIAMOND:
                g = blueDiamond;
                break;

            default:
                break;
        }

        // Instantiate the chosen 3D object as a child of this pickup
        if (g != null)
        {
            GameObject g1 = Instantiate(g, transform.position, Quaternion.identity);
            g1.transform.parent = transform;
        }
    }
}
