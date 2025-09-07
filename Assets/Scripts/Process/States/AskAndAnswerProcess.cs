using Cysharp.Threading.Tasks;

public class AskAndAnswerProcess : MainSceneProcess
{
    private AskAndAnswerLayer layer;
    
    public AskAndAnswerProcess()
    {
        Step = MainSceneStep.AskAndAnswer;
    }

    public override async UniTask ProcessEnter()
    {
        layer = await UILayerLoader.LoadAsync<AskAndAnswerLayer>();
    }
    
    public override async UniTask ProcessEnd()
    {
        await UILayerLoader.Remove<AskAndAnswerLayer>();
    }
}
