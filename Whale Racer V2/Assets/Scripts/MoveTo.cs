using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{

    public Transform goal;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        bool worked = agent.SetDestination(goal.position);
        Debug.Log(worked);
        Debug.Log(agent.destination);
    }
}