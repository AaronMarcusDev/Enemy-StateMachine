class StateMachine
{
    enum State
    {
        Patrol,
        Alert,
        Investigate,
        Attack
    }

    static void Main(string[] args)
    {
        State CurrentState = State.Patrol;

        switch (CurrentState)
        {
            case State.Patrol : Console.WriteLine("Patrol state enabled");
            case State.Alert : Console.WriteLine("Alert state enabled");
            case State.Investigate : Console.WriteLine("Investigate state enabled");
            case State.Attack : Console.WriteLine("Attack state enabled");
        }

    }
}