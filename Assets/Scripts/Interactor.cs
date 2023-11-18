using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]

public class Interactor : MonoBehaviour
{
    private PlayerCartController _playerCartController;
    
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private float _interactionPointHeight = 2f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    
    [SerializeField] private Collider[] _colliders = new Collider[20];
    public List<Collider> validGroceryColliderList = new List<Collider>();
    public List<Collider> validShoppingCartColliderList = new List<Collider>();
    public List<Collider> previousValidGroceryColliderList = new List<Collider>();
    public List<Collider> previousValidShoppingCartColliderList = new List<Collider>();
    [SerializeField] public IInteractable closestInteractible_GroceryItem;
    [SerializeField] public IInteractable closestInteractible_ShoppingCart;
    [SerializeField] public GameObject closestGameObject_GroceryItem;
    [SerializeField] public GameObject closestGameObject_ShoppingCart;
    [SerializeField] public GameObject closestGameObject_InteractableGO;
    [SerializeField] private int _numFound;
    
    bool groceriesInRange = false;
    bool shoppingCartsInRange = false;
    
    private IInteractable _interactable;
    private IInteractable _previousInteractable = null;

    private void Start()
    {
        _playerCartController = GetComponent<PlayerCartController>();
        closestGameObject_GroceryItem = null;
        closestGameObject_ShoppingCart = null;
    }

    void Update()
    {
        if (_playerCartController.isHoldingCart)
            return;
        
        _numFound = Physics.OverlapCapsuleNonAlloc(_interactionPoint.position, _interactionPoint.position + Vector3.up*_interactionPointHeight, _interactionPointRadius,
            _colliders, _interactableMask);
        // _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius,
        //     _colliders, _interactableMask);

        previousValidGroceryColliderList = validGroceryColliderList;
        previousValidShoppingCartColliderList = validShoppingCartColliderList;

        validGroceryColliderList.Clear();
        validShoppingCartColliderList.Clear();
        
        // closestInteractible_GroceryItem = null;
        // closestInteractible_ShoppingCart = null;
        closestGameObject_GroceryItem = null;
        closestGameObject_ShoppingCart = null;
        
        
        //TODO: Controlling the glow script
        
        if (_numFound > 0)
        {
            foreach (Collider col in _colliders)
            {
                if (col != null)
                {
                    if (col.gameObject.CompareTag("ShoppingCart"))
                    {
                        if (!validShoppingCartColliderList.Contains(col))
                            validShoppingCartColliderList.Add(col);
                    }

                    if (col.gameObject.CompareTag("Grocery"))
                    {
                        if (!validGroceryColliderList.Contains(col))
                        {
                            GroceryItem thisItem = col.gameObject.GetComponent<GroceryItem>();
                            // we don't want to interact with objects already held by the player or in the players cart
                            if (thisItem.thisGrocery.isHeldByPlayer || thisItem.thisGrocery.isInPlayersCart)
                                return;
                        
                            validGroceryColliderList.Add(col);
                        }
                    }
                }
            }
                
            if (validGroceryColliderList.Count > 0)
            {
                closestInteractible_GroceryItem =
                    FindClosestCollider(validGroceryColliderList).GetComponent<IInteractable>();
                closestGameObject_GroceryItem = closestInteractible_GroceryItem.InteractableGameObject;
            }

            if (validShoppingCartColliderList.Count > 0)
            {
                closestInteractible_ShoppingCart =
                    FindClosestCollider(validShoppingCartColliderList).GetComponent<IInteractable>();
                closestGameObject_ShoppingCart = closestInteractible_ShoppingCart.InteractableGameObject;
            }

            if (closestInteractible_ShoppingCart != null)
            {
                _interactable = closestInteractible_ShoppingCart;
                closestGameObject_ShoppingCart = closestInteractible_ShoppingCart.InteractableGameObject;
            }
            else if (closestInteractible_GroceryItem != null)
            {
                _interactable = closestInteractible_GroceryItem;
                closestGameObject_GroceryItem = closestInteractible_GroceryItem.InteractableGameObject;
            }

            if (_interactable != null)
            {
                _previousInteractable = _interactable;
                if (!_interactionPromptUI.IsDisplayed) _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                _interactable.InteractionObjectGlowScript.SetGlow(true);
                closestGameObject_InteractableGO = _interactable.InteractableGameObject;

                if (Input.GetButtonDown("X_1"))
                {
                    _interactable.InteractionObjectGlowScript.SetGlow(false);
                    _interactable.Interact(this);
                    if (_interactionPromptUI.IsDisplayed) _interactionPromptUI.Close();
                }
                
                _interactable = null;
            }
        }
        else
        {
            if (_interactable != null)
            {
                _interactable.InteractionObjectGlowScript.SetGlow(false);
                _interactable = null;
            }

            if (_interactionPromptUI.IsDisplayed) _interactionPromptUI.Close();
            
            if (closestInteractible_ShoppingCart != null)
            {
                closestInteractible_ShoppingCart.InteractionObjectGlowScript.SetGlow(false);
                closestInteractible_ShoppingCart = null;
            }
            if (closestInteractible_GroceryItem != null)
            {
                closestInteractible_GroceryItem.InteractionObjectGlowScript.SetGlow(false);
                closestInteractible_GroceryItem = null;
            }
        }
        
        if (validShoppingCartColliderList.Count == 0)
        {
            closestInteractible_ShoppingCart = null;
        }
        if (validGroceryColliderList.Count == 0)
        {
            closestInteractible_GroceryItem = null;
        }
        
        for (int i = 0; i < previousValidGroceryColliderList.Count; i++)
        {
            if(!validGroceryColliderList.Contains(previousValidGroceryColliderList[i]))
                previousValidGroceryColliderList[i].GetComponent<GroceryItem>().thisGrocery.myGlowScript.SetGlow(false);
        }
        for (int i = 0; i < previousValidShoppingCartColliderList.Count; i++)
        {
            if(!validShoppingCartColliderList.Contains(previousValidShoppingCartColliderList[i]))
                previousValidShoppingCartColliderList[i].GetComponent<ShoppingCart>().myGlowScript.SetGlow(false);
        }
        
        Array.Clear(_colliders, 0, 20);
    }

    private Collider FindClosestCollider(List<Collider> colliders)
    {
        colliders.TrimExcess();
        float closestDistance = Vector3.Distance(gameObject.transform.position, colliders[0].transform.position);
        Collider closestCol = colliders[0];
        

        for (int i = 0; i < colliders.Count; i++)
        {
            float distanceToObject = Vector3.Distance(gameObject.transform.position, colliders[i].transform.position);
            if (distanceToObject < closestDistance)
            {
                closestDistance = distanceToObject;
                closestCol = colliders[i];
            }
            
            colliders[i].GetComponent<IInteractable>().InteractionObjectGlowScript.SetGlow(false);
        }
        
        // closestCol.GetComponent<GroceryItem>().thisGrocery.myGlowScript.SetGlow(true);
        
        return closestCol;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        Gizmos.DrawWireSphere(_interactionPoint.position + Vector3.up*_interactionPointHeight, _interactionPointRadius);
    }
}
