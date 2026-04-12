using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CaterpillarAgent : Agent
{
    public float moveSpeed = 3f;

    public Transform player;
    public LayerMask plantLayer;

    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset position
        transform.localPosition = new Vector3(
            Random.Range(-5f, 5f),
            0,
            Random.Range(-5f, 5f)
        );

        rb.linearVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position
        sensor.AddObservation(transform.localPosition);

        // Player position
        sensor.AddObservation(player.localPosition);

        // Direction to nearest ripe plant
        TomatoPlant nearest = FindNearestRipePlant();
        if (nearest != null)
        {
            Vector3 dir = (nearest.transform.position - transform.position).normalized;
            sensor.AddObservation(dir);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3(
            actions.ContinuousActions[0],
            0,
            actions.ContinuousActions[1]
        );

        rb.AddForce(move * moveSpeed, ForceMode.VelocityChange);

 
        AddReward(-0.001f);
    }

    private TomatoPlant FindNearestRipePlant()
    {
        TomatoPlant[] plants = FindObjectsByType<TomatoPlant>(FindObjectsSortMode.None);

        TomatoPlant best = null;
        float minDist = float.MaxValue;

        foreach (var plant in plants)
        {
            if (!plant.IsRipeEnough) continue;

            float dist = Vector3.Distance(transform.position, plant.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                best = plant;
            }
        }

        return best;
    }
}
