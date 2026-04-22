using UnityEngine;

public class TruckSpawner : MonoBehaviour
{
    public static TruckSpawner Instance { get; private set; }

    [Tooltip("El prefab de tu camión completo (con los sockets)")]
    public GameObject truckPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnTruck();
    }

    public void SpawnTruck()
    {
        if (truckPrefab != null)
        {
            Instantiate(truckPrefab, transform.position, transform.rotation);
        }
    }
}