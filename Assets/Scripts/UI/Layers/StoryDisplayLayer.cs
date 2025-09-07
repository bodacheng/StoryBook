using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class StoryDisplayLayer : UILayer
{
    [Header("UI组件")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text themeText;
    [SerializeField] private Text pageCountText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button generateStoryButton;
    [SerializeField] private Text progressText;
    
    [Header("翻页控制")]
    [SerializeField] private Button prevPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Text pageInfoText;
    
    [Header("单页显示组件")]
    [SerializeField] private Image currentPageImage;
    [SerializeField] private Text currentPageText;
    
    [Header("故事参数输入")]
    [SerializeField] private InputField titleInputField;
    [SerializeField] private InputField themeInputField;
    [SerializeField] private InputField pageCountInputField;
    [SerializeField] private InputField artStyleInputField;
    [SerializeField] private GameObject inputPanel;
    
    private StoryData currentStory;
    
    // 翻页相关
    private int currentPageIndex = 0;
    
    // 故事生成相关
    private System.Func<string, string, int, string, UniTask<StoryData>> generateStoryFunction;
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
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        
        if (shareButton != null)
            shareButton.onClick.AddListener(OnShareButtonClicked);
        
        if (generateStoryButton != null)
            generateStoryButton.onClick.AddListener(OnGenerateStoryButtonClicked);
        
        // 翻页按钮事件
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(OnPrevPageClicked);
        
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(OnNextPageClicked);
    }
    
    /// <summary>
    /// 设置故事生成函数
    /// </summary>
    public void SetGenerateStoryFunction(System.Func<string, string, int, string, UniTask<StoryData>> generateFunction)
    {
        generateStoryFunction = generateFunction;
    }
    
    /// <summary>
    /// 显示故事数据
    /// </summary>
    public void DisplayStory(StoryData story)
    {
        currentStory = story;
        
        if (story == null)
        {
            // 显示空状态
            ShowEmptyState();
            return;
        }
        
        // 隐藏生成按钮
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(false);
        
        // 隐藏进度文本
        if (progressText != null)
            progressText.gameObject.SetActive(false);
        
        // 隐藏输入面板
        if (inputPanel != null)
            inputPanel.SetActive(false);
        
        // 显示翻页控制
        ShowPaginationControls(true);
        
        UpdateStoryInfo();
        
        // 显示第一页
        currentPageIndex = 0;
        ShowCurrentPage();
    }
    
    /// <summary>
    /// 显示空状态
    /// </summary>
    private void ShowEmptyState()
    {
        // 隐藏翻页控制
        ShowPaginationControls(false);
        
        // 清空当前页面显示
        ClearCurrentPageDisplay();
        
        // 显示输入面板
        if (inputPanel != null)
            inputPanel.SetActive(true);
        
        // 显示空状态信息
        if (titleText != null)
            titleText.text = "请输入故事参数";
        
        if (themeText != null)
            themeText.text = "准备就绪";
        
        if (pageCountText != null)
            pageCountText.text = "等待生成...";
        
        // 显示生成按钮
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(true);
        
        // 隐藏进度文本
        if (progressText != null)
            progressText.gameObject.SetActive(false);
        
        // 设置默认值
        SetDefaultInputValues();
    }
    
    /// <summary>
    /// 设置默认输入值
    /// </summary>
    private void SetDefaultInputValues()
    {
        if (titleInputField != null)
            titleInputField.text = "小兔子的冒险";
        
        if (themeInputField != null)
            themeInputField.text = "友谊与勇气";
        
        if (pageCountInputField != null)
            pageCountInputField.text = "3";
        
        if (artStyleInputField != null)
            artStyleInputField.text = "童话";
    }
    
    /// <summary>
    /// 获取用户输入的故事参数
    /// </summary>
    private (string title, string theme, int pageCount, string artStyle) GetStoryParameters()
    {
        string title = titleInputField != null ? titleInputField.text.Trim() : "小兔子的冒险";
        string theme = themeInputField != null ? themeInputField.text.Trim() : "友谊与勇气";
        string artStyle = artStyleInputField != null ? artStyleInputField.text.Trim() : "童话";
        
        int pageCount = 3;
        if (pageCountInputField != null && int.TryParse(pageCountInputField.text.Trim(), out int parsedCount))
        {
            pageCount = Mathf.Max(1, Mathf.Min(10, parsedCount)); // 限制在1-10页之间
        }
        
        return (title, theme, pageCount, artStyle);
    }
    
    /// <summary>
    /// 更新故事基本信息显示
    /// </summary>
    private void UpdateStoryInfo()
    {
        if (titleText != null)
            titleText.text = currentStory.title;
        
        if (themeText != null)
            themeText.text = $"主题: {currentStory.theme}";
        
        if (pageCountText != null)
            pageCountText.text = $"共 {currentStory.totalPages} 页";
    }
    
    /// <summary>
    /// 清空当前页面显示
    /// </summary>
    private void ClearCurrentPageDisplay()
    {
        if (currentPageImage != null)
        {
            currentPageImage.sprite = null;
        }
        
        if (currentPageText != null)
        {
            currentPageText.text = "";
        }
        
        if (pageInfoText != null)
        {
            pageInfoText.text = "";
        }
    }
    
    
    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
    private void OnBackButtonClicked()
    {
        Pop();
    }
    
    /// <summary>
    /// 保存按钮点击事件
    /// </summary>
    private void OnSaveButtonClicked()
    {
        if (currentStory == null)
        {
            Debug.LogWarning("没有可保存的故事");
            return;
        }
        
        SaveStory();
    }
    
    /// <summary>
    /// 分享按钮点击事件
    /// </summary>
    private void OnShareButtonClicked()
    {
        if (currentStory == null)
        {
            Debug.LogWarning("没有可分享的故事");
            return;
        }
        
        ShareStory();
    }
    
    /// <summary>
    /// 生成故事按钮点击事件
    /// </summary>
    private async void OnGenerateStoryButtonClicked()
    {
        if (isGenerating)
        {
            Debug.LogWarning("正在生成故事中，请稍候...");
            return;
        }
        
        if (generateStoryFunction == null)
        {
            Debug.LogError("故事生成函数未设置！");
            ShowMessage("故事生成功能未初始化");
            return;
        }
        
        await GenerateStoryAsync();
    }
    
    /// <summary>
    /// 异步生成故事
    /// </summary>
    private async UniTask GenerateStoryAsync()
    {
        if (isGenerating) return;
        
        isGenerating = true;
        
        try
        {
            // 获取用户输入的参数
            var (title, theme, pageCount, artStyle) = GetStoryParameters();
            
            // 验证输入
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(theme))
            {
                ShowMessage("请输入故事标题和主题");
                isGenerating = false;
                return;
            }
            
            // 显示生成状态
            ShowGeneratingState();
            
            // 调用生成函数，传递参数
            currentStory = await generateStoryFunction(title, theme, pageCount, artStyle);
            
            if (currentStory != null)
            {
                // 显示生成的故事
                DisplayStory(currentStory);
                ShowMessage("故事生成完成！");
            }
            else
            {
                ShowMessage("故事生成失败，请重试");
                ShowEmptyState();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"生成故事时发生错误: {ex.Message}");
            ShowMessage($"生成失败: {ex.Message}");
            ShowEmptyState();
        }
        finally
        {
            isGenerating = false;
        }
    }
    
    /// <summary>
    /// 显示生成中状态
    /// </summary>
    private void ShowGeneratingState()
    {
        // 显示生成中信息
        if (titleText != null)
            titleText.text = "正在生成故事...";
        
        if (themeText != null)
            themeText.text = "请稍候";
        
        if (pageCountText != null)
            pageCountText.text = "生成中...";
        
        // 隐藏生成按钮
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(false);
        
        // 显示进度文本
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "正在生成故事内容，请稍候...";
        }
    }
    
    /// <summary>
    /// 保存故事
    /// </summary>
    private void SaveStory()
    {
        try
        {
            if (StoryDataManager.Instance != null)
            {
                // 使用数据管理器保存
                StoryDataManager.Instance.UpdateStory(currentStory);
                ShowMessage("故事保存成功！");
            }
            else
            {
                // 备用保存方式
                string json = JsonUtility.ToJson(currentStory, true);
                string fileName = $"{currentStory.title}_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
                
                System.IO.File.WriteAllText(filePath, json);
                Debug.Log($"故事已保存: {fileName}");
                ShowMessage("故事保存成功！");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
            ShowMessage($"保存失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 分享故事
    /// </summary>
    private void ShareStory()
    {
        if (currentStory == null) return;
        
        string shareText = $"我创建了一个有趣的故事：{currentStory.title}\n";
        shareText += $"主题：{currentStory.theme}\n";
        shareText += $"共{currentStory.totalPages}页\n";
        shareText += "快来一起阅读吧！";
        
        // 这里可以集成具体的分享功能
        Debug.Log($"分享内容: {shareText}");
        ShowMessage("分享功能开发中...");
    }
    
    /// <summary>
    /// 显示消息
    /// </summary>
    private void ShowMessage(string message)
    {
        Debug.Log($"StoryDisplayLayer: {message}");
        // 这里可以显示Toast或其他UI提示
    }
    
    /// <summary>
    /// 更新页面显示（用于实时更新生成进度）
    /// </summary>
    public void UpdatePageDisplay(int pageNumber)
    {
        if (currentStory == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        // 如果当前显示的就是这个页面，则更新显示
        if (currentPageIndex + 1 == pageNumber)
        {
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// 显示/隐藏翻页控制
    /// </summary>
    private void ShowPaginationControls(bool show)
    {
        if (prevPageButton != null)
            prevPageButton.gameObject.SetActive(show);
        
        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(show);
        
        if (pageInfoText != null)
            pageInfoText.gameObject.SetActive(show);
    }
    
    /// <summary>
    /// 显示当前页面
    /// </summary>
    private void ShowCurrentPage()
    {
        if (currentStory == null) return;
        
        var page = currentStory.GetPage(currentPageIndex + 1);
        if (page == null) return;
        
        // 更新页面文本
        if (currentPageText != null)
            currentPageText.text = page.text;
        
        // 更新页面图像
        if (currentPageImage != null)
        {
            if (page.illustration != null)
            {
                var sprite = Sprite.Create(page.illustration, 
                    new Rect(0, 0, page.illustration.width, page.illustration.height), 
                    new Vector2(0.5f, 0.5f));
                currentPageImage.sprite = sprite;
                currentPageImage.SetNativeSize();
            }
            else
            {
                currentPageImage.sprite = null;
            }
        }
        
        // 更新翻页按钮状态
        UpdatePaginationButtons();
        
        // 更新页面信息显示
        UpdateCurrentPageInfo();
    }
    
    /// <summary>
    /// 更新翻页按钮状态
    /// </summary>
    private void UpdatePaginationButtons()
    {
        if (prevPageButton != null)
            prevPageButton.interactable = currentPageIndex > 0;
        
        if (nextPageButton != null)
            nextPageButton.interactable = currentPageIndex < currentStory.totalPages - 1;
    }
    
    /// <summary>
    /// 更新当前页面信息显示
    /// </summary>
    private void UpdateCurrentPageInfo()
    {
        if (pageInfoText != null && currentStory != null)
        {
            pageInfoText.text = $"第 {currentPageIndex + 1} 页 / 共 {currentStory.totalPages} 页";
        }
    }
    
    /// <summary>
    /// 上一页按钮点击事件
    /// </summary>
    private void OnPrevPageClicked()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// 下一页按钮点击事件
    /// </summary>
    private void OnNextPageClicked()
    {
        if (currentPageIndex < currentStory.totalPages - 1)
        {
            currentPageIndex++;
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// 跳转到指定页面
    /// </summary>
    public void GoToPage(int pageNumber)
    {
        if (currentStory == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        currentPageIndex = pageNumber - 1;
        ShowCurrentPage();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ClearCurrentPageDisplay();
    }
}

