using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class StoryUILayer : UILayer
{
    [Header("UI组件")]
    [SerializeField] private InputField titleInput;
    [SerializeField] private InputField themeInput;
    [SerializeField] private InputField pageCountInput;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text progressText;
    [SerializeField] private Text statusText;
    
    [Header("组件引用")]
    [SerializeField] private StoryManager storyManager;
    
    private bool isGenerating = false;
    
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();
        InitializeUI();
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        if (generateButton != null)
            generateButton.onClick.AddListener(OnGenerateButtonClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        
        if (pageCountInput != null)
            pageCountInput.text = "5"; // 默认5页
        
        UpdateUI();
    }
    
    /// <summary>
    /// 生成按钮点击事件
    /// </summary>
    private async void OnGenerateButtonClicked()
    {
        if (isGenerating || storyManager == null) return;
        
        string title = titleInput?.text?.Trim();
        string theme = themeInput?.text?.Trim();
        string pageCountStr = pageCountInput?.text?.Trim();
        
        // 验证输入
        if (string.IsNullOrEmpty(title))
        {
            ShowStatus("请输入故事标题");
            return;
        }
        
        if (string.IsNullOrEmpty(theme))
        {
            ShowStatus("请输入故事主题");
            return;
        }
        
        if (!int.TryParse(pageCountStr, out int pageCount) || pageCount < 1 || pageCount > 20)
        {
            ShowStatus("请输入有效的页数（1-20）");
            return;
        }
        
        await GenerateStoryAsync(title, theme, pageCount);
    }
    
    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
    private void OnBackButtonClicked()
    {
        Pop();
    }
    
    /// <summary>
    /// 生成故事
    /// </summary>
    private async Task GenerateStoryAsync(string title, string theme, int pageCount)
    {
        isGenerating = true;
        UpdateUI();
        
        try
        {
            ShowStatus("开始生成故事...");
            
            // 订阅进度更新事件
            storyManager.OnProgressUpdated += OnProgressUpdated;
            storyManager.OnStatusUpdated += OnStatusUpdated;
            
            // 生成故事
            var storyData = await storyManager.GenerateStoryAsync(title, theme, pageCount);
            
            if (storyData != null)
            {
                ShowStatus("故事生成完成！");
                // 可以在这里切换到故事显示页面
                await ShowStoryDisplay(storyData);
            }
        }
        catch (Exception e)
        {
            ShowStatus($"生成失败: {e.Message}");
            Debug.LogError($"故事生成失败: {e}");
        }
        finally
        {
            isGenerating = false;
            UpdateUI();
            
            // 取消订阅事件
            if (storyManager != null)
            {
                storyManager.OnProgressUpdated -= OnProgressUpdated;
                storyManager.OnStatusUpdated -= OnStatusUpdated;
            }
        }
    }
    
    /// <summary>
    /// 显示故事内容
    /// </summary>
    private async UniTask ShowStoryDisplay(StoryData storyData)
    {
        // 这里可以切换到故事显示页面
        // 或者直接在当前页面显示故事内容
        Debug.Log($"故事生成完成: {storyData.title}");
        
        // 示例：可以在这里添加切换到故事显示页面的逻辑
        // ProcessesRunner.Main.TrySwitchToStep(StoryDisplayStep).Forget();
    }
    
    /// <summary>
    /// 进度更新回调
    /// </summary>
    private void OnProgressUpdated(float progress)
    {
        if (progressSlider != null)
            progressSlider.value = progress;
        
        if (progressText != null)
            progressText.text = $"进度: {Mathf.RoundToInt(progress * 100)}%";
    }
    
    /// <summary>
    /// 状态更新回调
    /// </summary>
    private void OnStatusUpdated(string status)
    {
        ShowStatus(status);
    }
    
    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void UpdateUI()
    {
        if (generateButton != null)
            generateButton.interactable = !isGenerating;
        
        if (progressSlider != null)
            progressSlider.gameObject.SetActive(isGenerating);
        
        if (progressText != null)
            progressText.gameObject.SetActive(isGenerating);
    }
    
    /// <summary>
    /// 显示状态信息
    /// </summary>
    private void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        
        Debug.Log($"StoryUILayer: {message}");
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        
        // 清理事件订阅
        if (storyManager != null)
        {
            storyManager.OnProgressUpdated -= OnProgressUpdated;
            storyManager.OnStatusUpdated -= OnStatusUpdated;
        }
    }
}

