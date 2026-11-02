using UnityEngine;

// Zorg dat de Enemy altijd een meter en een vision sensor heeft
[RequireComponent(typeof(VisionSensor))]
[RequireComponent(typeof(DetectionMeter))]
public class EnemyStateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; } = State.Patrol;

    private DetectionMeter _meter;
    private VisionSensor _vision;

    private void Awake()
    {
        _meter = GetComponent<DetectionMeter>();
        _vision = GetComponent<VisionSensor>();
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        Debug.Log($"Meter: {_meter.CurrentDetection:F2} | CanSee: {_vision.CanSeePlayer} | State: {CurrentState}");
        // Bepaal de nieuwe state op basis van de meter
        State newState = CurrentState;

        if (_meter.IsAttacking)
        {
            newState = State.Attack;
        }
        else if (_meter.IsInvestigating)
        {
            newState = State.Investigate;
        }
        else if (_meter.IsAlerted)
        {
            newState = State.Alert;
        }
        else
        {
            newState = State.Patrol;
        }

        // on state change
        if (newState != CurrentState)
        {
            CurrentState = newState;
            Debug.Log($"[StateMachine] {name} is nu in de {CurrentState} state!");

            //TODO: NavMesh
            switch (CurrentState)
            {
                case State.Patrol:
                    // TODO: Go to patrol spot
                    Debug.Log("<State> Patrol");
                    break;
                case State.Alert:
                    // TODO: Stop walking and look at the player
                    Debug.Log("<State> Alert");
                    break;
                case State.Investigate:
                    // TODO: Walk to _vision.LastSeenPosition
                    Debug.Log("<State> Investigate");
                    break;
                case State.Attack:
                    // TODO: ATTACK!!                
                    Debug.Log("<State> Attack");
                    break;
            }
        }
    }
}