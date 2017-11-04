using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///waypoint script attached to checkpoint for AI whales to path towards.
///based on James Arndt's Unity Simple Vehicle AI tutorial - adapted from his JS script
///at any given time, ai whale's selected PathingWaypoint will be the waypoint it is heading towards.
/// </summary>
public class PathingWaypoint : MonoBehaviour
{
    //the first waypoint - shared by all waypoint scripts
    public static PathingWaypoint first;
    //pointer to the next waypoint after this one
    public PathingWaypoint next;
    bool isStart = false;
    public Vector3 CalculateTarget(Vector3 position)
    {
        //once close to the waypoint we're heading towards, start heading towards the one after that in advance
        if (Vector3.Distance(transform.position, position) < 6)
        {
            return next.transform.position;
        }
        //else just return the current target
        return transform.position;
    }

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
