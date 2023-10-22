using System.Collections;
using System.Collections.Generic;
using CreatingCharacters.Events;
using UnityEngine;

public class ButtonAndVerbManager : MonoBehaviour
{
    public List<UI_ActionPrompt.buttonTypes> buttonTypes = new List<UI_ActionPrompt.buttonTypes>();
    public List<GameObject> buttonGOs = new List<GameObject>();

    public List<UI_ActionPrompt.verbTypes> verbTypes = new List<UI_ActionPrompt.verbTypes>();
    public List<GameObject> verbGOs = new List<GameObject>();


    // We need to align our UI_ActionPrompt.buttonTypes enum and Verb enums with UI game objects:
    [SerializeField] private Dictionary<UI_ActionPrompt.buttonTypes, GameObject> UI_buttonBindings = new Dictionary<UI_ActionPrompt.buttonTypes, GameObject>();
    [SerializeField] private Dictionary<UI_ActionPrompt.verbTypes, GameObject> UI_verbBindings = new Dictionary<UI_ActionPrompt.verbTypes, GameObject>();

    private void Awake()
    {
        // foreach()

        // int buttonCount = System.Enum.GetNames(typeof(UI_ActionPrompt.buttonTypes)).Length;
        // int verbCount = System.Enum.GetNames(typeof(UI_ActionPrompt.verbTypes)).Length;

        int buttonCount = buttonGOs.Count;
        int verbCount = verbGOs.Count;


        for (int i = 0; i < buttonCount; i++)
        {
            UI_buttonBindings.Add(buttonTypes[i], buttonGOs[i]);
        }
        
        for (int i = 0; i < verbCount; i++)
        {
            UI_verbBindings.Add(verbTypes[i], verbGOs[i]);
        }


    }

    public void ToggleUIButtonAndVerbPrompts(ButtonAndVerb buttonAndVerb)
    {
        // Debug.Log("Button and Verb: " + buttonAndVerb.actionName.ToString() + "visibility = " + buttonAndVerb.isVisible);

        GameObject requestedButton = null;
        GameObject requestedVerb = null;

        UI_buttonBindings.TryGetValue(buttonAndVerb.thisButton, out requestedButton);
        UI_verbBindings.TryGetValue(buttonAndVerb.thisVerb, out requestedVerb);

        requestedButton.SetActive(buttonAndVerb.isVisible);
        requestedVerb.SetActive(buttonAndVerb.isVisible);
    }


    public void AlignButtonsAndVerbs()
    {

    }
}
