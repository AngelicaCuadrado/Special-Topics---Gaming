using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabTransparency : MonoBehaviour
{
    [SerializeField] private Renderer[] targetRenderers;

    [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material transparentMaterial;

    [Header("Fade Settings")]
    [SerializeField] private float normalAlpha = 1f;
    [SerializeField] private float grabbedAlpha = 0.35f;
    [SerializeField] private float fadeSpeed = 6f;

    private XRGrabInteractable grabInteractable;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        ApplyMaterial(normalMaterial);
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        ApplyMaterial(transparentMaterial);
        SetAlpha(normalAlpha);
        StartFade(grabbedAlpha);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        StartFade(normalAlpha, true);
    }

    private void ApplyMaterial(Material mat)
    {
        if (mat == null) return;

        foreach (var rend in targetRenderers)
        {
            rend.material = mat;
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (var rend in targetRenderers)
        {
            if (!rend.material.HasProperty("_BaseColor")) continue;

            Color c = rend.material.GetColor("_BaseColor");
            c.a = alpha;
            rend.material.SetColor("_BaseColor", c);
        }
    }

    private void StartFade(float targetAlpha, bool swapBackWhenDone = false)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeAlpha(targetAlpha, swapBackWhenDone));
    }

    private IEnumerator FadeAlpha(float targetAlpha, bool swapBackWhenDone)
    {
        bool done = false;

        while (!done)
        {
            done = true;

            foreach (var rend in targetRenderers)
            {
                Material mat = rend.material;

                if (!mat.HasProperty("_BaseColor")) continue;

                Color c = mat.GetColor("_BaseColor");
                float newAlpha = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);

                if (Mathf.Abs(newAlpha - targetAlpha) > 0.01f)
                    done = false;

                c.a = newAlpha;
                mat.SetColor("_BaseColor", c);
            }

            yield return null;
        }

        SetAlpha(targetAlpha);

        if (swapBackWhenDone)
        {
            ApplyMaterial(normalMaterial);
        }
    }
}