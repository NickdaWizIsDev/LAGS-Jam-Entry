using UnityEngine;
using TMPro;

public class MerchantDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    
    public string[] dialogueLines = {
        "Don't go around tryna be a jack of all trades, son. I'm sure you know how the sayin' goes.",
        "Deep in the caves, your pickaxe is your best friend. Well, your only friend.",
        "Watch your head down there. And your toes. Actually, just watch everything.",
        "You got the coin, I got the gear. Simple as that.",
        "Another day, another quota. Don't let management grind you down to dust.",
        "Upgrades ain't cheap, but neither is a medical bill."
    };

    private void Start()
    {
        SpeakRandomLine();
    }

    public void SpeakRandomLine()
    {
        if (dialogueLines.Length == 0 || dialogueText == null) return;
        
        int randomIndex = Random.Range(0, dialogueLines.Length);
        dialogueText.text = dialogueLines[randomIndex];
    }

    // New method to force him to say the price when you hover
    public void SpeakSpecificLine(string text)
    {
        if (dialogueText != null) dialogueText.text = text;
    }
}