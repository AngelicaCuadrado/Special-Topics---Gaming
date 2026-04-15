using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CaterpillarSpray : MonoBehaviour
{
    [Header("Configuración de Uso")]
    public float maxCapacity = 5f;
    public float rechargeSpeed = 0.5f;
    private float currentCapacity;

    [Header("Referencias")]
    public XRGrabInteractable grabInteractable;
    public ParticleSystem sprayParticles;
    public GameObject sprayHitbox;
    public AudioSource sprayAudio;

    [Header("UI del Objeto")]
    public GameObject uiCanvas;
    public Image fillBar;

    private bool isSpraying = false;

    void Start()
    {
        currentCapacity = maxCapacity;
        uiCanvas.SetActive(false);
        sprayHitbox.SetActive(false);

        if (sprayAudio != null) sprayAudio.Stop();

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.activated.AddListener(StartSpraying);
        grabInteractable.deactivated.AddListener(StopSpraying);
    }

    void Update()
    {
        HandleCooldownAndRecharge();
        UpdateUI();
    }

    private void StartSpraying(ActivateEventArgs arg)
    {
        if (currentCapacity > 0)
        {
            isSpraying = true;
            sprayParticles.Play();
            sprayHitbox.SetActive(true);

            if (sprayAudio != null && !sprayAudio.isPlaying)
                sprayAudio.Play();
        }
    }

    private void StopSpraying(DeactivateEventArgs arg)
    {
        isSpraying = false;
        sprayParticles.Stop();
        sprayHitbox.SetActive(false);

        if (sprayAudio != null)
            sprayAudio.Stop();
    }

    private void HandleCooldownAndRecharge()
    {
        if (isSpraying)
        {
            currentCapacity -= Time.deltaTime;
            if (currentCapacity <= 0)
            {
                currentCapacity = 0;
                StopSpraying(null);
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
        uiCanvas.SetActive(isSpraying || currentCapacity < maxCapacity);
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(StartSpraying);
        grabInteractable.deactivated.RemoveListener(StopSpraying);
    }
}