using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Basket : MonoBehaviour
{
    [Header("Visual Tomatoes fill:")]
    [SerializeField] private GameObject[] fillStages;

    [Header("Feedback Effects")]
    [Tooltip("Particle system")]
    [SerializeField] private ParticleSystem tomatoParticle;
    [Tooltip("Audio source for tomato sound")]
    [SerializeField] private AudioSource tomatoAudio;

    [Header("Grab Configuration")]
    [SerializeField] private XRGrabInteractable basketGrab;

    private int currentTomatoCount = 0;
    private int maxTomatoes = 3;

    void Start()
    {
        foreach (GameObject stage in fillStages)
        {
            if (stage != null) stage.SetActive(false);
        }

        if (basketGrab != null) basketGrab.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TomatoFruit droppedFruit = other.GetComponent<TomatoFruit>();

        if (droppedFruit != null)
        {
            if (currentTomatoCount < maxTomatoes)
            {
                if (tomatoParticle != null) tomatoParticle.Play();
                if (tomatoAudio != null) tomatoAudio.Play();

                fillStages[currentTomatoCount].SetActive(true);
                currentTomatoCount += droppedFruit.value;
                Destroy(other.gameObject);

                if (GameManager.Instance != null) GameManager.Instance.AddTomatoScore();

                if (currentTomatoCount >= maxTomatoes)
                {
                    if (GameManager.Instance != null) GameManager.Instance.BasketFilledScore();

                    Rigidbody rbPadre = GetComponentInParent<Rigidbody>();
                    if (rbPadre != null) rbPadre.isKinematic = false;

                    if (basketGrab != null) basketGrab.enabled = true;
                }
            }
        }
    }
}