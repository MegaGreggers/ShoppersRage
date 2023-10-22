namespace CreatingCharacters.Events
{
    [System.Serializable] public struct ButtonAndVerb
    {
        public string actionName;
        public UI_ActionPrompt.buttonTypes thisButton;
        public UI_ActionPrompt.verbTypes thisVerb;
        public bool isVisible;
    }
}