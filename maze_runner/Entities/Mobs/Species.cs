namespace maze_runner.Entities.Mobs;

public abstract class Species<T> where T : Mob
{
    protected readonly List<T> _aliveMembers = new();
    
    public void Register(T member)
    {
        _aliveMembers.Add(member);
    }

    public void ReportDeath(T deadMember)
    {
        _aliveMembers.Remove(deadMember);
        OnMemberDeath();
    }

    protected abstract void OnMemberDeath();
}