using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BasketSpawner : MonoBehaviour
{
    [Header("Configuración del Prefab")]
    public GameObject basketPrefab;
    public Transform spawnPoint;

    [Header("Sockets del Camión")]
    public XRSocketInteractor[] truckSockets;

    void Start()
    {
        foreach (var socket in truckSockets)
        {
            if (socket != null)
                socket.selectEntered.AddListener(OnBasketPlaced);
        }

        SpawnNewBasket();
    }

    private void OnBasketPlaced(SelectEnterEventArgs args)
    {
        Invoke("SpawnNewBasket", 0.5f);
    }

    public void SpawnNewBasket()
    {
        if (basketPrefab != null && spawnPoint != null)
        {
            Instantiate(basketPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Nueva canasta lista para llenar.");
        }
        else
        {
            Debug.LogError("Faltan referencias en el BasketSpawner (Prefab o SpawnPoint).");
        }
    }

    private void OnDestroy()
    {
        foreach (var socket in truckSockets)
        {
            if (socket != null)
                socket.selectEntered.RemoveListener(OnBasketPlaced);
        }
    }
}