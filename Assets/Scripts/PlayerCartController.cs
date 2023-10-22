using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RootMotion.Demos;
using CreatingCharacters.Events;

public class PlayerCartController : MonoBehaviour
{
    public static event Action onTryCheckoutGroceries;

    public bool playerIsAtCheckout = false;
    public CharacterPuppet myPuppet;

    public List<GameObject> cartsInFrontOfPlayer = new List<GameObject>();
    public List<GameObject> groceriesInFrontOfPlayer = new List<GameObject>();
    public List<GameObject> groceriesHeldByPlayer = new List<GameObject>();
    public List<GameObject> groceriesStandsInFrontOfPlayer = new List<GameObject>();
    // let's get the GroceryListManager and store the required  THERE not in this script
    public List<GameManager.allGroceryTypes> playersGroceryListToCollect = new List<GameManager.allGroceryTypes>();
    // public List<GameManager.allGroceryTypes> playersGroceriesInCart = new List<GameManager.allGroceryTypes>();

    public List<GameObject> UI_ButtonActionPrompts = new List<GameObject>();

    [SerializeField] private IntEvent onGroceryCountUpdate;

    public int maxGroceriesHeldByPlayer = 10;
    public bool playerHasAllGroceriesOnList = false;
    public double percentageOfGroceriesCollected = 0f;

    public GameObject shoppingCart_positionHolder;
    public IKControl ikControl;
    public bool isHoldingCart = false;
    public bool isHoldingGroceries = false;

    public GameObject rightHandContainer;
    public GameObject leftHandContainer;
    public GameObject groceryBundleContainer;

    private float releaseSpeed = 1.5f;

    public GameObject heldCart;
    private GameObject closestGO;
    private GameObject previousClosestGO;
    
    public GameObject closestCart;
    public GameObject closestGrocery;
    public GameObject closestGroceryStand;

    private GameObject rightHandGrocery;
    private GameObject leftHandGrocery;
    
    private Collider myTriggerVolume;
    private GameObject L_Hand_IK_GO;
    private GameObject R_Hand_IK_GO;

    // Start is called before the first frame update
    void Start()
    {
        ikControl = GetComponentInChildren<IKControl>();
        // shoppingCart_positionHolder = transform.Find("shoppingCart_positionHolder").gameObject;
        myTriggerVolume = GetComponent<BoxCollider>();
        myPuppet = GetComponent<CharacterPuppet>();
    }

