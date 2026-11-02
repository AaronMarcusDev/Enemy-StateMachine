class StateMachine
{

    static void Main(string[] args)
    {
        State CurrentState = State.Alert;

        switch (CurrentState)
        {
            case State.Patrol: { Console.WriteLine("Patrol state enabled"); break; }
            case State.Alert: { Console.WriteLine("Alert state enabled"); break; }
            case State.Investigate: { Console.WriteLine("Investigate state enabled"); break; }
            case State.Attack: { Console.WriteLine("Attack state enabled"); break; }
        }

    }
}