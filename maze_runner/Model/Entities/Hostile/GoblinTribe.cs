namespace maze_runner.Model.Entities.Hostile;

public class GoblinTribe : Species<Goblin>
{
    protected override void OnMemberDeath()
    {
        foreach (var goblin in AliveMembers)
        {
            goblin.Frighten();
        }
    }
}