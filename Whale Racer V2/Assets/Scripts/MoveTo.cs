using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Animator whaleAnimator;
    private AnimationHashTable animations;

    public GameObject goal;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        whaleAnimator = gameObject.GetComponent<Animator>();

        Debug.Log(agent.transform.rotation);
        Debug.Log(agent.SetDestination(goal.transform.position));
    }
    void Update()
    {
        float velocity = GetComponent<NavMeshAgent>().velocity.magnitude;
        if (animations == null)
        {
            animations = GameManager.gmInst.GetComponent<AnimationHashTable>();
        }
        if (velocity > 2f)
        {
            whaleAnimator.SetFloat(animations.moveFloat, velocity / 2);
        }
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