    // Update is called once per frame
    void Update()
    {
        // ADD THIS:
        // if character is in tumble state - DROP all carts and groceries!!
        if(myPuppet != null)
        {
            if (myPuppet.puppet.state == RootMotion.Dynamics.BehaviourPuppet.State.Unpinned)
            {
                if (isHoldingCart)
                {
                    ReleaseCart();
                    return;
                }

                else if (isHoldingGroceries)
                {
                    DropAllGroceries();
                }
            }
        }
        
        if (Input.GetButtonDown("X_1"))
        {
            if (isHoldingCart)
            {
                ReleaseCart();
                // onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
            }
            else if (!isHoldingCart)
            {
                if (groceriesInFrontOfPlayer.Count > 0)
                {
                    GrabClosestGrocery();
                    // onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
                }

                if (cartsInFrontOfPlayer.Count > 0)
                {
                    if (!isHoldingGroceries)
                        GrabClosestCart();
                    
                    else if (isHoldingGroceries)
                    {
                        PlaceGroceriesInClosestCart();
                        // CheckGroceriesInCartAgainstShoppingList();
                        if (playerHasAllGroceriesOnList)
                        {
                            //TODO: activate UI "get to the checkout!"
                        }
                    }
                }
            }
        }

        if (Input.GetButtonDown("Y_1"))
        {
            if (playerIsAtCheckout)
            {
                return;
            }
            
            if (!playerIsAtCheckout)
            {
                if (isHoldingCart)
                {
                    heldCart.GetComponent<ShoppingCart>().EmptyCart();
                    onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
                }
                
                if (groceriesHeldByPlayer.Count > 0)
                {
                    DropAllGroceries();
                    onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
                }
            }
        }

        isHoldingGroceries = (groceriesHeldByPlayer.Count > 0) ? true : false;
    }
    void GrabClosestGrocery()
    {
        closestGrocery = FindClosestGO(groceriesInFrontOfPlayer);
        closestGrocery.GetComponent<GroceryItem>().thisGrocery.myGlowScript.SetGlow(false);
        closestGrocery.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = true;
    
        if (groceriesHeldByPlayer.Count < maxGroceriesHeldByPlayer)
        {
            if (rightHandGrocery == null)
            {
                rightHandGrocery = closestGrocery;
                closestGrocery = null;

                groceriesHeldByPlayer.Add(rightHandGrocery);

                ParentPhysicalObjectToPlayer(rightHandGrocery,rightHandContainer.transform);

                isHoldingGroceries = true;
                groceriesInFrontOfPlayer.Remove(closestGrocery);
                groceriesInFrontOfPlayer.Remove(rightHandGrocery);
                return;
            }

            else if (rightHandGrocery != null && leftHandGrocery == null)
            {
                leftHandGrocery = closestGrocery;
                closestGrocery = null;

                groceriesHeldByPlayer.Add(leftHandGrocery);

                ParentPhysicalObjectToPlayer(leftHandGrocery, leftHandContainer.transform);

                isHoldingGroceries = true;
                groceriesInFrontOfPlayer.Remove(closestGrocery);
                return;
            }

            else if (groceriesHeldByPlayer.Count >= 2)
            {
                // create grocery bundle ala Link in BOTW

                groceriesHeldByPlayer.Add(closestGrocery);

                ParentPhysicalObjectToPlayer(closestGrocery, groceryBundleContainer.transform);

                isHoldingGroceries = true;
                groceriesInFrontOfPlayer.Remove(closestGrocery);
            }

            groceriesInFrontOfPlayer.Remove(closestGrocery);
        }
        else
        {
            Debug.Log("Player is maxed out at " + maxGroceriesHeldByPlayer + " groceries!");
            // trigger UI "You can't carry anymore!"
            return;
        }
    }
    void PlaceGroceriesInClosestCart()
    {
        heldCart = FindClosestGO(cartsInFrontOfPlayer);
        
        ShoppingCart shoppingCartScript = heldCart.GetComponent<ShoppingCart>();

        shoppingCartScript.AddGroceryToCart(groceriesHeldByPlayer);
        GameManager.instance._GroceryListManager.AddAquiredGroceryItems(heldCart.GetComponent<ShoppingCart>().containedGroceryGOs);
        groceriesHeldByPlayer.Clear();
        
        // CheckGroceriesInCartAgainstShoppingList();
        // shoppingCartScript.UpdateCartUI(true, percentageOfGroceriesCollected);

        // onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);

        rightHandGrocery = null;
        leftHandGrocery = null;
        closestGrocery = null;
        isHoldingGroceries = false;
    }
    
    void GrabClosestCart()
    {
        heldCart = FindClosestGO(cartsInFrontOfPlayer);

        if (heldCart != null)
        {
            ShoppingCart myTempCart = heldCart.GetComponent<ShoppingCart>();
            
            GameManager.instance._GroceryListManager.RemovedAllItemsFromCart();
            GameManager.instance._GroceryListManager.AddAquiredGroceryItems(heldCart.GetComponent<ShoppingCart>().containedGroceryGOs);

            ParentPhysicalObjectToPlayer(heldCart, shoppingCart_positionHolder.transform);

            ikControl.leftHandObj = heldCart.gameObject.transform.Find("L_Hand_IK");
            ikControl.rightHandObj = heldCart.gameObject.transform.Find("R_Hand_IK");
            ikControl.ikActive = true;

            myTriggerVolume.enabled = false;
            isHoldingCart = true;

            cartsInFrontOfPlayer.Clear();
            
            
            // myTempCart.UpdateCartUI(false,0f);
            
            // Debug.Log("Grabbed a Cart!");
        }
    }
    
