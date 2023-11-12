using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShoppingCart : MonoBehaviour, IInteractable
{
    public bool showCartUI = false;
    public PieChartControllerSimple thisCartsCompletionPieChart;

    public List<GameObject> containedGroceryGOs = new List<GameObject>();
    public List<GameObject> itemSlotGOs = new List<GameObject>();

    public GameObject thisCartsCollectedGroceryParent;

    public ObjectGlow myGlowScript;

    public Vector3 debugTextOffset = Vector3.zero;

    Camera cam;

    private GameObject nextFreeSlot = null;
    private Rigidbody rb;

    private void Start()
    {
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        
        myGlowScript = GetComponent<ObjectGlow>();
        myGlowScript.SetGlow(false);

        // get all item slots from CollectedGroceries GO
        for(int i = 0; i < thisCartsCollectedGroceryParent.transform.childCount; i++)
        {
            itemSlotGOs.Add(thisCartsCollectedGroceryParent.transform.GetChild(i).gameObject);
        }

        thisCartsCompletionPieChart.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude > 0.1f)
        {
            // Debug.Log(Vector3.Dot(transform.up, Vector3.down)); 
            if (Vector3.Dot(transform.up, Vector3.down) > -0.7f)
            {
                // Debug.Log("Cart tipped over!");
                if (containedGroceryGOs.Count > 0)
                {
                    EmptyCart();
                }
            }
        }
    }

    public void AddGroceriesToCart(List<GameObject> groceries)
    {
        foreach(GameObject go in groceries)
        {
            go.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = false;
            containedGroceryGOs.Add(go);
            ParentItemToFreeItemSlot(go);
        }
    }

    public void ParentItemToFreeItemSlot(GameObject go)
    {
        nextFreeSlot = null;

        for (int i = 0; i < itemSlotGOs.Count; i++)
        {
            if(itemSlotGOs[i].transform.childCount == 0)
            {
                nextFreeSlot = itemSlotGOs[i];
            }
            if (nextFreeSlot == null)
                Debug.Log("The cart is full!");
        }

        if(nextFreeSlot != null)
        {
            go.transform.SetParent(nextFreeSlot.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<Collider>().enabled = false;
            go.GetComponent<Rigidbody>().isKinematic = true;
            go.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = true;
        }
        else
        {
            go.transform.SetParent(null);
            go.GetComponent<Collider>().enabled = true;
            go.GetComponent<Rigidbody>().isKinematic = false;
            go.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = false;
        }
    }
    public void EmptyCart()
    {
        // foreach(GameObject go in containedGroceryGOs)
        // {
        //     go.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = false;
        //     
        //     go.transform.SetParent(null);
        //     go.GetComponent<Collider>().enabled = true;
        //     go.GetComponent<Rigidbody>().isKinematic = false;
        //     go.GetComponent<Rigidbody>().AddForce(transform.up * 6f + new Vector3(-3f,0f,0f) , ForceMode.VelocityChange);
        //     containedGroceryGOs.Remove(go);
        //     containedGroceryGOs.TrimExcess();
        // }
// 
        
        for (int i = containedGroceryGOs.Count - 1; i >= 0; i--)
        {
            containedGroceryGOs[i].GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = false;
            
            containedGroceryGOs[i].transform.SetParent(null);
            containedGroceryGOs[i].GetComponent<Collider>().enabled = true;
            containedGroceryGOs[i].GetComponent<Rigidbody>().isKinematic = false;
            containedGroceryGOs[i].GetComponent<Rigidbody>().AddForce(transform.up * 6f + new Vector3(-3f,0f,0f) , ForceMode.VelocityChange);
            containedGroceryGOs.Remove(containedGroceryGOs[i]);
            containedGroceryGOs.TrimExcess();
        }

        GameManager.instance._GroceryListManager.RemovedAllItemsFromCart();
    }

    public void UpdateCartUI(bool active, float percentage)
    {
       thisCartsCompletionPieChart.gameObject.SetActive(active);
       thisCartsCompletionPieChart.SetValue(percentage);
    }

    private void OnGUI()
    {
        if (showCartUI)
        {
            for (int i = 0; i < containedGroceryGOs.Count; i++)
            {
                // debugTextOffset;
                
                Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

                GUI.Label(new Rect(screenPos.x, screenPos.y, 200f, 1000f), containedGroceryGOs[i].name);
            }
        }
    }
    
    // just a list of groceries
    // a UI element that displays how complete the cart is compared to players shopping list - probably a PIE CHART!

    /*
    private GameObject collectedGroceryContainer;

    

    private void Start()
    {
        collectedGroceryContainer = gameObject.transform.Find("CollectedGroceries").gameObject;
    }

    
    public void PutGroceryInThisCart(GameObject go)
    {
        go.GetComponent<Collider>().enabled = false;
        go.GetComponent<Rigidbody>().isKinematic = true;

        // The following finds empty item transform in cart and assigns it as a parent.
        
        for (int i = 0; i < collectedGroceryContainer.transform.childCount; i++)
        {
            Transform t = collectedGroceryContainer.transform.GetChild(i);

            if (t.transform.childCount < 1)
            {
                go.transform.parent = collectedGroceryContainer.transform.GetChild(i);
            }
            if( i == collectedGroceryContainer.transform.childCount)
            {
                Debug.Log("Cart is full!");
            }
        }

        go.transform.localPosition = Vector3.zero;
        go.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = true;
        containedGroceryGOs.Add(go);
        CheckItems();
    }

    void CheckItems()
    {
        int itemsRemaining = 0;

        foreach(GameObject g in containedGroceryGOs)
        {
            if (g.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart)
            {

                // update UI!
            }
            else
            {
                itemsRemaining++;
            }
        }

        Debug.Log("There are " + itemsRemaining + " items remaining!");

        if (itemsRemaining == 0)
        {
            // playerHasAllGroceries = true;
            Debug.Log("Get to the Checkout!");
        }
    }
    */
    
    
    [SerializeField] private string _prompt;
    [SerializeField] private ObjectGlow glowObject;
    public string InteractionPrompt => _prompt;
    public ObjectGlow InteractionObjectGlowScript => myGlowScript;
    public ShoppingCart InteractableShoppingCartScript => GetComponent<ShoppingCart>();
    public GroceryItem InteractableGroceryItemScript => null;
    public GameObject InteractableGameObject => gameObject;
    
    public bool Interact(Interactor interactor)
    {
        PlayerCartController playerCartController = interactor.GetComponent<PlayerCartController>();
        // groceries in front of the player are handled in the Interactor, including the closest grocery
        if (playerCartController.isHoldingGroceries)
        { 
            playerCartController.PlaceGroceriesInCart(gameObject);
        }
        if (playerCartController.isHoldingCart)
        {
            playerCartController.ReleaseCart();
        }
        else if (!playerCartController.isHoldingCart)
        {
            playerCartController.GrabCart(gameObject);
        }
        
        
        
        Debug.Log("Grabbing a Cart!");
        return true;
    }
}
