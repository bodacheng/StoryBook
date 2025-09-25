using UnityEngine;

[CreateAssetMenu(fileName = "AIServiceConfig", menuName = "StoryBook/API/AI Service Config")]
public class AIServiceConfig : ScriptableObject
{
    [Header("Model Selection")]
    [SerializeField] private AIModelType currentModel = AIModelType.Gemini;
    
    [Header("Model Configurations")]
    [SerializeField] private GeminiConfig geminiConfig;
    [SerializeField] private OpenAIConfig openAIConfig;
    
    [Header("Settings")]
    [SerializeField] private bool allowModelSwitching = true;
    [SerializeField] private bool showModelInUI = true;
    
    // Public properties
    public AIModelType CurrentModel => currentModel;
    public GeminiConfig GeminiConfig => geminiConfig;
    public OpenAIConfig OpenAIConfig => openAIConfig;
    public bool AllowModelSwitching => allowModelSwitching;
    public bool ShowModelInUI => showModelInUI;
    
    /// <summary>
    /// Switch to a different AI model
    /// </summary>
    public void SetModel(AIModelType model)
    {
        if (!allowModelSwitching)
        {
            Debug.LogWarning("Model switching is disabled in configuration");
            return;
        }
        
        currentModel = model;
        Debug.Log($"AI Model switched to: {model}");
    }
    
    /// <summary>
    /// Get the current model configuration
    /// </summary>
    public ScriptableObject GetCurrentConfig()
    {
        return currentModel switch
        {
            AIModelType.Gemini => geminiConfig,
            AIModelType.OpenAI => openAIConfig,
            _ => geminiConfig
        };
    }
    
    /// <summary>
    /// Check if the current model is properly configured
    /// </summary>
    public bool IsCurrentModelConfigured()
    {
        return currentModel switch
        {
            AIModelType.Gemini => geminiConfig != null && geminiConfig.IsValid(),
            AIModelType.OpenAI => openAIConfig != null && openAIConfig.IsValid(),
            _ => false
        };
    }
    
    /// <summary>
    /// Get available model types
    /// </summary>
    public AIModelType[] GetAvailableModels()
    {
        var available = new System.Collections.Generic.List<AIModelType>();
        
        if (geminiConfig != null && geminiConfig.IsValid())
            available.Add(AIModelType.Gemini);
            
        if (openAIConfig != null && openAIConfig.IsValid())
            available.Add(AIModelType.OpenAI);
            
        return available.ToArray();
    }
    
    // Validate in editor
    private void OnValidate()
    {
        if (geminiConfig == null)
        {
            Debug.LogWarning($"[{name}] Gemini config is not assigned");
        }
        
        if (openAIConfig == null)
        {
            Debug.LogWarning($"[{name}] OpenAI config is not assigned");
        }
        
        if (!IsCurrentModelConfigured())
        {
            Debug.LogWarning($"[{name}] Current model ({currentModel}) is not properly configured");
        }
    }
}
