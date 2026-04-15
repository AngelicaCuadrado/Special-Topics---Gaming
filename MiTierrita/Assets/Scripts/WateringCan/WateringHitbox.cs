using UnityEngine;

public class WateringHitbox : MonoBehaviour
{
    [Tooltip("Cuánta agua añade a la planta por segundo")]
    public float waterPower = 30f;

    private void OnTriggerStay(Collider other)
    {
        TomatoPlant plant = other.GetComponent<TomatoPlant>();

        if (plant != null)
        {
            plant.AddWater(waterPower * Time.deltaTime);
        }
    }
}