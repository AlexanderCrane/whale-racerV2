using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{

    public GameObject goal;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Debug.Log(agent.transform.rotation);
        Debug.Log(agent.SetDestination(goal.transform.position));
    }



    void OnTriggerEnter(Collider other)
    {
        GameObject checkPoint = other.gameObject;
        PathingWaypoint wayPoint = checkPoint.GetComponent<PathingWaypoint>();
        if (wayPoint != null)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            GameObject nextGoal = wayPoint.next.gameObject;
            agent.SetDestination(nextGoal.transform.position);
        }
    }
}