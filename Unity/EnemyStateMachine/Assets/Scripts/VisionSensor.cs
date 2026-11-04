using UnityEngine;

[RequireComponent(typeof(Transform))]

// I have 'designed' this class with three main steps:
// 1. a radius/distance check
// 2. an fov check similar to Maths3D
// 3. A raycast check to see whether there is not an object in the way, that's why there are layers below.
// I hope the rest of the code explains itself :)

public class VisionSensor : MonoBehaviour
{
    [Header("Config")]
    public float viewRange = 10f;

    [Range(0f, 360f)] // just a circle all around
    public float fieldOfView = 90f;

    [Header("Layers")]
    public LayerMask playerLayer;
    public LayerMask obstacleLayer; // to block the raycast

    [Header("Debug")]
    public bool debugMode = false;
    public bool drawGizmos = true;

    // Output variables (public)
    public bool CanSeePlayer { get; private set; } // auto properties for shorthand notation
    //                                      ^-- the setters may only be changed from within

    public Vector3 LastSeenPosition { get; private set; } // ''

    // Private variables (_)
    private Transform _currPlayerTrans;
    private bool _prevSeenBool; // to compare to on a state change
                                //                      ^-- state, as in state of the player in vision t/f

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // retrieve player and set the transform
        if (playerObj != null)
        {
            _currPlayerTrans = playerObj.transform;
            if (debugMode)
            {
                // nice to have as feedback
                DebugLogs.msg(DebugLogs.DebugOption.FoundPlayer, _currPlayerTrans.name);
            }
        }
        else
        {
            // is useful to check whether the player tag is assigned for example,
            //  independent of the debug mode as this is important
            DebugLogs.msg(DebugLogs.DebugOption.NoPlayer);
        }
    }

    private void Update()
    {
        CanSeePlayer = CheckVision();

        if (CanSeePlayer != _prevSeenBool) // player enters or leaves vision compared to previous frame
        {
            if (debugMode)
            {
                DebugLogs.msg(DebugLogs.DebugOption.SightChange, CanSeePlayer.ToString());
            }
            _prevSeenBool = CanSeePlayer;
        }
    }

    private bool CheckVision()
    {
        if (_currPlayerTrans == null) return false;

        // vector from the enemy to the player
        Vector3 toPlayer = _currPlayerTrans.position - transform.position;
        float distance = toPlayer.magnitude;

        // Check whether the player is in the vision range :O
        if (distance > viewRange) return false;

        // Check of the FOV angle 
        float angle = Vector3.Angle(transform.forward, toPlayer);
        // ^-- much easier than in Math3D haha

        // the angle is from one side of the z axis, zo if it is large then half of the FOV then the player is out of sight
        if (angle > fieldOfView * 0.5f)
        {
            if (debugMode)
            {
                DebugLogs.msg(DebugLogs.DebugOption.FOVCheck);
                UnityEngine.Debug.Log($"Angle: {angle}, FOV/2: {fieldOfView * 0.5f}");
            }
            return false;
        }

        // Checking if there are no objects in the way with a raycast (nice for the player so they can hide)
        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, distance, obstacleLayer, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        LastSeenPosition = _currPlayerTrans.position; // update the known position
        return true; // the player is in the FOV
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        // Yellow wire sphere showing the maximum view range radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
        // https://docs.unity3d.com/520/Documentation/ScriptReference/Gizmos.DrawWireSphere.html?source=post_page-----6b10c683ef09--------------------------------

        // Calculate half the field of view angle so it can build the cone edges the same on both sides
        float halfFov = fieldOfView * 0.5f;

        // Boundaries of the vision cones
        // by rotating the forward vector around Y by half of the FOV on both sides
        Vector3 leftDir = Quaternion.Euler(0, -halfFov, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, halfFov, 0) * transform.forward;

        // Draw the two outer rays (edges of the vision cone)
        Gizmos.DrawRay(transform.position, leftDir * viewRange);
        Gizmos.DrawRay(transform.position, rightDir * viewRange);

        // Number of segments to approximate the curve
        int steps = 20;

        // Start the arc from the left edge of the cone
        Vector3 prev = transform.position + leftDir * viewRange;

        // Iterate across the FOV and draw line segments to build the arc, literally made of lines, but it works well
        for (int i = 1; i <= steps; i++)
        {
            // progress along the arc (0 = left edge, 1 = right edge)
            float t = i / (float)steps;

            // Interpolate the direction from left to right across the field of view
            Vector3 dir = Quaternion.Euler(0, -halfFov + fieldOfView * t, 0) * transform.forward;

            // Endpoint of the current arc segment at the outer view range
            Vector3 point = transform.position + dir * viewRange;

            // Draw a segment from the previous point to the current point
            Gizmos.DrawLine(prev, point);
            prev = point;
        }

        // If the player is currently visible, draw a green line to the last known position
        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, LastSeenPosition);
        }
    }
}