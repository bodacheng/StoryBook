using UnityEngine;

[CreateAssetMenu(fileName = "GeminiConfig", menuName = "StoryBook/API/Gemini Config")]
public class GeminiConfig : ScriptableObject
{
    [Header("API配置")]
    [SerializeField] private string apiKey = "YOUR_API_KEY";
    [SerializeField] private string model = "gemini-2.5-flash";
    
    [Header("请求设置")]
    [SerializeField] private int defaultTimeoutMs = 20000;
    [SerializeField] private int imageTimeoutMs = 60000;
    
    [Header("图片生成设置")]
    [SerializeField] private int defaultImageCount = 1;
    [SerializeField] private string defaultAspectRatio = "1:1";
    
    // 公共属性
    public string ApiKey => apiKey;
    public string Model => model;
    public int DefaultTimeoutMs => defaultTimeoutMs;
    public int ImageTimeoutMs => imageTimeoutMs;
    public int DefaultImageCount => defaultImageCount;
    public string DefaultAspectRatio => defaultAspectRatio;
    
    // 验证配置
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY" && 
               !string.IsNullOrEmpty(model);
    }
    
    // 在编辑器中验证
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY")
        {
            Debug.LogWarning($"[{name}] 请设置有效的API Key");
        }
        
        if (string.IsNullOrEmpty(model))
        {
            Debug.LogWarning($"[{name}] 请设置模型名称");
        }
    }
}
