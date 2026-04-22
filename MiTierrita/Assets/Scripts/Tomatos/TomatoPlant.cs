using System;
using System.Collections;
using Unity.AppUI.UI;
using Unity.MLAgents;
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
    [SerializeField] private float growthRate = 5f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem _seedParticlePrefab;
    [SerializeField] private Transform _particleSpawnPoint;

    [Header("Water Settings")]
    public float currentWaterLevel = 0f;
    public float maxWaterLevel = 100f;
    public float waterDepletionRate = 2f;

    public GrowthStage growthStage;

    public bool IsRipeEnough => growthStage >= GrowthStage.Sapling;

    [Header("Models (Order matters)")]
    public GameObject[] models;

    private GrowthStage previousStage;
    private XRSimpleInteractable interactable;

    [SerializeField] private GameObject tomatoFruitPrefab;

    [SerializeField] private bool AutoWater;

    [HideInInspector] public PlantingSpot originSpot;

    private PlantSpawner spawner;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnInteract);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnInteract);
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;

        if (interactor != null)
        {
            Harvest(interactor);
        }
    }

    void Start()
    {
        spawner = FindFirstObjectByType<PlantSpawner>();
        if (Application.isPlaying && Academy.Instance.IsCommunicatorOn) AutoWater = true;
        UpdateGrowthStage();
        UpdateModel();
        previousStage = growthStage;

        if (growthStage == GrowthStage.Seed)
        {
            PlaySeedParticle();
        }
    }

    void Update()
    {

        HandleGrowth();
        UpdateGrowthStage();

        if (growthStage != previousStage)
        {
            UpdateModel();

            if (growthStage == GrowthStage.Seed)
            {
                PlaySeedParticle();
            }

            previousStage = growthStage;
        }
    }

    private void HandleGrowth()
    {
        if (growth >= 100f) return;

        if (currentWaterLevel > 0 || AutoWater == true)
        {
            growth += growthRate * Time.deltaTime;
            growth = Mathf.Clamp(growth, 0f, 100f);
            currentWaterLevel -= waterDepletionRate * Time.deltaTime;
            if (currentWaterLevel < 0) currentWaterLevel = 0;
        }
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
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i] != null)
                models[i].SetActive(false);
        }

        if (models[(int)growthStage] != null)
            models[(int)growthStage].SetActive(true);
    }

    public void Harvest(XRBaseInteractor interactor)
    {
        if (growthStage == GrowthStage.Sapling || growthStage == GrowthStage.Green)
        {
            if (originSpot != null)originSpot.ClearPlant();

            Destroy(gameObject);
            return;
        }

        if (growthStage != GrowthStage.Ripe) return;

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

        if (originSpot != null)
            originSpot.ClearPlant();
        Destroy(gameObject);
    }

    private IEnumerator AttachNextFrame(XRBaseInteractor interactor, XRGrabInteractable grab)
    {
        yield return null;

        var manager = grab.interactionManager;

        if (manager != null)
        {
            manager.SelectEnter(
                (IXRSelectInteractor)interactor,
                (IXRSelectInteractable)grab
            );
        }
    }

    public void AddWater(float amount)
    {
        currentWaterLevel += amount;
        currentWaterLevel = Mathf.Clamp(currentWaterLevel, 0f, maxWaterLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        CaterpillarAgent agent = other.GetComponent<CaterpillarAgent>();

        if (agent != null && growthStage >= GrowthStage.Sapling)
        {
            agent.AddReward(1.0f);
            if (spawner != null)
                spawner.SpawnPlant();

            if (originSpot != null)
                originSpot.ClearPlant();
            Destroy(gameObject);
        }
    }


    private void PlaySeedParticle() 
    {
        if (_seedParticlePrefab == null) return;

        // position of the particle system will be either the specified spawn point or the plant's position
        Vector3 pos = _particleSpawnPoint != null ? _particleSpawnPoint.position : transform.position;
        
        // instantiate the particle system at the determined position
        ParticleSystem ps = Instantiate(_seedParticlePrefab, pos, Quaternion.identity);
        Destroy(ps.gameObject, 2f);
    }
}