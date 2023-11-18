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
    // let's get the GroceryListManager and store the required groceries THERE not in this script
    // public List<GameManager.allGroceryTypes> playersGroceryListToCollect = new List<GameManager.allGroceryTypes>();
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
    public Interactor interactor;

    private float releaseSpeed = 1.5f;

    public GameObject heldCart;
    private GameObject closestGO;
    private GameObject previousClosestGO;
    
    public GameObject closestCart;
    // public GameObject closestGrocery;
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
        interactor = GetComponent<Interactor>();
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
                    ReleaseCart();
                    // heldCart.GetComponent<ShoppingCart>().EmptyCart();
                }
                
                if (groceriesHeldByPlayer.Count > 0)
                {
                    DropAllGroceries();
                }
            }
        }

        isHoldingGroceries = (groceriesHeldByPlayer.Count > 0) ? true : false;
    } 
    public void GrabGrocery(GameObject closestGrocery)
    {
        closestGrocery.GetComponent<GroceryItem>().thisGrocery.myGlowScript.SetGlow(false);
        closestGrocery.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = true;

        groceriesHeldByPlayer.Add(closestGrocery);
    
        if (groceriesHeldByPlayer.Count < maxGroceriesHeldByPlayer)
        {
            if (rightHandGrocery == null)
            {
                rightHandGrocery = closestGrocery;

                ParentPhysicalObjectToPlayer(rightHandGrocery,rightHandContainer.transform);

                isHoldingGroceries = true;
                interactor.validGroceryColliderList.Remove(closestGrocery.GetComponent<Collider>());
                closestGrocery = null;
                return;
            }

            if (rightHandGrocery != null && leftHandGrocery == null)
            {
                leftHandGrocery = closestGrocery;

                ParentPhysicalObjectToPlayer(leftHandGrocery, leftHandContainer.transform);

                isHoldingGroceries = true;
                interactor.validGroceryColliderList.Remove(closestGrocery.GetComponent<Collider>());
                closestGrocery = null;
                return;
            }

            if (groceriesHeldByPlayer.Count >= 2)
            {
                // TODO: create physical grocery bundle ala Link in BOTW, but a physical wobbling stack

                ParentPhysicalObjectToPlayer(closestGrocery, groceryBundleContainer.transform);

                isHoldingGroceries = true;
                interactor.validGroceryColliderList.Remove(closestGrocery.GetComponent<Collider>());
            }
        }
        else
        {
            Debug.Log("Player is maxed out at " + maxGroceriesHeldByPlayer + " groceries!");
            // trigger UI "You can't carry anymore!"
            return;
        }
    }
    public void PlaceGroceriesInCart(GameObject cartToPlaceGroceriesIn)
    {
        ShoppingCart shoppingCartScript = interactor.closestInteractible_ShoppingCart.InteractableShoppingCartScript;

        shoppingCartScript.AddGroceriesToCart(groceriesHeldByPlayer);
        GameManager.instance._GroceryListManager.AddAquiredGroceryItems(shoppingCartScript.containedGroceryGOs);
        groceriesHeldByPlayer.Clear();
        
        // shoppingCartScript.UpdateCartUI(true, );

        rightHandGrocery = null;
        leftHandGrocery = null;
        isHoldingGroceries = false;
    }
    
    public void GrabCart(GameObject goCart)
    {
        heldCart = goCart;

        if (heldCart != null)
        {
            GameManager.instance._GroceryListManager.RemovedAllItemsFromCart();
            GameManager.instance._GroceryListManager.AddAquiredGroceryItems(heldCart.GetComponent<ShoppingCart>().containedGroceryGOs);

            ParentPhysicalObjectToPlayer(heldCart, shoppingCart_positionHolder.transform);
            interactor.validShoppingCartColliderList.Remove(heldCart.GetComponent<Collider>());
            
            ikControl.leftHandObj = heldCart.gameObject.transform.Find("L_Hand_IK");
            ikControl.rightHandObj = heldCart.gameObject.transform.Find("R_Hand_IK");
            ikControl.ikActive = true;

            cartsInFrontOfPlayer.Clear();

            isHoldingCart = true;

            // myTempCart.UpdateCartUI(false,0f);

            // Debug.Log("Grabbed a Cart!");
        }
    }
    
    public void ReleaseCart()
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
            go.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer = false;
            UnParentPhysicalObjectFromPlayer(go);
            go.GetComponent<Rigidbody>().AddForce(transform.forward * releaseSpeed, ForceMode.VelocityChange);
            // groceriesHeldByPlayer.Remove(go);
        }

        groceriesHeldByPlayer.Clear();
        // onGroceryCountUpdate.Raise(groceriesHeldByPlayer.Count);
        isHoldingGroceries = false;
        
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
    
    
    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.CompareTag("Grocery") & other.gameObject.GetComponent<GroceryItem>())
        // {
        //     if (other.gameObject.GetComponent<GroceryItem>().thisGrocery.isInPlayersCart
        //         || other.gameObject.GetComponent<GroceryItem>().thisGrocery.isHeldByPlayer
        //         || groceriesInFrontOfPlayer.Contains(other.gameObject))
        //     {
        //         return;
        //     }
        //     else
        //     {
        //         groceriesInFrontOfPlayer.Add(other.gameObject);
        //         closestGrocery = FindClosestGO(groceriesInFrontOfPlayer);
        //     }
        // }

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
        // if (other.gameObject.CompareTag("Grocery"))
        // {
        //     other.GetComponent<ObjectGlow>().SetGlow(false);
        //     groceriesInFrontOfPlayer.Remove(other.gameObject);
        //     groceriesInFrontOfPlayer.TrimExcess();
        // }
        
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

    // TODO: OnTriggerStay
    private void OnTriggerStay(Collider other)
    {
        // check if the list of close game objects contains this gameobject, if not add it
        
        // find the closest:
        // Grocery
        // Shopping Cart
        // Grocery Stand
        
        // determine ACTIVELY CLOSEST OBJECT based on priority, there can be ONLY ONE
        // HIGHLIGHT GLOW on closest object
        // Update VERB UI e.g. "X -> Grab Bananas" "X -> Put Bananas in Cart" or "X -> Grab Cart"
    }
}
