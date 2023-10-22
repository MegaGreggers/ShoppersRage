using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartControllerSimple : MonoBehaviour
{
    public Image pieChartImage;
    public float startPercentage = 5f;

    private void Awake()
    {
        pieChartImage = GetComponent<Image>();
    }

    void Start()
    {
        SetValue(startPercentage);
    }

    public void SetValue(float percentage)
    {
        if(pieChartImage)
            pieChartImage.fillAmount = percentage;
        else
        {
            Debug.LogWarning(gameObject.name + " can't find it's piechartImage.");
        }
    }
}
