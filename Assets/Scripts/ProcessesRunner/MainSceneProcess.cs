using Cysharp.Threading.Tasks;

public abstract class MainSceneProcess : SceneProcess
{
    public MainSceneStep Step;
    
    public override async UniTask ProcessEnter()
    {
        await base.ProcessEnter();
    }

    public override async UniTask ProcessEnter<T>(T t)
    {
        await base.ProcessEnter(t);
    }
}

public enum MainSceneStep
{
    Title = 0,
    Chapter = 1
}