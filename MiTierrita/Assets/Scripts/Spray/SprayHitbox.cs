using UnityEngine;

public class SprayHitbox : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        CaterpillarAgent agent = other.GetComponent<CaterpillarAgent>();

        if (agent != null)
        {
            Debug.Log("Worm Killed");
            if (GameManager.Instance != null) GameManager.Instance.CaterpillarKilledScore();

            agent.AddReward(-1.0f);
            agent.EndEpisode();
        }
    }
}