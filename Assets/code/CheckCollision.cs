using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    [Header("Damage done to targets")]
    public int damage = 20;   // default fallback

    private void OnTriggerEnter(Collider other)
    {
        // 1) Safely compute damage from GameManager if it exists
        GameObject gmObj = GameObject.Find("GameManager");
        GameManager gm = null;

        if (gmObj != null)
        {
            gm = gmObj.GetComponent<GameManager>();
        }

        if (gm != null && gm.player != null)
        {
            damage = gm.player.power / 2;
        }
        else
        {
            // Fallback so we don't crash if GameManager or player is missing
            // You can change this number if you want stronger hits
            damage = 20;
            Debug.LogWarning("CheckCollision: GameManager or player missing, using default damage " + damage);
        }

        // 2) Only care about objects tagged "target"
        if (!other.CompareTag("target"))
        {
            return;
        }

        // 3) Try to get the ManageTargetHealth component on the object or its parent
        ManageTargetHealth target = other.GetComponent<ManageTargetHealth>();
        if (target == null && other.transform.parent != null)
        {
            target = other.transform.parent.GetComponent<ManageTargetHealth>();
        }

        if (target != null)
        {
            target.DecreaseHealth(damage);
            Debug.Log("Hit target '" + other.name + "' for " + damage + " damage. Remaining health: " + target.GetHealth());
        }
        else
        {
            Debug.LogWarning("CheckCollision: Hit object tagged 'target' but no ManageTargetHealth found on " + other.name);
        }
    }
}
