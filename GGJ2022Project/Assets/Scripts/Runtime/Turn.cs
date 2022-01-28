namespace GGJ
{
    /// <summary>
    /// Represents a game turn. Provided to TurnReceiver by TurnSystem. Should include convenience methods for things like setting the camera target?
    /// </summary>
    public class Turn
    {
        public Turn(TurnSystem turnSystem, TurnReceiver turnReceiver, int turnNumber)
        {
            this.turnSystem = turnSystem;
            this.turnReceiver = turnReceiver;
            this.turnNumber = turnNumber;
        }

        public readonly TurnSystem turnSystem;
        public readonly TurnReceiver turnReceiver;
        public readonly int turnNumber;
    }
}
