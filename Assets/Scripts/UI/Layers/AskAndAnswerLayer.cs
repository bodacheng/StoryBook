using UnityEngine;
using UnityEngine.UI;

public class AskAndAnswerLayer : UILayer
{
    [SerializeField] private InputField askInputField;
    [SerializeField] private Text answerText;
    
    public async void OnAskClicked()
    {
        answerText.text = "Asking…";
        
        try
        {
            var ans = await SceneManager.Instance.GeminiClient.AskAsync(askInputField.text);
            answerText.text = ans;
        }
        catch (System.Exception ex)
        {
            answerText.text = ex.Message;
        }
    }
}
