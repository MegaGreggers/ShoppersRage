using System.Collections;
using System.Collections.Generic;
using CreatingCharacters.Events;
using UnityEngine.Events;
using UnityEngine;
using System;

public class Checkout : MonoBehaviour
{
    public bool thisCheckoutHasAllItems = false;
    public bool allowCheckout = false;

    public List<GameManager.allGroceryTypes> groceriesAtTheCheckout = new List<GameManager.allGroceryTypes>();

    public Dictionary<GameManager.allGroceryTypes, int> groceries_groceryQuantities = new Dictionary<GameManager.allGroceryTypes, int>();
    
    public PlayerCartController playerCartControllerAtCheckout = null;

    [SerializeField] private ButtonAndVerbEvent uiPrompt_checkoutButtonAndVerbEvent;
    [SerializeField] private ButtonAndVerb uiPrompt_checkoutButtonAndVerb_enable;
    [SerializeField] private ButtonAndVerb uiPrompt_checkoutButtonAndVerb_disable;



    public GameObject UI_Victory;
    public string actionName;
    public GameObject UI_Checkout_ButtonGO;
    public GameObject UI_Checkout_VerbGO;

    private void Start()
    {
        // PlayerCartController.onTryCheckoutGroceries += PlayerCartController_onTryCheckoutGroceries;
        // uiPrompt_checkoutButtonAndVerbEvent = GetComponent<>();
    }

    private void Update()
    {
        if (playerCartControllerAtCheckout != null)
        {
            if (Input.GetButtonDown("Y_1"))
            {
                CheckGroceriesForVictory();
            }
            
            if (UI_Victory != null && thisCheckoutHasAllItems)
            {
                UI_Victory.SetActive(true);
                GameManager.instance.LevelCompleted();
                uiPrompt_checkoutButtonAndVerbEvent.Raise(uiPrompt_checkoutButtonAndVerb_disable);
                GameManager.instance.SetUIPlayerDirective(GameManager.UI_PlayerDirectives.Checkout, false);
            }
        }
    }
    
    // private void PlayerCartController_onTryCheckoutGroceries()
    // {
    //     if(thisCheckoutHasAllItems)
    //         UI_Victory.SetActive(true);
    // }

    public void CheckGroceriesForVictory()
    {
        Debug.Log("CheckGroceries()");

        if (GameManager.instance._GroceryListManager.PlayerHasAllGroceries())
        {
            thisCheckoutHasAllItems = true;
        }
        else
        {
            thisCheckoutHasAllItems = false;
        }
    }

    int GetNumberOfGroceries (List<GameManager.allGroceryTypes> list, GameManager.allGroceryTypes countThisGroceryType)
    {
        int num = 0;

        foreach (GameManager.allGroceryTypes gt in list)
        {
            if(gt == countThisGroceryType)
            {
                num++;
            }
        }

        return num;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player at Checkout");
            
            uiPrompt_checkoutButtonAndVerbEvent.Raise(uiPrompt_checkoutButtonAndVerb_enable);

            playerCartControllerAtCheckout = other.gameObject.GetComponentInParent<PlayerCartController>();
            if (playerCartControllerAtCheckout != null)
            {
                playerCartControllerAtCheckout.playerIsAtCheckout = true;
                // if(playerCartControllerAtCheckout.hasAllGroceriesOnList)
                
                // UI_Checkout_ButtonGO.SetActive(true);
                // UI_Checkout_VerbGO.SetActive(true);
                // if (UI_Victory != null)
                //     UI_Victory.SetActive(true);
            }
            else
            {
                Debug.Log("Player not found!");
            }
        }
        

        if (other.gameObject.CompareTag("Grocery"))
        {
            groceriesAtTheCheckout.Add(other.gameObject.GetComponent<GroceryItem>().thisGrocery.myGroceryType);
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCartControllerAtCheckout = null;
            uiPrompt_checkoutButtonAndVerbEvent.Raise(uiPrompt_checkoutButtonAndVerb_disable);
            other.GetComponentInParent<PlayerCartController>().playerIsAtCheckout = false;
            UI_Checkout_ButtonGO.SetActive(false);
        }
        if (other.gameObject.CompareTag("Grocery"))
        {
            groceriesAtTheCheckout.Remove(other.gameObject.GetComponent<GroceryItem>().thisGrocery.myGroceryType);
            groceriesAtTheCheckout.TrimExcess();
        }
    }
}
