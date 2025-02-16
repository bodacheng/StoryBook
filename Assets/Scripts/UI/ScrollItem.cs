
public abstract class ScrollData
{
        
}

public abstract class ScrollItem<T> : ScrollGridItem where T : ScrollData
{
    public T GetScrollData() => ScrollData;
    protected T ScrollData;
    protected sealed override void OnSetView(object value)
    {
        ScrollData = (T)value;
        Initialize();
    }
        
    protected abstract void Initialize();
}