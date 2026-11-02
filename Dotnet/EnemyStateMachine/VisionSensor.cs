using UnityEngine;

[RequireComponent(typeof(Transform))]
public class VisionSensor : MonoBehaviour
{
    [Header("Configuratie")]
    [Tooltip("Hoe ver de vijand kan kijken (in meters).")]
    public float viewRange = 10f;

    [Tooltip("Totale field of view in graden (bijv. 90 = 45 links + 45 rechts).")]
    [Range(0f, 360f)]
    public float fieldOfView = 90f;

    [Tooltip("Welke layers tellen als 'speler'?")]
    public LayerMask playerLayer;

    [Tooltip("Welke layers blokkeren het zicht (muren, dozen, etc.)?")]
    public LayerMask obstacleLayer;

    [Header("Debug")]
    public bool drawGizmos = true;
    public Color gizmoColor = Color.yellow;

    // --- Outputs voor de state machine ---
    public bool CanSeePlayer { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private Transform _player;

    private void Awake()
    {
        // Zoek de speler één keer (tag "Player").
        // Wil je liever via layer werken? Gebruik dan een trigger of singleton.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning($"{name}: geen object met tag 'Player' gevonden.");
    }

    private void Update()
    {
        CanSeePlayer = CheckVision();
    }

    private bool CheckVision()
    {
        if (_player == null) return false;

        Vector3 toPlayer = _player.position - transform.position;
        float distance = toPlayer.magnitude;

        // 1. Range check
        if (distance > viewRange) return false;

        // 2. FOV check
        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > fieldOfView * 0.5f) return false;

        // 3. Line-of-sight check (raycast naar de speler)
        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit,
                            distance, obstacleLayer, QueryTriggerInteraction.Ignore))
        {
            // Er zit iets tussen → geen zicht
            return false;
        }

        // Zichtbaar!
        LastSeenPosition = _player.position;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmoColor;

        // Range sphere
        Gizmos.DrawWireSphere(transform.position, viewRange);

        // FOV cone (twee lijnen + een boog)
        float halfFov = fieldOfView * 0.5f;
        Vector3 leftDir  = Quaternion.Euler(0, -halfFov, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0,  halfFov, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftDir  * viewRange);
        Gizmos.DrawRay(transform.position, rightDir * viewRange);

        // Boog tekenen
        int steps = 20;
        Vector3 prev = transform.position + leftDir * viewRange;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 dir = Quaternion.Euler(0, -halfFov + fieldOfView * t, 0) * transform.forward;
            Vector3 point = transform.position + dir * viewRange;
            Gizmos.DrawLine(prev, point);
            prev = point;
        }

        // Laat de laatste bekende positie zien
        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, LastSeenPosition);
        }
    }
}