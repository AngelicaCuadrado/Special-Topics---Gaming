using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CaterpillarAgent : Agent
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    private Rigidbody rb;

    [Header("Environment")]
    public Transform player;
    public LayerMask plantLayer;

    [Header("Allowed Area")]
    public BoxCollider allowedArea;

    private TomatoPlant nearestPlant;
    private float previousDistance;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;

        // Spawn inside allowed area
        transform.localPosition = GetRandomPointInsideArea();

        nearestPlant = FindNearestRipePlant();
        previousDistance = nearestPlant != null
            ? Vector3.Distance(transform.localPosition, nearestPlant.transform.localPosition)
            : 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position
        sensor.AddObservation(transform.localPosition);

        // Player position
        sensor.AddObservation(player.localPosition);

        // Distance to player (helps avoidance)
        float playerDist = Vector3.Distance(transform.localPosition, player.localPosition);
        sensor.AddObservation(playerDist);

        // Nearest plant info
        nearestPlant = FindNearestRipePlant();
        if (nearestPlant != null)
        {
            Vector3 dir = (nearestPlant.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dir);

            float dist = Vector3.Distance(transform.localPosition, nearestPlant.transform.localPosition);
            sensor.AddObservation(dist);
        }
        else
        {
            sensor.AddObservation(Vector3.zero); // direction
            sensor.AddObservation(0f);           // distance
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        Vector3 move = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        rb.linearVelocity = move * moveSpeed;

        // Time penalty
        AddReward(-0.0005f);

        // Boundary avoidance
        if (!IsInsideAllowedArea(transform.localPosition))
        {
            AddReward(-1f);
            EndEpisode();
            return;
        }

        float distToEdge = DistanceToAreaEdge(transform.localPosition);
        AddReward(-Mathf.Clamp01(1f - distToEdge / 3f) * 0.001f);

        // Player avoidance shaping
        float playerDist = Vector3.Distance(transform.localPosition, player.localPosition);

        // Reward staying far from player
        AddReward(Mathf.Clamp(playerDist / 10f, 0f, 1f) * 0.001f);

        // Punish getting too close
        if (playerDist < 2f)
            AddReward(-0.01f);

        // Plant seeking
        if (nearestPlant != null)
        {
            float newDistance = Vector3.Distance(transform.localPosition, nearestPlant.transform.localPosition);
            float diff = previousDistance - newDistance;

            AddReward(diff * 0.1f);
            previousDistance = newDistance;

            if (newDistance < 0.5f)
            {
                AddReward(2f);
                EndEpisode();
            }
        }
        else
        {
            // No plant available encourage staying near center
            Vector3 center = allowedArea.center + allowedArea.transform.localPosition;
            float distFromCenter = Vector3.Distance(transform.localPosition, center);
            AddReward(-distFromCenter * 0.0005f);
        }
    }

    // ---------------------------------------------------------
    // AREA HELPERS
    // ---------------------------------------------------------

    private Vector3 GetRandomPointInsideArea()
    {
        Vector3 center = allowedArea.center + allowedArea.transform.localPosition;
        Vector3 size = allowedArea.size * 0.5f;

        return new Vector3(
            Random.Range(center.x - size.x, center.x + size.x),
            0.75f,
            Random.Range(center.z - size.z, center.z + size.z)
        );
    }

    private bool IsInsideAllowedArea(Vector3 pos)
    {
        Vector3 center = allowedArea.center + allowedArea.transform.localPosition;
        Vector3 size = allowedArea.size * 0.5f;

        return
            pos.x >= center.x - size.x &&
            pos.x <= center.x + size.x &&
            pos.z >= center.z - size.z &&
            pos.z <= center.z + size.z;
    }

    private float DistanceToAreaEdge(Vector3 pos)
    {
        Vector3 center = allowedArea.center + allowedArea.transform.localPosition;
        Vector3 size = allowedArea.size * 0.5f;

        float dx = size.x - Mathf.Abs(pos.x - center.x);
        float dz = size.z - Mathf.Abs(pos.z - center.z);

        return Mathf.Min(dx, dz);
    }

    // ---------------------------------------------------------
    // PLANT SEARCH
    // ---------------------------------------------------------

    private TomatoPlant FindNearestRipePlant()
    {
        TomatoPlant[] plants = FindObjectsByType<TomatoPlant>(FindObjectsSortMode.None);

        TomatoPlant best = null;
        float minDist = float.MaxValue;

        foreach (var plant in plants)
        {
            if (!plant.IsRipeEnough) continue;

            float dist = Vector3.Distance(transform.localPosition, plant.transform.localPosition);

            if (dist < minDist)
            {
                minDist = dist;
                best = plant;
            }
        }

        return best;
    }
}