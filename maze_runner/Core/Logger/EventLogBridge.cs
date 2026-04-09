
namespace maze_runner.Core.Logger;

public class EventLogBridge
{
    public EventLogBridge(EventBus bus, IMessageLog logger)
    {
        bus.WallBumped.Subscribe(_ => logger.Log("Tried to walk into wall"));
        bus.ItemEquipped.Subscribe(e => logger.Log($"Equipped item: {e.ItemName}"));
        bus.AttackResolved.Subscribe(e => logger.Log($"{e.Attacker} dealt {e.Damage} damage to enemy: {e.Defender}"));
        bus.EnemyDefeated.Subscribe(e => logger.Log($"Enemy defeated: {e.EnemyName}"));
        bus.ItemPickedUp.Subscribe(e => logger.Log($"Picked up item: {e.ItemName}"));
        bus.UnknownInput.Subscribe(e => logger.Log($"Unknown input: [{e.Key}]"));
    }
}