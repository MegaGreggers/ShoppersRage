using System.Collections;
using System.Collections.Generic;
using CreatingCharacters.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private VoidEvent onGameStart;
    public enum GameScreens { Intro, TitleScreen, Gameplay, Results };
    public GameScreens currentGameScreen = GameScreens.Gameplay;
    public GameObject AIShopperPrefab;

    public enum allGroceryTypes { Cheerios, Bananas, Milk, Yogurt, Grapes, Watermelon, Pumpkin };

    // [Header("UI Player Directives")]
    public enum UI_PlayerDirectives { Checkout, GrabCart, GrabGrocery, HurryUp };
    public GameObject UI_GetToCheckout;

    public bool useAISpawner = true;
    public int numAIShoppers = 20;

    private int numCurrentShoppersActive = 0;

    public List<GameObject> spawnPoints;
    public List<GameObject> actionPoints;
    public List<GameObject> checkoutPoints;

    public PlayerCartController _PlayerCartController;
    public GroceryListManager _GroceryListManager;
    public CountdownTimer _CountdownTimer;
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        _PlayerCartController = FindObjectOfType<PlayerCartController>();
        _GroceryListManager = FindObjectOfType<GroceryListManager>();
        _CountdownTimer = FindObjectOfType<CountdownTimer>();
        
        onGameStart.Raise();

        GameObject waypointsParent = transform.Find("_Waypoints").gameObject;
        GameObject spawnPointParent = waypointsParent.transform.Find("SpawnPoints").gameObject;
        GameObject actionPointParent = waypointsParent.transform.Find("ActionPoints").gameObject;
        GameObject checkoutPointParent = waypointsParent.transform.Find("CheckoutPoints").gameObject;

        for ( int i = 0; i < spawnPointParent.transform.childCount; i++)
        {
            spawnPoints.Add(spawnPointParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < actionPointParent.transform.childCount; i++)
        {
            actionPoints.Add(actionPointParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < checkoutPointParent.transform.childCount; i++)
        {
            checkoutPoints.Add(checkoutPointParent.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (useAISpawner)
        {
            RunAIShoppers();
        }
        
    }

    void RunAIShoppers()
    {
        // spawn shoppers at random spawn points
        if (numCurrentShoppersActive < numAIShoppers)
        {
            for (int i = 0; i < numAIShoppers; i++)
            {
                // Instantiating Shoppers at random point is causing asserts - change to only move the root of the character
                // Try Instantiating and then setting position on Character_Meshes
                // GameObject newShopper = Instantiate(AIShopperPrefab, GetRandomChildGameObject(spawnPoints).transform.position, Quaternion.identity);

                GameObject newShopper = Instantiate(AIShopperPrefab, transform.localPosition, Quaternion.identity) as GameObject;
                GameObject charMesh = newShopper.transform.Find("Character_Meshes").gameObject;
                charMesh.transform.position = GetRandomChildGameObject(spawnPoints).transform.position;



                foreach (Transform child in charMesh.transform)
                    child.gameObject.SetActive(false);
                GetRandomChildGameObject(charMesh).SetActive(true);

                numCurrentShoppersActive++;
            }
        }
    }
    
    
    GameObject GetRandomChildGameObject(List<GameObject> waypointParent)
    {
        int randomIndex = Random.Range(0, waypointParent.Count);
        GameObject randomGO = waypointParent[randomIndex];
        return randomGO;
    }

    GameObject GetRandomChildGameObject(GameObject waypointParent)
    {
        List<GameObject> childGOs = new List<GameObject>();

        for (int i = 0; i < waypointParent.transform.childCount; i++)
        {
            childGOs.Add(waypointParent.transform.GetChild(i).gameObject);
        }

        int randomIndex = Random.Range(0, waypointParent.transform.childCount);
        GameObject randomGO = waypointParent.transform.GetChild(randomIndex).gameObject;
        return randomGO;
    }

    public void SetUIPlayerDirective(UI_PlayerDirectives directive, bool enabled)
    {
        switch (directive)
        {
            case (UI_PlayerDirectives.Checkout):
                UI_GetToCheckout.SetActive(enabled);
                return;
        }
    }

    public void LevelCompleted()
    {
        _CountdownTimer.LevelCompleted();
    }
}
