using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SeedBag : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí tu Prefab de la semilla (debe tener XR Grab Interactable)")]
    [SerializeField] private GameObject seedPrefab;
    public void ExtractSeed(SelectEnterEventArgs args)
    {
        GameObject newSeed = Instantiate(
            seedPrefab,
            args.interactorObject.transform.position,
            args.interactorObject.transform.rotation
        );

        XRGrabInteractable grab = newSeed.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            StartCoroutine(AttachNextFrame((IXRSelectInteractor)args.interactorObject, grab));
        }
    }

    private IEnumerator AttachNextFrame(IXRSelectInteractor interactor, XRGrabInteractable grab)
    {
        yield return null;

        var manager = grab.interactionManager;
        if (manager != null)
        {
            manager.SelectEnter(interactor, (IXRSelectInteractable)grab);
        }
    }
}