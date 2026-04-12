using UnityEngine;

public class PlayerDamageZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CaterpillarAgent agent = other.GetComponent<CaterpillarAgent>();

        if (agent != null)
        {
            agent.AddReward(-1.0f);
            agent.EndEpisode();
        }
    }
}
