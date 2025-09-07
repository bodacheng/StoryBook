using Cysharp.Threading.Tasks;
using UnityEngine;

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

[SerializeField]
public enum MainSceneStep
{
    Title = 0,
    Chapter = 1,
    AskAndAnswer = 2,
    PicGenerator = 3,
    Story = 4
}