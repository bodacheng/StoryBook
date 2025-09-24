using UnityEngine;

[CreateAssetMenu(fileName = "GeminiConfig", menuName = "StoryBook/API/Gemini Config")]
public class GeminiConfig : ScriptableObject
{
    [Header("API Configuration")]
    [SerializeField] private string apiKey = "YOUR_API_KEY";
    [SerializeField] private string model = "gemini-2.5-flash";
    
    [Header("Request Settings")]
    [SerializeField] private int defaultTimeoutMs = 20000;
    [SerializeField] private int imageTimeoutMs = 60000;
    
    [Header("Image Generation Settings")]
    [SerializeField] private int defaultImageCount = 1;
    [SerializeField] private string defaultAspectRatio = "1:1";
    
    // Public properties
    public string ApiKey => apiKey;
    public string Model => model;
    public int DefaultTimeoutMs => defaultTimeoutMs;
    public int ImageTimeoutMs => imageTimeoutMs;
    public int DefaultImageCount => defaultImageCount;
    public string DefaultAspectRatio => defaultAspectRatio;
    
    // Validate configuration
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY" && 
               !string.IsNullOrEmpty(model);
    }
    
    // Validate in editor
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY")
        {
            Debug.LogWarning($"[{name}] Please set a valid API Key");
        }
        
        if (string.IsNullOrEmpty(model))
        {
            Debug.LogWarning($"[{name}] Please set model name");
        }
    }
}
