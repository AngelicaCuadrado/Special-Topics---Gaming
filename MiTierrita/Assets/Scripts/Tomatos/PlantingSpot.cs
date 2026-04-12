using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    private TomatoPlant currentPlant;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚠️ ¡Algo entró en la tierra! Objeto: " + other.gameObject.name + " | Tag: " + other.tag);

        if (other.CompareTag("Seed"))
        {
            if (currentPlant == null)
            {
                Debug.Log("🌱 ¡Semilla correcta detectada! Plantando...");
                Destroy(other.gameObject);
                Plant();
            }
            else
            {
                Debug.Log("❌ Cayó una semilla, pero YA HAY una planta aquí.");
            }
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