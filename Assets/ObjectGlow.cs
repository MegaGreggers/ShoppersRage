using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGlow : MonoBehaviour
{
    private GameObject glowObject;
    // Start is called before the first frame update
    void Start()
    {
        FindGlowObject();
        SetGlow(false);
    }

    public void SetGlow(bool glow)
    {
        if (glowObject)
            glowObject.SetActive(glow);
        else
        {
            if(glowObject == null)
                Debug.LogWarning(gameObject.name + " could not find Glow object.");
        }
    }

    private void FindGlowObject()
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
        }
    }
}
