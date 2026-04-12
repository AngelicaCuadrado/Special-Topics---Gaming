using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;

    private TomatoPlant currentPlant;

    public void Plant() 
    {
        if (currentPlant == null) 
        {
            GameObject newPLant = Instantiate(plantPrefab);
            currentPlant = newPLant.GetComponent<TomatoPlant>();
        }
    }

    public void Harvest() 
    {
        Destroy(currentPlant.gameObject);
        currentPlant = null;
    }
}
