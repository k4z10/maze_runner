namespace maze_runner.Model.Entities.Mobs;

public class SkeletonTribe : Species<Skeleton>
{
    protected override void OnMemberDeath()
    {
        foreach (var skely in AliveMembers)
            skely.Enrage();
    }
}