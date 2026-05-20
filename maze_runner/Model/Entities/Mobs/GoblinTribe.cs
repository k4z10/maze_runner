namespace maze_runner.Model.Entities.Mobs;

public class GoblinTribe : Species<Goblin>
{
    protected override void OnMemberDeath()
    {
        foreach (var goblin in _aliveMembers)
        {
            goblin.Frighten();
        }
    }
}