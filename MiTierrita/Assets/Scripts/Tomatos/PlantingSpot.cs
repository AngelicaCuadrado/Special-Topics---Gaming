using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    private TomatoPlant currentPlant;
    public bool HasPlant => currentPlant != null;

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
            Debug.Log("Planting Spot Planted");
            GameObject newPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
            currentPlant = newPlant.GetComponent<TomatoPlant>();

            currentPlant.originSpot = this;
        }
        if(AudioManager.Instance != null)AudioManager.Instance.PlaySFX(AudioManager.Instance.plantingSound);
    }

    public void ClearPlant()
    {
        currentPlant = null;
    }
}