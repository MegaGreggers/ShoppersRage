using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryItem : MonoBehaviour, IInteractable
{
    public Grocery thisGrocery = new Grocery();
    // private bool _isKingInteractable;
    // public bool IsKingInteractable
    // {
    //     get => _isKingInteractable;
    //     set
    //     {
    //         _isKingInteractable = value;
    //         thisGrocery.myGlowScript.SetGlow(_isKingInteractable);
    //     }
    // }

    [System.Serializable]
    public struct Grocery
    {
        public GameObject groceryObject;
        [HideInInspector]
        public ObjectGlow myGlowScript;
        public bool isInPlayersCart;
        public bool isHeldByPlayer;
        public GameManager.allGroceryTypes myGroceryType;
    }

    private void Awake()
    {
        thisGrocery.myGlowScript = GetComponent<ObjectGlow>();
    }
    private void Start()
    {
        thisGrocery.groceryObject = gameObject;
        thisGrocery.isInPlayersCart = false;
        if(thisGrocery.myGlowScript)
            thisGrocery.myGlowScript.SetGlow(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // We don't want ALL groceries to light up, just the closest one - PlayerCartController should handle this.
        // 
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     thisGrocery.myGlowScript.SetGlow(true);
        // }
        // 
    }

    private void OnTriggerExit(Collider other)
    {
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     thisGrocery.myGlowScript.SetGlow(false);
        // }
    }

    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public ObjectGlow InteractionObjectGlowScript => thisGrocery.myGlowScript;
    public ShoppingCart InteractableShoppingCartScript => null;
    public GroceryItem InteractableGroceryItemScript => GetComponent<GroceryItem>();
    public GameObject InteractableGameObject => gameObject;
    
    public bool Interact(Interactor interactor)
    {
        // groceries in front of the player are handled in the Interactor, including the closest grocery
        PlayerCartController playerCartController = interactor.GetComponent<PlayerCartController>();
        playerCartController.GrabGrocery(gameObject);
        
        // Debug.Log("Grabbing Grocery!");
        return true;
    }
}