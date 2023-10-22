using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPieChart : MonoBehaviour
{
    public PieChartController _pieChartController;
    
    public void GenerateRandomPieChart()
    {
        float[] values = new float[3];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = Random.Range(0.0f, 100.0f);
        }
        
        GetComponent<PieChartController>().SetValues(values);
    }
}
