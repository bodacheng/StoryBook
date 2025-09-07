using Cysharp.Threading.Tasks;

public class TitleProcess : MainSceneProcess
{
    private TitleLayer layer;
    
    public TitleProcess()
    {
        Step = MainSceneStep.Title;
    }

    public override async UniTask ProcessEnter()
    {
        layer = await UILayerLoader.LoadAsync<TitleLayer>();
    }
    
    public override async UniTask ProcessEnd()
    {
        await UILayerLoader.Remove<TitleLayer>();
    }
}