    void ReleaseCart()
    {
        // cartsInFrontOfPlayer.Clear();
        // Quaternion cartRotation = myCart.transform.rotation;

        ikControl.leftHandObj = null;
        ikControl.rightHandObj = null;
        ikControl.ikActive = false;
        
        // CheckGroceriesInCartAgainstShoppingList();
        
        // update held cart's pie chart
        // heldCart.GetComponent<ShoppingCart>().UpdateCartUI(true,percentageOfGroceriesCollected);
        
        // enable pie chart before releasing the cart
        
        UnParentPhysicalObjectFromPlayer(heldCart);
        heldCart.GetComponent<Rigidbody>().AddForce(transform.forward * releaseSpeed, ForceMode.VelocityChange);
        myTriggerVolume.enabled = true;
        isHoldingCart = false;

        heldCart = null;
        // Debug.Log("Released Cart!");
    }
    void DropAllGroceries()
    {
        groceriesHeldByPlayer.TrimExcess();
        foreach (GameObject go in groceriesHeldByPlayer)
        {
            // drop all in bundle container
            closestGrocery.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = false;
            UnParentPhysicalObjectFromPlayer(go);
            go.GetComponent<Rigidbody>().AddForce(transform.forward * releaseSpeed, ForceMode.VelocityChange);
            // groceriesHeldByPlayer.Remove(go);
        }

        groceriesHeldByPlayer.Clear();
        onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
        isHoldingGroceries = false;

        closestGrocery = null;
        rightHandGrocery = null;
        leftHandGrocery = null;
    }
    
