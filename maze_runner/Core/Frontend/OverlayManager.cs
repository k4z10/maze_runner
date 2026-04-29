namespace maze_runner.Core.Frontend;

public class OverlayManager
{
    private readonly Stack<IOverlay> _overlays = new();

    public void Push(IOverlay overlay) => _overlays.Push(overlay);

    public void UpdateAndProcessInput(char key)
    {
        while (_overlays.Count > 0 && _overlays.Peek().IsFinished)
        {
            _overlays.Pop();
        }

        if (_overlays.Count > 0)
        {
            _overlays.Peek().ProcessInput(key);
        }
    }

    public void RenderAll()
    {
        if (_overlays.Count == 0) return;
        foreach (var overlay in _overlays.Reverse())
        {
            overlay.Render();
        }
    }

    public bool HasActiveOverlays => _overlays.Count > 0;
}