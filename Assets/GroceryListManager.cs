using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroceryListManager : MonoBehaviour
{
    public GameObject listPaper;
    private PlayerCartController _playerCartController;
    private int numberOfSlotsOnGroceryListPaper = 0;
    private int numUniqueGroceryTypes = 0;

    // For Gameplay, set all the required groceries on the shopping list on these UI gameobjects
    private List<GroceryList_UIItem> groceryList_UIItems = new List<GroceryList_UIItem>();

    public List<GameManager.allGroceryTypes> groceryList_RequiredItems = new List<GameManager.allGroceryTypes>();
    public List<GameManager.allGroceryTypes> groceryList_AquiredItems = new List<GameManager.allGroceryTypes>();

    // private List<GameManager.allGroceryTypes>

    // Start is called before the first frame update
    void Start()
    {
        listPaper = gameObject;
        numberOfSlotsOnGroceryListPaper = listPaper.transform.childCount;

        _playerCartController = GameManager.instance._PlayerCartController;

        // update UI list based on groceryList_RequiredItems!
        PopulateGroceryListUI();

        if (numUniqueGroceryTypes > numberOfSlotsOnGroceryListPaper)
        {
            Debug.LogError(
                "The required groceries are longer than the listPaper!  Please limit the number of unique grocery items to " +
                numberOfSlotsOnGroceryListPaper);
            return;
        }

        // Scrape the UI items for the required list
        ScrapeUIItemsForRequiredList();


        // AddAquiredGroceryItems(GameManager.allGroceryTypes.Cheerios);
    }

    private void ScrapeUIItemsForRequiredList()
    {
        for (int i = 0; i < groceryList_UIItems.Count; i++)
        {
            if (groceryList_UIItems[i].quantityRequired == 0)
                return;
            else if (groceryList_UIItems[i].quantityRequired >= 1)
            {
                for (int j = 0; j < groceryList_UIItems[i].quantityRequired; j++)
                {
                    groceryList_RequiredItems.Add(groceryList_UIItems[i].type);
                }
            }
        }

        if (groceryList_RequiredItems.Count == 0)
        {
            Debug.LogWarning("GrocerListManager has no Required Groceries set!");
            return;
        }
    }

    private void PopulateGroceryListUI()
    {
        // collect all the UI items on the grocery list paper   
        for (int i = 0; i < numberOfSlotsOnGroceryListPaper; i++)
        {
            groceryList_UIItems.Add(listPaper.transform.GetChild(i).gameObject.GetComponent<GroceryList_UIItem>());
        }
    }

    public void AddAquiredGroceryItems(List<GameObject> groceriesToAdd)
    {
        groceryList_AquiredItems.Clear();
        
        for (int i = 0; i < groceriesToAdd.Count; i++)
        {
            // Gets the type from the grocery GameObject and adds it to the aquired items
            groceryList_AquiredItems.Add(groceriesToAdd[i].GetComponent<GroceryItem>().thisGrocery.myGroceryType);
        }
        
        UpdateGroceryListUI(groceryList_AquiredItems);
    }

    // public bool ListContainsGroceryType(GameManager.allGroceryTypes type, List<GroceryList_UIItem> gList)
    // {
    //     bool containsItem = false;
    // 
    //     for (int i = 0; i < gList.Count; i++)
    //     {
    //         if (gList[i].type == type)
    //         {
    //             containsItem = true;
    //         }
    //     }
    // 
    //     return containsItem;
    // }

    public GroceryList_UIItem ReturnMatchingGroceryListItemFromList(GameManager.allGroceryTypes type, List<GroceryList_UIItem> gList)
    {
        // GroceryList_UIItem gUIItem = new GroceryList_UIItem(GameManager.allGroceryTypes.Bananas, 1);
        GroceryList_UIItem gUIItem = new GroceryList_UIItem();

        for (int i = 0; i < gList.Count; i++)
        {
            if (gList[i].type == type)
            {
                gUIItem = gList[i];
            }
        }

        return gUIItem;
    }

    public bool PlayerHasAllGroceries()
    {
        // Compare the aquired groceries to the the required groceries and return a bool
        bool hasAllGroceries = true;

        for (int i = 0; i < groceryList_UIItems.Count; i++)
        {
            if (groceryList_UIItems[i].quantityRequired > 0)
            {
                if (!groceryList_UIItems[i].allGroceriesofThisTypeAquired)
                    hasAllGroceries = false;
            }
        }
        
        return hasAllGroceries;
    }

    public void UpdateGroceryListUI(List<GameManager.allGroceryTypes> aquiredGroceryTypesList)
    {
        // zero out all the aquired quantities so we can't add them repeatedly
        for (int i = 0; i < groceryList_UIItems.Count; i++)
        {
            if(groceryList_UIItems[i].quantityRequired > 0)
                groceryList_UIItems[i].quantityAquired = 0;
        }
        
        // 
        for (int i = 0; i < aquiredGroceryTypesList.Count; i++)
        {
            ReturnMatchingGroceryListItemFromList(aquiredGroceryTypesList[i], groceryList_UIItems).quantityAquired++;
        }

        foreach (var uiItem in groceryList_UIItems)
        {
            uiItem.UpdateListItemUIElements();
        }

        GameManager.instance.SetUIPlayerDirective(GameManager.UI_PlayerDirectives.Checkout, PlayerHasAllGroceries());
        
    }
    
    public void RemovedAllItemsFromCart()
    {
        groceryList_AquiredItems.Clear();
        UpdateGroceryListUI(groceryList_AquiredItems);
    }

    
}
