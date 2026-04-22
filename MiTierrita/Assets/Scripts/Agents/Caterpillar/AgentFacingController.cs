using UnityEngine;

public class AgentFacingController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Ignore tiny movements
        velocity.y = 0f;
        if (velocity.sqrMagnitude < 0.001f)
            return;

        // Determine the direction to face
        Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);

        // Smoothly rotate toward movement direction
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