    void ParentPhysicalObjectToPlayer(GameObject go, Transform parentObject)
        {
            go.GetComponent<Rigidbody>().isKinematic = true;
            go.GetComponent<Collider>().enabled = false;
            go.transform.SetParent(parentObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }
    void UnParentPhysicalObjectFromPlayer(GameObject go)
        {
            go.GetComponent<Rigidbody>().isKinematic = false;
            go.GetComponent<Rigidbody>().useGravity = true;
            go.GetComponent<Collider>().enabled = true;
            go.transform.SetParent(null);
        }
    GameObject FindClosestGO(List<GameObject> goList)
    { 
        
        previousClosestGO = closestGO;

        closestGO = null;

        float shortestDistance = 20f;

        foreach (GameObject go in goList)
        {
            float f = Vector3.Distance(go.transform.position, transform.position);

            if (f < shortestDistance)
            {
                shortestDistance = f;
                closestGO = go;
            }
        }

        if(closestGO != null)
        {
            if (closestGO.GetComponent<ShoppingCart>() || closestGO.GetComponent<GroceryItem>())
            {
                closestGO.GetComponent<ObjectGlow>().SetGlow(true);
                if(previousClosestGO)
                    previousClosestGO.GetComponent<ObjectGlow>().SetGlow(false);
            }
        }

        return closestGO;
    }
    /*
    public void CheckGroceriesInCartAgainstShoppingList()
    {
        Debug.Log("CheckGroceriesInCartAgainstShoppingList()"); 

        int numMatches = 0;

        List<GameManager.allGroceryTypes> playersGroceriesInCart = new List<GameManager.allGroceryTypes>();

        List<GameManager.allGroceryTypes> tempPlayersGroceryListToCollect = new List<GameManager.allGroceryTypes>();
        List<GameManager.allGroceryTypes> tempGroceriesInCart = new List<GameManager.allGroceryTypes>();

        for (int i = 0; i < heldCart.GetComponent<ShoppingCart>().containedGroceryGOs.Count; i++)
        {
            playersGroceriesInCart.Add(heldCart.GetComponent<ShoppingCart>().containedGroceryGOs[i].GetComponent<GroceryItem>().thisGrocery.myGroceryType);
        }
        
        // Add grocery list items to temp list
        for (int i = 0; i < playersGroceryListToCollect.Count; i++)
        {
            tempPlayersGroceryListToCollect.Add(playersGroceryListToCollect[i]);
        }
        
        // Add groceries in cart items to temp list
        for (int i = 0; i < playersGroceriesInCart.Count; i++)
        {
            tempGroceriesInCart.Add(playersGroceriesInCart[i]);
        }
        
        for ( int i = 0; i < tempGroceriesInCart.Count; i++ )
        {
            if (tempPlayersGroceryListToCollect.Contains(tempGroceriesInCart[i]))
            {
                numMatches++;
                tempPlayersGroceryListToCollect.Remove(tempGroceriesInCart[i]);
                tempPlayersGroceryListToCollect.TrimExcess();
            }
        }
        
        if(tempPlayersGroceryListToCollect.Count <= 0)
        {
            playerHasAllGroceriesOnList = true;
            //TODO: enable UI "get your cart to the checkout!"
        }
        else
        {
            playerHasAllGroceriesOnList = false;
        }

        percentageOfGroceriesCollected = ((double)numMatches / (double)playersGroceryListToCollect.Count) * 100f;
        Debug.Log("percentage of groceries in cart: " + percentageOfGroceriesCollected);
        Debug.Log("numMatches: " + numMatches);
    }

    */
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grocery") & other.gameObject.GetComponent<GroceryItem>())
        {
            if (other.gameObject.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart
                || other.gameObject.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer
                || groceriesInFrontOfPlayer.Contains(other.gameObject))
            {
                return;
            }
            else
            {
                groceriesInFrontOfPlayer.Add(other.gameObject);
                closestGrocery = FindClosestGO(groceriesInFrontOfPlayer);
            }
        }

        if (other.gameObject.CompareTag("ShoppingCart") && !isHoldingCart && !cartsInFrontOfPlayer.Contains(other.gameObject))
        {
            cartsInFrontOfPlayer.Add(other.gameObject);
            closestCart = FindClosestGO(cartsInFrontOfPlayer);
        }

        if (other.gameObject.CompareTag("GroceryStand") && !groceriesStandsInFrontOfPlayer.Contains(other.gameObject))
        {
            groceriesStandsInFrontOfPlayer.Add(other.gameObject);
            closestGroceryStand = FindClosestGO(groceriesStandsInFrontOfPlayer);
        }
        
        // Set Glow on CLOSEST OBJECT - whether grocery, cart, or grocery stand
        // Set Glow based on distance and priority - e.g. Groceries always take priority over carts
        
        
        
        /* backup old logic
        if (other.gameObject.CompareTag("Grocery") & other.gameObject.GetComponent<GroceryItem>())
        {
            if (other.gameObject.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart)
            {
                return;
            }
            else
            {
                groceriesInFrontOfPlayer.Add(other.gameObject);
                closestGrocery = FindClosestGO(groceriesInFrontOfPlayer);
            }
        }

        if (other.gameObject.CompareTag("ShoppingCart") && !isHoldingCart)
        {
            cartsInFrontOfPlayer.Add(other.gameObject);
            closestCart = FindClosestGO(cartsInFrontOfPlayer);
        }

        if (other.gameObject.CompareTag("GroceryStand"))
        {
            groceriesStandsInFrontOfPlayer.Add(other.gameObject);
            closestGroceryStand = FindClosestGO(groceriesStandsInFrontOfPlayer);
        }
        */
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Grocery"))
        {
            other.GetComponent<ObjectGlow>().SetGlow(false);
            groceriesInFrontOfPlayer.Remove(other.gameObject);
            groceriesInFrontOfPlayer.TrimExcess();
        }
        
        if (other.gameObject.CompareTag("ShoppingCart"))
        {
            other.GetComponent<ObjectGlow>().SetGlow(false);
            cartsInFrontOfPlayer.Remove(other.gameObject);
            cartsInFrontOfPlayer.TrimExcess();
        }
        
        if (other.gameObject.CompareTag("GroceryStand"))
        {
            other.GetComponent<ObjectGlow>().SetGlow(false);
            groceriesStandsInFrontOfPlayer.Remove(other.gameObject);
            groceriesStandsInFrontOfPlayer.TrimExcess();
        }
    }
}
