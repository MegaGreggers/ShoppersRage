using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryItem : MonoBehaviour
{
    public Grocery thisGrocery = new Grocery();

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
}
