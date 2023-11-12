using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string InteractionPrompt { get; }
    ObjectGlow InteractionObjectGlowScript { get; }
    ShoppingCart InteractableShoppingCartScript { get; }
    GroceryItem InteractableGroceryItemScript { get; }
    GameObject InteractableGameObject { get; }

    bool Interact(Interactor interactor);
}
