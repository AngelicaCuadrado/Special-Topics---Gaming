using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ControlMode
    {
        Manual,
        Simulated
    }
    [Header("Mode")]
    public ControlMode controlMode = ControlMode.Simulated;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Simulation Settings")]
    public Transform targetAgent; // the AI we want to chase
    public float chaseDistance = 10f;
    public float randomMoveRadius = 5f;

    private Rigidbody rb;
    private Vector3 simulatedDirection;
    private float changeDirTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (controlMode)
        {
            case ControlMode.Manual:
                HandleManual();
                break;

            case ControlMode.Simulated:
                HandleSimulated();
                break;
        }
    }

    private void HandleManual() 
    {
        //To Be Implamented, not sure if needed
    }

    private void HandleSimulated()
    {
        if (targetAgent == null)
        {
            RandomWander();
            return;
        }

        float dist = Vector3.Distance(transform.position, targetAgent.position);

        if (dist < chaseDistance)
        {
            // Chase agent
            Vector3 dir = (targetAgent.position - transform.position).normalized;
            Move(dir);
        }
        else
        {
            // Wander
            RandomWander();
        }
    }

    private void RandomWander()
    {
        changeDirTimer -= Time.deltaTime;

        if (changeDirTimer <= 0f)
        {
            simulatedDirection = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized;

            changeDirTimer = Random.Range(1f, 3f);
        }

        Move(simulatedDirection);
    }

    private void Move(Vector3 direction)
    {
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
    }
}
