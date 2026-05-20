using maze_runner.Core;

namespace maze_runner.Model.Entities.Mobs;

public abstract class Mob : Entity
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly IEventPublisher _eventPublisher;
    private readonly Action<AcousticWavePropagate> _acousticHandler;

    protected Mob(string name, int maxHealth, IEventPublisher ep, IEventSubscriber es) : base(name, maxHealth)
    {
        _eventSubscriber = es;
        _eventPublisher = ep;
        _acousticHandler = HandleAcousticWave;
        _eventSubscriber.Subscribe(_acousticHandler);
    }


    private void HandleAcousticWave(AcousticWavePropagate e)
    {
        if (e.Wave.TryGetValue(this.Position, out var distance))
        {
            _eventPublisher.Publish(new SoundRegisteredEvent(this, e.Origin, distance));
        }
    }

    protected override void Die()
    {
        _eventSubscriber.Unsubscribe(_acousticHandler);
    }
}