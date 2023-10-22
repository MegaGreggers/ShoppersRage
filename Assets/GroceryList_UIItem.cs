using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GroceryList_UIItem : MonoBehaviour
{
    public GameManager.allGroceryTypes type;
    public int quantityRequired = 0;
    public int quantityAquired = 0;
    public TextMeshPro TMP_groceryType;
    public TextMeshPro TMP_groceryQuantity;
    public bool allGroceriesofThisTypeAquired = false;

    private void Awake()
    {
        TMP_groceryType = GetComponent<TextMeshPro>();
        TMP_groceryQuantity = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>();

        SetGroceryListItem_TMP(type, quantityAquired);

        if (quantityRequired == 0)
        {
            TMP_groceryType.enabled = false;
            TMP_groceryQuantity.enabled = false;
        }
    }

    private void SetGroceryListItem_TMP(GameManager.allGroceryTypes groceryType, int aquiredGroceryQuantity)
    {
        string aquiredQuant = aquiredGroceryQuantity.ToString();
        string type = groceryType.ToString();
        
        if (aquiredGroceryQuantity >= quantityRequired){}
        TMP_groceryType.text = type;
        TMP_groceryQuantity.text = (aquiredQuant + "/" + quantityRequired);

        if (aquiredGroceryQuantity >= quantityRequired)
        {
            TMP_groceryType.text = ("<s>"+type+"</s>");
            TMP_groceryQuantity.text = ("<s>"+aquiredQuant + "/" + quantityRequired+"</s>");
        }
    }

    public void UpdateListItemUIElements()
    {
        TMP_groceryType.text = type.ToString();
        TMP_groceryQuantity.text = (quantityAquired + "/" + quantityRequired);

        if (quantityAquired >= quantityRequired)
        {
            TMP_groceryType.text = ("<s>"+type+"</s>");
            TMP_groceryQuantity.text = ("<s>"+quantityAquired + "/" + quantityRequired+"</s>");
            allGroceriesofThisTypeAquired = true;
        }
        else if (quantityAquired < quantityRequired)
        {
            allGroceriesofThisTypeAquired = false;
        }
    }
    
    // public GroceryList_UIItem(GameManager.allGroceryTypes type, int quantityRequired)
    // {
    //     this.type = type;
    //     this.quantityRequired = quantityRequired;
    // }
}
