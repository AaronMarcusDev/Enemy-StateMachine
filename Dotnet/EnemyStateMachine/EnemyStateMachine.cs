class StateMachine
{

    [SerializeField] private VisionSensor vision;
    [SerializeField] private float detectionFillRate = 0.5f;
    [SerializeField] private float detectionDrainRate = 0.25f;
    [SerializeField] private float alertThreshold = 0.5f;
    [SerializeField] private float attackThreshold = 1f;

    private float detection = 0f;
    private State currentState = State.Patrol;

    void Update()
    {
        if (vision.CanSeePlayer)
            detection = Mathf.Clamp01(detection + detectionFillRate * Time.deltaTime);
        else
            detection = Mathf.Clamp01(detection - detectionDrainRate * Time.deltaTime);

        // state-logica op basis van detection + vision
    }
}