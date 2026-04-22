using UnityEngine;
using System.Linq;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField]private PlantingSpot[] spots;

    private void Awake()
    {
        spots = FindObjectsByType<PlantingSpot>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        for (int i = 0; i < 5; i++) 
        {
            spots[i].Plant();
        }
    }

    public void SpawnPlant()
    {
        // Find an empty planting spot
        PlantingSpot emptySpot = spots.FirstOrDefault(s => s.HasPlant == false);

        if (emptySpot != null)
        {
            Debug.Log("Spawner Planted");
            emptySpot.Plant();
        }
        else
        {
            Debug.LogWarning("No empty planting spots available!");
        }
    }
}
