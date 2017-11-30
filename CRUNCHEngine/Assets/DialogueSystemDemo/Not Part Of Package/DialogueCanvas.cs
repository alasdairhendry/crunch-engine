using Crunch.Communication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCanvas : DialogueReader 
{
    [SerializeField] private Text info;
    [SerializeField] private Text text;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject container;

    protected override void Start()
    {
        // Initialize the Dialogue Reader
        base.Start();
    }

    public override void OnStart(DialogueStart start)       // Called when any Dialogue is started
    {
        base.OnStart(start);
        Debug.Log("STARAAAT");
    }

    public override void OnSpeech(DialogueSpeech speech)    // Called when a Speech section begins
    {
        base.OnSpeech(speech);
        
        DestroyButtons();   // Destroy any previous buttons 

        info.text = "Talking to " + activeDialogue.gameObject.name;     // Inform the player who they are talking to

        text.text = activeDialogue.gameObject.name + " says:\n\n" + speech.speech;  // Print the Speech onto the screen 
    }

    public override void OnOptions(DialogueOptions options) // Called when an Options section begins
    {
        base.OnOptions(options);
        
        DestroyButtons();   // Destroy any previous buttons

        info.text = "Talking to " + activeDialogue.gameObject.name; // Inform the player who the aye talking to
        text.text = ""; // Nullify any previous Speech data

        // Loop through the options we have been given
        for (int i = 0; i < options.options.Count; i++)
        {
            // Spawn a button for each option
            GameObject button = Instantiate(buttonPrefab);
            button.transform.parent = container.transform;

            // Display the Option inside the button
            button.GetComponentInChildren<Text>().text = options.GetOptions[i];

            // Tell the button to move forward when clicked
            int x = i; button.GetComponent<Button>().onClick.AddListener(() => { Iterate(x); });            
        }        
    }

    public override void OnEnd(DialogueEnd end)         // Called when the Dialogue ends
    {
        base.OnEnd(end);
        
        DestroyButtons();   // Destroy any buttons we may have left over 

        // Inform the user that the dialogue has finished
        info.text = "Dialogue Ended";
        text.text = "";
    }

    private void DestroyButtons()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }
}