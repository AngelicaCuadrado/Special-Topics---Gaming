using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
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

    [SerializeField] private GameObject tomatoFruitPrefab;

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

    public void Harvest(XRBaseInteractor interactor)
    {
        if (growthStage != GrowthStage.Ripe) return;

        // Spawn fruit at hand position
        GameObject fruit = Instantiate(
            tomatoFruitPrefab,
            interactor.transform.position,
            interactor.transform.rotation
        );

        XRGrabInteractable grab = fruit.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            StartCoroutine(AttachNextFrame(interactor, grab));
        }

        // Destroy AFTER scheduling grab
        Destroy(gameObject);
    }

    private IEnumerator AttachNextFrame(XRBaseInteractor interactor, XRGrabInteractable grab)
    {
        yield return null; // wait one frame

        var manager = grab.interactionManager;

        if (manager != null)
        {
            manager.SelectEnter(
                (IXRSelectInteractor)interactor,
                (IXRSelectInteractable)grab
            );
        }
    }
}
