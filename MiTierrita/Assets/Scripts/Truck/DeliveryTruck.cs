using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DeliveryTruck : MonoBehaviour
{
    [Header("Sockets Configuration")]
    [Tooltip("Arrastra aquí los 4 XR Socket Interactors que están en la parte trasera del camión")]
    public XRSocketInteractor[] basketSockets;

    [Header("Movement Configuration")]
    public float moveSpeed = 3f;
    [Tooltip("Dirección hacia la que se moverá el camión (Local)")]
    public Vector3 moveDirection = Vector3.forward;

    private bool isFullAndMoving = false;

    void OnEnable()
    {
        foreach (var socket in basketSockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener(OnBasketPlaced);
                socket.selectExited.AddListener(OnBasketRemoved);
            }
        }
    }

    void OnDisable()
    {
        foreach (var socket in basketSockets)
        {
            if (socket != null)
            {
                socket.selectEntered.RemoveListener(OnBasketPlaced);
                socket.selectExited.RemoveListener(OnBasketRemoved);
            }
        }
    }

    private void OnBasketPlaced(SelectEnterEventArgs args) { CheckSockets(); }
    private void OnBasketRemoved(SelectExitEventArgs args) { CheckSockets(); }

    private void CheckSockets()
    {
        if (isFullAndMoving) return;

        int filledCount = 0;
        foreach (var socket in basketSockets)
        {
            if (socket.hasSelection) filledCount++;
        }

        if (filledCount >= basketSockets.Length)
        {
            StartMoving();
        }
    }

    private void StartMoving()
    {
        isFullAndMoving = true;
    }

    void Update()
    {
        if (isFullAndMoving)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TruckExit"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TruckDispatched();
            }

            if (TruckSpawner.Instance != null)
            {
                TruckSpawner.Instance.SpawnTruck();
            }

            Destroy(gameObject);
        }
    }
}