using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;  // Assign your TextMeshPro component in the Inspector
    public float typingSpeed = 0.05f;  // Speed at which characters are typed (in seconds per character)
    private string fullText;  // The full text that will be revealed

    void Start()
    {
        // Store the full text that is assigned to the TextMeshPro component
        fullText = textMeshPro.text;

        // Start the typing effect
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        string[] textParts = fullText.Split('~');
        foreach (string part in textParts)
        {
            // Clear the text field before starting the typing effect
            textMeshPro.text = "";

            foreach (char letter in part.ToCharArray())
            {
                textMeshPro.text += letter;  // Add the next letter
                yield return new WaitForSeconds(typingSpeed);  // Wait for the specified delay
            }
            yield return new WaitForSeconds(.25f);
        }
    }
}
