using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{

    public GameObject goal;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        bool worked = agent.SetDestination(goal.transform.position);
        Debug.Log(worked);
        Debug.Log(agent.destination);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject checkPoint = other.gameObject;
        PathingWaypoint wayPoint = checkPoint.GetComponent<PathingWaypoint>();
        if (wayPoint != null)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            Debug.Log("retargeting");
            GameObject nextGoal = wayPoint.next.gameObject;
            Debug.Log(nextGoal.name);
            Debug.Log(agent.SetDestination(nextGoal.transform.position));
            //agent.SetDestination(nextGoal.transform.position);
        }
    }
}