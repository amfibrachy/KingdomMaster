namespace _Scripts.Core.NPC
{
    using JobSystem;

    public interface IHasJob
    {
        JobType Job { get; set; }
    }
}
