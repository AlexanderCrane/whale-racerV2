using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//based on James Arndt's Unity Simple Vehicle AI tutorial - adapted from his JS sript
//at any given time, ai whale's selected PathingWaypoint will be the waypoint it is HEADING TOWARDS
//currently unused because I discovered the navmesh system, but will probably be used to guide navmeshagent between checkpts
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

    // Update is called once per frame
    void Awake()
    {
        if (isStart)
        {
            first = this;
        }
    }
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
