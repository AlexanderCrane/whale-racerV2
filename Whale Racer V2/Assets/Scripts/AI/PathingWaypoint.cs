using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///waypoint script attached to checkpoint for AI whales to path towards.
///based on James Arndt's Unity Simple Vehicle AI tutorial - adapted from his JS script
///PathingWaypoint scripts on checkpoints serve as a 'linked list' of destinations for the AI NavMeshAgent.
/// </summary>
public class PathingWaypoint : MonoBehaviour
{
    //the first waypoint - shared by all waypoint scripts
    public static PathingWaypoint first;
    //pointer to the next waypoint after this one
    public PathingWaypoint next;
    bool isStart = false;

    ///  <summary>
    /// Awake method for pathing waypoints. Initializes the first waypoint to this one if selected in the editor.
    ///  </summary>
    void Awake()
    {
        if (isStart)
        {
            first = this;
        }
    }
    ///  <summary>
    /// Draws a green line between correctly connected waypoints in the editor for design purposes.
    ///  </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .3f);
        Gizmos.DrawCube(transform.position, new Vector3(5, 5, 5));

        if (next)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, next.transform.
            position);
        }
    }
}
