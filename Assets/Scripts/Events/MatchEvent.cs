namespace Events
{
    public class MatchEvent
    {
        public readonly int Count;
        public readonly int Length;

        public MatchEvent(int count, int length)
        {
            Count = count;
            Length = length;
        }
    }
}