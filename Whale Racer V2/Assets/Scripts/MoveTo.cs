using UnityEngine;
using System.Collections;
using UnityEngine.AI;
/// <summary>
/// Script to direct the AI Navmeshagent to move between checkpoints.
/// </summary>
public class MoveTo : MonoBehaviour
{
    public Animator whaleAnimator;
    private AnimationHashTable animations;

    public GameObject goal;
    /// <summary>
    /// Start method. Initializes navmeshagent and AI whale animator.
    /// </summary>
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        whaleAnimator = gameObject.GetComponent<Animator>();
        agent.SetDestination(goal.transform.position);
    }
    /// <summary>
    /// Update method for the AI whale. Plays animations.
    /// </summary>
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
    /// <summary>
    /// OnTriggerEnter method for the AI whale. Updates its destination to the next checkpoint when it enters one.
    /// </summary>
    /// <param name="other">The object (eventually a PathingWaypoint) the AI whale collides with.</param>
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