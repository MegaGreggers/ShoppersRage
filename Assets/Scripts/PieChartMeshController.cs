﻿using UnityEngine;

public class PieChartMeshController : MonoBehaviour
{
    PieChartMesh mPieChart;
    float[] mData;
    public float delay = 0.1f;
    public Material mainMaterial;
    public Material[] materials;
    public int segments;
    private int randomBuffer;

    void Start()
    {
        randomBuffer = Random.seed;
        Random.seed = 10;

        materials = new Material[segments];
        for (int i = 0; i < segments; i++)
        {
            materials[i] = new Material(mainMaterial);
            materials[i].color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)255);
            Debug.LogError(materials[i].color);
        }

        mPieChart = gameObject.AddComponent<PieChartMesh>() as PieChartMesh;
        if (mPieChart != null)
        {
            mPieChart.Init(mData, 100, 0, 100, materials, delay);
            mData = GenerateRandomValues(segments);
            mPieChart.Draw(mData);
        }
        Random.seed = randomBuffer;
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            randomBuffer = Random.seed;
            Random.seed = 10;
            mData = GenerateRandomValues(segments);
            mPieChart.Draw(mData);
            Random.seed = randomBuffer;
        }
    }

    float[] GenerateRandomValues(int length)
    {
        float[] targets = new float[length];

        for (int i = 0; i < length; i++)
        {
            targets[i] = Random.Range(0f, 100f);
        }
        return targets;
    }
}