﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGlow : MonoBehaviour
{
    private GameObject glowObject;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.transform.childCount > 0)
        {
            foreach (Transform t in GetComponentInChildren<Transform>())
            {
                if (t.gameObject.name == "glow")
                {
                    glowObject = t.gameObject;
                }
            }

            SetGlow(false);
        }

        if(glowObject == null)
            Debug.LogWarning("Glow object not found.");
    }

    public void SetGlow(bool glow)
    {
        glowObject.SetActive(glow);
    }
}
