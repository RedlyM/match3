namespace MatchThree.Spawning
{
    public interface IPoolable
    {
        void Prepare();
        void Release();
    }
}