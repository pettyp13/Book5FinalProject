using UnityEngine;

public class ManageNPCDialogueTrigger : MonoBehaviour
{
    DialogueSystem dialogue;

    void Start()
    {
        // Get the DialogueSystem on the same object (Diana)
        dialogue = GetComponent<DialogueSystem>();

        // Make sure the collider is a trigger
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react when the PLAYER enters the trigger
        if (other.CompareTag("Player"))
        {
            // Stop the player's movement (your ControlPlayer should have this)
            ControlPlayer playerControl = other.GetComponent<ControlPlayer>();
            if (playerControl != null)
            {
                playerControl.StartTalking();
            }

            // Start the dialogue on this NPC (Diana)
            if (dialogue != null)
            {
                dialogue.startDialogue();
            }
        }
    }
}
