using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    private TomatoPlant currentPlant;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seed") && currentPlant == null)
        {
            Destroy(other.gameObject);
            Plant();
        }
    }

    public void Plant()
    {
        if (currentPlant == null)
        {
            GameObject newPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
            currentPlant = newPlant.GetComponent<TomatoPlant>();
        }
    }
}