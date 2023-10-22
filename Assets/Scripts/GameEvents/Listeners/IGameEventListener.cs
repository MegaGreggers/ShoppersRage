using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatingCharacters.Events
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T item);
    }
}
