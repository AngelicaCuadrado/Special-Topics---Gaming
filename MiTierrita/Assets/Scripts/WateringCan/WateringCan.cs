using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WateringCan : MonoBehaviour
{
    [Header("Configuración de Uso")]
    public float maxCapacity = 5f;
    public float rechargeSpeed = 0.5f;
    private float currentCapacity;

    [Header("Referencias")]
    public XRGrabInteractable grabInteractable;
    public ParticleSystem waterParticles;
    public GameObject waterHitbox;
    public AudioSource waterAudio;

    [Header("UI del Objeto")]
    public GameObject uiCanvas;
    public Image fillBar;

    private bool isWatering = false;

    void Start()
    {
        currentCapacity = maxCapacity;
        uiCanvas.SetActive(false);
        waterHitbox.SetActive(false);

        if (waterAudio != null) waterAudio.Stop();

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.activated.AddListener(StartWatering);
        grabInteractable.deactivated.AddListener(StopWatering);
    }

    void Update()
    {
        HandleCooldownAndRecharge();
        UpdateUI();
    }

    private void StartWatering(ActivateEventArgs arg)
    {
        if (currentCapacity > 0)
        {
            isWatering = true;
            waterParticles.Play();
            waterHitbox.SetActive(true);

            if (waterAudio != null && !waterAudio.isPlaying)
                waterAudio.Play();
        }
    }

    private void StopWatering(DeactivateEventArgs arg)
    {
        isWatering = false;
        waterParticles.Stop();
        waterHitbox.SetActive(false);

        if (waterAudio != null)
            waterAudio.Stop();
    }

    private void HandleCooldownAndRecharge()
    {
        if (isWatering)
        {
            currentCapacity -= Time.deltaTime;
            if (currentCapacity <= 0)
            {
                currentCapacity = 0;
                StopWatering(null);
            }
        }
        else
        {
            if (currentCapacity < maxCapacity)
            {
                currentCapacity += (Time.deltaTime * rechargeSpeed);
                if (currentCapacity > maxCapacity)
                    currentCapacity = maxCapacity;
            }
        }
    }

    private void UpdateUI()
    {
        if (fillBar != null) fillBar.fillAmount = currentCapacity / maxCapacity;
        uiCanvas.SetActive(isWatering || currentCapacity < maxCapacity);
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(StartWatering);
        grabInteractable.deactivated.RemoveListener(StopWatering);
    }
}