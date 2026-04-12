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
    public float arenaSize = 6f;

    private TomatoPlant nearestPlant;
    private float previousDistance;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset velocity
        rb.linearVelocity = Vector3.zero;

        // Randomize agent position
        transform.localPosition = new Vector3(
            Random.Range(-arenaSize * 0.8f, arenaSize * 0.8f),
            0,
            Random.Range(-arenaSize * 0.8f, arenaSize * 0.8f)
        );

        // Find nearest plant at start
        nearestPlant = FindNearestRipePlant();
        previousDistance = nearestPlant != null
            ? Vector3.Distance(transform.localPosition, nearestPlant.transform.localPosition)
            : 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position (3)
        sensor.AddObservation(transform.localPosition);

        // Player position (3)
        sensor.AddObservation(player.localPosition);

        // Nearest plant direction (3)
        nearestPlant = FindNearestRipePlant();
        if (nearestPlant != null)
        {
            Vector3 dir = (nearestPlant.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dir);

            // Distance to plant (1)
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

        // Time penalty (encourages faster completion)
        AddReward(-0.0005f);

        // Bounds check
        if (Mathf.Abs(transform.localPosition.x) > arenaSize ||
            Mathf.Abs(transform.localPosition.z) > arenaSize)
        {
            AddReward(-1f);
            EndEpisode();
            return;
        }

        // Reward for approaching plant
        if (nearestPlant != null)
        {
            float newDistance = Vector3.Distance(transform.localPosition, nearestPlant.transform.localPosition);
            float diff = previousDistance - newDistance;

            // Reward moving closer, punish moving away
            AddReward(diff * 0.1f);

            previousDistance = newDistance;

            // Reached plant
            if (newDistance < 0.5f)
            {
                AddReward(2f);
                EndEpisode();
            }
        }
    }


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