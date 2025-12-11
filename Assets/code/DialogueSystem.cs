using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    // --- UI references (assign these in the Inspector on Diana) ---
    public TextMeshProUGUI dialogueText;   // Text element inside dialoguePanel
    public GameObject dialoguePanel;       // Panel background object
    public Image dialogueImage;            // Portrait image (optional, can be left unused)

    // --- Dialogue data ---
    string nameOfCharacter;
    Dialogue[] dialogues;
    int currentDialogueIndex = 0;
    bool waitingForUserInput = false;
    bool dialogueIsActive = false;

    // Internal reference to the panel (we’ll prefer the one from the inspector)
    GameObject dialoguePanelObj;

    void Start()
    {
        // NPC name should match the <character name="..."> in dialogues.xml
        nameOfCharacter = gameObject.name;

        // Load dialogues from XML
        LoadDialoguesSafe();

        // ----- Panel reference -----
        if (dialoguePanel != null)
        {
            dialoguePanelObj = dialoguePanel;
        }
        else
        {
            // Fallback to search by name if not assigned
            dialoguePanelObj = GameObject.Find("dialoguePanel");
        }

        // Hide panel at start
        if (dialoguePanelObj != null)
            dialoguePanelObj.SetActive(false);

        // NOTE: dialogueImage is optional. You can assign it in the inspector
        // and set a sprite on its Image component; the script doesn’t have to
        // do anything with it for now.
    }

    // -------------------------------------------------------------------------
    // Load all dialogues for this character from Resources/dialogues.xml
    // -------------------------------------------------------------------------
    void LoadDialoguesSafe()
    {
        // dialogues.xml must be in Assets/Resources and imported as TextAsset
        TextAsset textAsset = Resources.Load<TextAsset>("dialogues");
        if (textAsset == null)
        {
            Debug.LogError("DialogueSystem: dialogues.xml not found in Resources.");
            dialogues = new Dialogue[0];
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        // Find the <character> node matching this NPC's name
        XmlNode characterNode = null;
        foreach (XmlNode character in doc.SelectNodes("dialogues/character"))
        {
            if (character.Attributes != null &&
                character.Attributes.GetNamedItem("name") != null &&
                character.Attributes.GetNamedItem("name").Value == nameOfCharacter)
            {
                characterNode = character;
                break;
            }
        }

        if (characterNode == null)
        {
            Debug.LogWarning("DialogueSystem: No dialogues found for character " + nameOfCharacter);
            dialogues = new Dialogue[0];
            return;
        }

        // Build list of Dialogue objects
        List<Dialogue> dialogueList = new List<Dialogue>();
        foreach (XmlNode dialogueFromXML in characterNode.SelectNodes("dialogue"))
        {
            Dialogue d = new Dialogue();

            // Main message
            if (dialogueFromXML.Attributes != null)
            {
                XmlNode contentAttr = dialogueFromXML.Attributes.GetNamedItem("content");
                if (contentAttr != null)
                    d.message = contentAttr.Value;
            }

            d.response = new string[2];
            d.targetForResponse = new int[2];

            int choiceIndex = 0;
            foreach (XmlNode choice in dialogueFromXML.SelectNodes("choice"))
            {
                if (choice.Attributes == null) continue;
                if (choiceIndex >= 2) break; // only 2 choices (A and B)

                XmlNode contentAttr = choice.Attributes.GetNamedItem("content");
                XmlNode targetAttr  = choice.Attributes.GetNamedItem("target");

                if (contentAttr != null)
                    d.response[choiceIndex] = contentAttr.Value;

                int targetVal = -1;
                if (targetAttr != null)
                    int.TryParse(targetAttr.Value, out targetVal);

                d.targetForResponse[choiceIndex] = targetVal;
                choiceIndex++;
            }

            // Fill missing choices with empty text and -1 (end)
            for (int i = choiceIndex; i < 2; i++)
            {
                d.response[i] = "";
                d.targetForResponse[i] = -1;
            }

            dialogueList.Add(d);
        }

        dialogues = dialogueList.ToArray();
        Debug.Log("DialogueSystem: loaded " + dialogues.Length + " dialogues for " + nameOfCharacter);
    }

    // -------------------------------------------------------------------------
    // Main update: handles displaying and advancing dialogue when active
    // -------------------------------------------------------------------------
    void Update()
    {
        // If dialogue not active or nothing loaded, do nothing
        if (!dialogueIsActive || dialogues == null || dialogues.Length == 0)
            return;

        if (!waitingForUserInput)
        {
            // Show the panel
            if (dialoguePanelObj != null)
                dialoguePanelObj.SetActive(true);

            // Display current dialogue or end
            if (currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogues.Length)
            {
                DisplayDialogue();
            }
            else
            {
                EndDialogue();
                return;
            }

            waitingForUserInput = true;
        }
        else
        {
            // Handle player choices A / B
            if (Input.GetKeyDown(KeyCode.A))
            {
                int nextIndex = dialogues[currentDialogueIndex].targetForResponse[0];
                currentDialogueIndex = nextIndex;
                waitingForUserInput = false;
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                int nextIndex = dialogues[currentDialogueIndex].targetForResponse[1];
                currentDialogueIndex = nextIndex;
                waitingForUserInput = false;
            }
        }
    }

    // -------------------------------------------------------------------------
    // Show the current dialogue line in the UI
    // -------------------------------------------------------------------------
    void DisplayDialogue()
    {
        if (dialogueText == null)
        {
            Debug.LogError("DialogueSystem: dialogueText reference is missing.");
            return;
        }

        Dialogue d = dialogues[currentDialogueIndex];

        string textToDisplay =
            "[" + nameOfCharacter + "] " + d.message +
            "\n\n[A]> " + d.response[0] +
            "\n[B]> " + d.response[1];

        dialogueText.text = textToDisplay;
    }

    // -------------------------------------------------------------------------
    // End the conversation and re-enable player actions
    // -------------------------------------------------------------------------
    void EndDialogue()
    {
        dialogueIsActive = false;
        waitingForUserInput = false;
        currentDialogueIndex = 0;

        if (dialoguePanelObj != null)
            dialoguePanelObj.SetActive(false);

        // Re-enable player movement
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            ControlPlayer cp = playerObj.GetComponent<ControlPlayer>();
            if (cp != null)
                cp.EndTalking();
        }

        // OPTIONAL: notify QuestSystem if a GameManager exists
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            QuestSystem qs = gm.GetComponent<QuestSystem>();
            if (qs != null)
                qs.Notify(QuestSystem.possibleActions.talk_to, nameOfCharacter);
        }
    }

    // -------------------------------------------------------------------------
    // Called by ControlPlayer when the player enters the NPC trigger
    // -------------------------------------------------------------------------
    public void startDialogue()
    {
        currentDialogueIndex = 0;
        waitingForUserInput = false;
        dialogueIsActive = true;
    }
}
