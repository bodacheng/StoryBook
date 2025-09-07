
public class TitleLayer : UILayer
{
    public void MoveToAsk()
    {
        Move(MainSceneStep.AskAndAnswer);
    }
    
    public void MoveToPicGenerator()
    {
        Move(MainSceneStep.PicGenerator);
    }
}
