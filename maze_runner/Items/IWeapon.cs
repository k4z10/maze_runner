namespace maze_runner.Items;

public interface IWeapon
{
    int Damage { get; }
    int RequiredHands { get; set; }
}