using UnityEngine;
using UnityEngine.UI;

public class PicGeneratorLayer : UILayer
{
    [SerializeField] private InputField askInputField;
    [SerializeField] private RawImage preview;
    [SerializeField] private Text state;
    
    public async void OnGenClicked()
    {
        state.text = "Generating…";
        var tex = await SceneManager.Instance.GeminiClient.GeneratePic(askInputField.text);
        preview.texture = tex[0];
        //preview.SetNativeSize(); // Or set size manually
        state.text = "Done";
    }
}
