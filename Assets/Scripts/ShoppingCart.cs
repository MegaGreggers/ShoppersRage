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
    public bool _cartIsFull = false;

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
        
        // Create an instance of the custom comparer
        GameObjectHeightComparer comparer = new GameObjectHeightComparer();
        // Sort the list using the custom comparer
        itemSlotGOs.Sort(comparer);
        // Now, gameObjectsList is sorted by height from lowest to highest
        
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
            if (!_cartIsFull)
            {
                go.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = false;
                
                ParentItemToFreeItemSlot(go);
            }
            else
            {
                go.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = false;
                ParentItemToFreeItemSlot(go);
            }
        }
    }

    public void ParentItemToFreeItemSlot(GameObject go)
    {
        nextFreeSlot = null;
        
        // checking for an itemSlot with no grocery parented to it already
        for (int i = 0; i < itemSlotGOs.Count; i++)
        {
            
            if (nextFreeSlot == null)
            {
                if(itemSlotGOs[i].transform.childCount == 0)
                {
                    nextFreeSlot = itemSlotGOs[i];
                }
            }
            
            if (nextFreeSlot == null)
            {
                // TODO: UI alert that cart is full? OR let slots be infinite, but will cause cart to tip over
                Debug.Log("The cart is full!");
                _cartIsFull = true;
            }
        }

        if(nextFreeSlot != null)
        {
            go.transform.SetParent(nextFreeSlot.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<Collider>().enabled = false;
            go.GetComponent<Rigidbody>().isKinematic = true;
            go.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart = true;
            containedGroceryGOs.Add(go);
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
        _cartIsFull = false;
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
            return true;
        }
        // this doesn't cuerrently make sense as the held shopping cart is exluded from being interactable
        if (playerCartController.isHoldingCart)
        {
            playerCartController.ReleaseCart();
        }
        else if (!playerCartController.isHoldingCart)
        {
            playerCartController.GrabCart(gameObject);
        }


        // Debug.Log("Grabbing a Cart!");
        return true;
    }
    
    public class GameObjectHeightComparer : IComparer<GameObject>
    {
        public int Compare(GameObject obj1, GameObject obj2)
        {
            // Compare the y positions of the game objects
            float height1 = obj1.transform.position.y;
            float height2 = obj2.transform.position.y;

            if (height1 < height2)
                return -1;
            else if (height1 > height2)
                return 1;
            else
                return 0;
        }
    }

    public class HeightSorter : MonoBehaviour
    {
        void SortByHeight()
        {
            // Your list of GameObjects
            List<GameObject> gameObjectsList = new List<GameObject>();

            // Populate your list with game objects

            // Create an instance of the custom comparer
            GameObjectHeightComparer comparer = new GameObjectHeightComparer();

            // Sort the list using the custom comparer
            gameObjectsList.Sort(comparer);

            // Now, gameObjectsList is sorted by height from lowest to highest
        }
    }
}
