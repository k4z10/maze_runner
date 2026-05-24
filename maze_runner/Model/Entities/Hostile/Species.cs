namespace maze_runner.Model.Entities.Hostile;

public abstract class Species<T> where T : Entity
{
    protected readonly List<T> AliveMembers = new();
    
    public void Register(T member)
    {
        AliveMembers.Add(member);
    }

    public void ReportDeath(T deadMember)
    {
        AliveMembers.Remove(deadMember);
        OnMemberDeath();
    }

    protected abstract void OnMemberDeath();
}