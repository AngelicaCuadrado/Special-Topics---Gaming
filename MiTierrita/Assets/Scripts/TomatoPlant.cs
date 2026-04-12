using System;
using UnityEngine;

public enum GrowthStage
{
    Seed,
    Sprout,
    Sapling,
    Green,
    Ripe
}

public class TomatoPlant : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float growth = 0f;
    [SerializeField] private float growthRate = 5f;      // growth per second

    public GrowthStage growthStage;

    [Header("Models (Order matters)")]
    // 0 = Seed, 1 = Sprout, 2 = Sapling, 3 = Green, 4 = Ripe
    public GameObject[] models;

    private GrowthStage previousStage;

    void Start()
    {
        UpdateGrowthStage();
        UpdateModel();
    }

    void Update()
    {
        HandleGrowth();
        UpdateGrowthStage();

        if (growthStage != previousStage)
        {
            UpdateModel();
            previousStage = growthStage;
        }
    }

    private void HandleGrowth()
    {
        if (growth >= 100f) return;

        growth += growthRate * Time.deltaTime;
        growth = Mathf.Clamp(growth, 0f, 100f);
    }

    private void UpdateGrowthStage()
    {
        if (growth < 25f)
            growthStage = GrowthStage.Seed;
        else if (growth < 50f)
            growthStage = GrowthStage.Sprout;
        else if (growth < 75f)
            growthStage = GrowthStage.Sapling;
        else if (growth < 100f)
            growthStage = GrowthStage.Green;
        else
            growthStage = GrowthStage.Ripe;
    }

    private void UpdateModel()
    {
        // Disable all models first
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(false);
        }

        // Enable the correct one
        models[(int)growthStage].SetActive(true);
    }
}
