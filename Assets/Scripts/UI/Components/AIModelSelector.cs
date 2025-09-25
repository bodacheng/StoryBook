using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for selecting AI models
/// </summary>
public class AIModelSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown modelDropdown;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button applyButton;
    
    [Header("Settings")]
    [SerializeField] private bool showStatus = true;
    [SerializeField] private bool autoApply = false;
    
    private AIServiceManager aiServiceManager;
    private AIModelType[] availableModels;
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }
    
    void OnDestroy()
    {
        RemoveEventListeners();
    }
    
    /// <summary>
    /// Initialize UI components
    /// </summary>
    private void InitializeUI()
    {
        // Get AI Service Manager
        aiServiceManager = AIServiceManager.Instance;
        
        if (aiServiceManager == null)
        {
            Debug.LogError("AIServiceManager not found!");
            SetStatus("AI Service Manager not available", Color.red);
            return;
        }
        
        // Subscribe to events
        aiServiceManager.OnModelChanged += OnModelChanged;
        aiServiceManager.OnError += OnError;
        
        // Initialize dropdown
        RefreshModelList();
        UpdateStatus();
    }
    
    /// <summary>
    /// Setup event listeners
    /// </summary>
    private void SetupEventListeners()
    {
        if (modelDropdown != null)
        {
            modelDropdown.onValueChanged.AddListener(OnModelDropdownChanged);
        }
        
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshModelList);
        }
        
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(ApplyModelChange);
        }
    }
    
    /// <summary>
    /// Remove event listeners
    /// </summary>
    private void RemoveEventListeners()
    {
        if (aiServiceManager != null)
        {
            aiServiceManager.OnModelChanged -= OnModelChanged;
            aiServiceManager.OnError -= OnError;
        }
        
        if (modelDropdown != null)
        {
            modelDropdown.onValueChanged.RemoveListener(OnModelDropdownChanged);
        }
        
        if (refreshButton != null)
        {
            refreshButton.onClick.RemoveListener(RefreshModelList);
        }
        
        if (applyButton != null)
        {
            applyButton.onClick.RemoveListener(ApplyModelChange);
        }
    }
    
    /// <summary>
    /// Refresh the list of available models
    /// </summary>
    public void RefreshModelList()
    {
        if (aiServiceManager == null) return;
        
        availableModels = aiServiceManager.GetAvailableModels();
        
        if (modelDropdown != null)
        {
            modelDropdown.ClearOptions();
            
            var options = new System.Collections.Generic.List<string>();
            foreach (var model in availableModels)
            {
                options.Add(model.ToString());
            }
            
            modelDropdown.AddOptions(options);
            
            // Set current selection
            var currentModel = aiServiceManager.CurrentModel;
            var currentIndex = System.Array.IndexOf(availableModels, currentModel);
            if (currentIndex >= 0)
            {
                modelDropdown.value = currentIndex;
            }
        }
        
        UpdateStatus();
    }
    
    /// <summary>
    /// Handle model dropdown change
    /// </summary>
    private void OnModelDropdownChanged(int index)
    {
        if (availableModels == null || index < 0 || index >= availableModels.Length) return;
        
        var selectedModel = availableModels[index];
        
        if (autoApply)
        {
            ApplyModelChange(selectedModel);
        }
        else
        {
            SetStatus($"Selected: {selectedModel} (Click Apply to switch)", Color.yellow);
        }
    }
    
    /// <summary>
    /// Apply the selected model change
    /// </summary>
    public void ApplyModelChange()
    {
        if (modelDropdown == null || availableModels == null) return;
        
        var index = modelDropdown.value;
        if (index >= 0 && index < availableModels.Length)
        {
            ApplyModelChange(availableModels[index]);
        }
    }
    
    /// <summary>
    /// Apply model change to AI service manager
    /// </summary>
    private void ApplyModelChange(AIModelType modelType)
    {
        if (aiServiceManager == null) return;
        
        bool success = aiServiceManager.SwitchToModel(modelType);
        
        if (success)
        {
            UpdateStatus();
        }
        else
        {
            SetStatus($"Failed to switch to {modelType}", Color.red);
        }
    }
    
    /// <summary>
    /// Update status display
    /// </summary>
    private void UpdateStatus()
    {
        if (aiServiceManager == null) return;
        
        var currentModel = aiServiceManager.CurrentModel;
        var isConfigured = aiServiceManager.IsConfigured;
        var providerName = aiServiceManager.CurrentProviderName;
        
        if (isConfigured)
        {
            SetStatus($"Current: {providerName} ({currentModel})", Color.green);
        }
        else
        {
            SetStatus($"Current: {providerName} ({currentModel}) - Not Configured", Color.red);
        }
    }
    
    /// <summary>
    /// Set status text with color
    /// </summary>
    private void SetStatus(string message, Color color)
    {
        if (statusText != null && showStatus)
        {
            statusText.text = message;
            statusText.color = color;
        }
        
        Debug.Log($"[AIModelSelector] {message}");
    }
    
    /// <summary>
    /// Handle model changed event
    /// </summary>
    private void OnModelChanged(AIModelType newModel)
    {
        RefreshModelList();
    }
    
    /// <summary>
    /// Handle error event
    /// </summary>
    private void OnError(string errorMessage)
    {
        SetStatus($"Error: {errorMessage}", Color.red);
    }
    
    /// <summary>
    /// Enable/disable auto-apply mode
    /// </summary>
    public void SetAutoApply(bool enabled)
    {
        autoApply = enabled;
        
        if (applyButton != null)
        {
            applyButton.gameObject.SetActive(!enabled);
        }
    }
    
    /// <summary>
    /// Show/hide status text
    /// </summary>
    public void SetShowStatus(bool show)
    {
        showStatus = show;
        
        if (statusText != null)
        {
            statusText.gameObject.SetActive(show);
        }
    }
}
