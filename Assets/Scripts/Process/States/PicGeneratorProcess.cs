using Cysharp.Threading.Tasks;

public class PicGeneratorProcessProcess : MainSceneProcess
{
    private PicGeneratorLayer layer;
    
    public PicGeneratorProcessProcess()
    {
        Step = MainSceneStep.PicGenerator;
    }
    
    public override async UniTask ProcessEnter()
    {
        layer = await UILayerLoader.LoadAsync<PicGeneratorLayer>();
    }
    
    public override async UniTask ProcessEnd()
    {
        await UILayerLoader.Remove<PicGeneratorLayer>();
    }
}
