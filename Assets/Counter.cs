using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    TextMeshPro counterText;
    // Start is called before the first frame update
    void Awake()
    {
        counterText = GetComponent<TextMeshPro>();
    }

    public void UpdateTextMesh (int i)
    {
        counterText.text = i.ToString();
        // Debug.Log(i.ToString());
    }
}
