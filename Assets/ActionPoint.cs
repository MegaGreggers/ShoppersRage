using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoint : MonoBehaviour
{
    public Animation actionPoint_animation;
    public bool actionIsLooping;
    public float actionDuration;

    public GameObject thisGroceryItem;

    public enum actionTypes { grabGrocery_Fwd, scanGroceries };
    public actionTypes actionType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
