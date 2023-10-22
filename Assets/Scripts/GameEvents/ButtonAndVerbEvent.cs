using UnityEngine;

namespace CreatingCharacters.Events
{
    [CreateAssetMenu(fileName = "New Button and Verb Event", menuName = "Game Events/Button and Verb Event")]
    public class ButtonAndVerbEvent : BaseGameEvent<ButtonAndVerb> 
    { 
        public void Raise() => Raise(new ButtonAndVerb()); 
    }
}
