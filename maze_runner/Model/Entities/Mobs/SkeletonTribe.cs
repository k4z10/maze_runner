namespace maze_runner.Model.Entities.Mobs;

public class SkeletonTribe : Species<Skeleton>
{
    protected override void OnMemberDeath()
    {
        foreach (var skely in _aliveMembers)
            skely.Enrage();
    }
}