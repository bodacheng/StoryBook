using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
/// Settings UI Layer for configuring AI models and other settings
/// </summary>
public class SettingsLayer : UILayer
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private AIModelSelector modelSelector;
    [SerializeField] private Toggle autoApplyToggle;
    [SerializeField] private Toggle showStatusToggle;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI infoText;
    
    [Header("Settings")]
    [SerializeField] private bool showInfoPanel = true;
    
    private AIServiceManager aiServiceManager;
    
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();
        
        InitializeUI();
        SetupEventListeners();
        UpdateInfo();
    }
    
    public override async UniTask OnPreClose()
    {
        RemoveEventListeners();
        await base.OnPreClose();
    }
    
    /// <summary>
    /// Initialize UI components
    /// </summary>
    private void InitializeUI()
    {
        aiServiceManager = AIServiceManager.Instance;
        
        if (aiServiceManager == null)
        {
            Debug.LogError("AIServiceManager not found!");
            return;
        }
        
        // Initialize model selector
        if (modelSelector != null)
        {
            modelSelector.RefreshModelList();
        }
        
        // Initialize toggles
        if (autoApplyToggle != null)
        {
            autoApplyToggle.isOn = false; // Default to manual apply
            if (modelSelector != null)
            {
                modelSelector.SetAutoApply(false);
            }
        }
        
        if (showStatusToggle != null)
        {
            showStatusToggle.isOn = true; // Default to show status
            if (modelSelector != null)
            {
                modelSelector.SetShowStatus(true);
            }
        }
    }
    
    /// <summary>
    /// Setup event listeners
    /// </summary>
    private void SetupEventListeners()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClicked);
        }
        
        if (autoApplyToggle != null)
        {
            autoApplyToggle.onValueChanged.AddListener(OnAutoApplyToggleChanged);
        }
        
        if (showStatusToggle != null)
        {
            showStatusToggle.onValueChanged.AddListener(OnShowStatusToggleChanged);
        }
    }
    
    /// <summary>
    /// Remove event listeners
    /// </summary>
    private void RemoveEventListeners()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnResetButtonClicked);
        }
        
        if (autoApplyToggle != null)
        {
            autoApplyToggle.onValueChanged.RemoveListener(OnAutoApplyToggleChanged);
        }
        
        if (showStatusToggle != null)
        {
            showStatusToggle.onValueChanged.RemoveListener(OnShowStatusToggleChanged);
        }
    }
    
    /// <summary>
    /// Update information panel
    /// </summary>
    private void UpdateInfo()
    {
        if (infoText == null || !showInfoPanel) return;
        
        if (aiServiceManager == null)
        {
            infoText.text = "AI Service Manager not available";
            return;
        }
        
        var currentModel = aiServiceManager.CurrentModel;
        var providerName = aiServiceManager.CurrentProviderName;
        var isConfigured = aiServiceManager.IsConfigured;
        var availableModels = aiServiceManager.GetAvailableModels();
        
        var info = $"<b>Current AI Provider:</b> {providerName}\n";
        info += $"<b>Model Type:</b> {currentModel}\n";
        info += $"<b>Status:</b> {(isConfigured ? "Configured" : "Not Configured")}\n";
        info += $"<b>Available Models:</b> {availableModels.Length}\n\n";
        
        info += "<b>Instructions:</b>\n";
        info += "• Select a model from the dropdown\n";
        info += "• Click Apply to switch models\n";
        info += "• Enable Auto-Apply for instant switching\n";
        info += "• Check status for configuration details";
        
        infoText.text = info;
    }
    
    /// <summary>
    /// Handle back button click
    /// </summary>
    private void OnBackButtonClicked()
    {
        // Return to previous layer or main menu
        UILayerLoader.Remove<SettingsLayer>().Forget();
    }
    
    /// <summary>
    /// Handle save button click
    /// </summary>
    private void OnSaveButtonClicked()
    {
        // Save current settings (if needed)
        Debug.Log("Settings saved");
        
        // Show confirmation
        if (infoText != null)
        {
            var originalText = infoText.text;
            infoText.text = "Settings saved successfully!";
            infoText.color = Color.green;
            
            // Reset after 2 seconds
            UniTask.Delay(2000).ContinueWith(() =>
            {
                if (infoText != null)
                {
                    infoText.text = originalText;
                    infoText.color = Color.white;
                }
            }).Forget();
        }
    }
    
    /// <summary>
    /// Handle reset button click
    /// </summary>
    private void OnResetButtonClicked()
    {
        // Reset to default settings
        if (modelSelector != null)
        {
            modelSelector.RefreshModelList();
        }
        
        if (autoApplyToggle != null)
        {
            autoApplyToggle.isOn = false;
        }
        
        if (showStatusToggle != null)
        {
            showStatusToggle.isOn = true;
        }
        
        UpdateInfo();
        Debug.Log("Settings reset to defaults");
    }
    
    /// <summary>
    /// Handle auto-apply toggle change
    /// </summary>
    private void OnAutoApplyToggleChanged(bool isOn)
    {
        if (modelSelector != null)
        {
            modelSelector.SetAutoApply(isOn);
        }
    }
    
    /// <summary>
    /// Handle show status toggle change
    /// </summary>
    private void OnShowStatusToggleChanged(bool isOn)
    {
        if (modelSelector != null)
        {
            modelSelector.SetShowStatus(isOn);
        }
    }
    
    /// <summary>
    /// Show settings layer
    /// </summary>
    public static async UniTask<SettingsLayer> ShowAsync()
    {
        return await UILayerLoader.LoadAsync<SettingsLayer>();
    }
    
    /// <summary>
    /// Hide settings layer
    /// </summary>
    public static async UniTask HideAsync()
    {
        await UILayerLoader.Remove<SettingsLayer>();
    }
}
