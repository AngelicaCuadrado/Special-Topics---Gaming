using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Basket : MonoBehaviour
{
    [Header("Visual Tomatoes fill:")]
    [SerializeField] private GameObject[] fillStages;

    [Header("Grab Configuration")]
    [SerializeField] private XRGrabInteractable basketGrab;

    private int currentTomatoCount = 0;
    private int maxTomatoes = 5;
    void Start()
    {
        foreach (GameObject stage in fillStages)
        {
            if (stage != null) stage.SetActive(false);
        }

        if (basketGrab != null)
        {
            basketGrab.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TomatoFruit droppedFruit = other.GetComponent<TomatoFruit>();

        if (droppedFruit != null)
        {
            if (currentTomatoCount < maxTomatoes)
            {
                fillStages[currentTomatoCount].SetActive(true);
                currentTomatoCount += droppedFruit.value;

                Destroy(other.gameObject);

                if (currentTomatoCount >= maxTomatoes)
                {
                    if (basketGrab != null) basketGrab.enabled = true;
                    Debug.Log("Basket full! You can now pick it up.");
                }
            }
            else
            {
                Debug.Log("This basket is already full, please move this tomato to another basket.");
            }
        }
    }
}