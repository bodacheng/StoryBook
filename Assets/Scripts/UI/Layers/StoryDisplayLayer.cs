using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class StoryDisplayLayer : UILayer
{
    [Header("UI组件")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text themeText;
    [SerializeField] private Text pageCountText;
    [SerializeField] private ScrollRect storyScrollRect;
    [SerializeField] private Transform storyContainer;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button shareButton;
    
    [Header("页面显示设置")]
    [SerializeField] private float pageSpacing = 20f;
    [SerializeField] private Vector2 pageSize = new Vector2(800, 600);
    
    private StoryData currentStory;
    private List<GameObject> pageObjects = new List<GameObject>();
    
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
        
        UpdateStoryInfo();
        CreateStoryPages();
    }
    
    /// <summary>
    /// 显示空状态
    /// </summary>
    private void ShowEmptyState()
    {
        // 清理现有页面
        ClearStoryPages();
        
        // 显示加载或空状态信息
        if (titleText != null)
            titleText.text = "正在生成故事...";
        
        if (themeText != null)
            themeText.text = "请稍候";
        
        if (pageCountText != null)
            pageCountText.text = "准备中...";
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
    /// 创建故事页面显示
    /// </summary>
    private void CreateStoryPages()
    {
        if (storyContainer == null || pagePrefab == null) return;
        
        // 清理现有页面
        ClearStoryPages();
        
        // 创建新页面
        for (int i = 0; i < currentStory.totalPages; i++)
        {
            var page = currentStory.GetPage(i + 1);
            if (page != null)
            {
                CreatePageDisplay(page, i);
            }
        }
        
        // 设置容器大小
        UpdateContainerSize();
    }
    
    /// <summary>
    /// 创建单个页面显示
    /// </summary>
    private void CreatePageDisplay(StoryPage page, int index)
    {
        var pageObj = Instantiate(pagePrefab, storyContainer);
        pageObjects.Add(pageObj);
        
        // 设置页面位置
        var rectTransform = pageObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = pageSize;
            rectTransform.anchoredPosition = new Vector2(0, -index * (pageSize.y + pageSpacing));
        }
        
        // 设置页面编号
        var pageNumberText = pageObj.transform.Find("PageNumber")?.GetComponent<Text>();
        if (pageNumberText != null)
            pageNumberText.text = $"第 {page.pageNumber} 页";
        
        // 设置页面文本
        var pageText = pageObj.transform.Find("PageText")?.GetComponent<Text>();
        if (pageText != null)
            pageText.text = page.text;
        
        // 设置插画
        var illustrationImage = pageObj.transform.Find("Illustration")?.GetComponent<Image>();
        if (illustrationImage != null && page.illustration != null)
        {
            var sprite = Sprite.Create(page.illustration, 
                new Rect(0, 0, page.illustration.width, page.illustration.height), 
                new Vector2(0.5f, 0.5f));
            illustrationImage.sprite = sprite;
        }
        
        // 设置页面状态
        var statusText = pageObj.transform.Find("Status")?.GetComponent<Text>();
        if (statusText != null)
        {
            statusText.text = page.IsComplete() ? "✓ 完成" : "⏳ 生成中...";
            statusText.color = page.IsComplete() ? Color.green : Color.yellow;
        }
    }
    
    /// <summary>
    /// 清理故事页面
    /// </summary>
    private void ClearStoryPages()
    {
        foreach (var pageObj in pageObjects)
        {
            if (pageObj != null)
                Destroy(pageObj);
        }
        pageObjects.Clear();
    }
    
    /// <summary>
    /// 更新容器大小
    /// </summary>
    private void UpdateContainerSize()
    {
        if (storyContainer == null) return;
        
        var containerRect = storyContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            float totalHeight = currentStory.totalPages * (pageSize.y + pageSpacing) - pageSpacing;
            containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, totalHeight);
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
    /// 滚动到指定页面
    /// </summary>
    public void ScrollToPage(int pageNumber)
    {
        if (storyScrollRect == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        float targetY = (pageNumber - 1) * (pageSize.y + pageSpacing);
        float maxY = storyContainer.GetComponent<RectTransform>().sizeDelta.y - storyScrollRect.viewport.rect.height;
        float normalizedY = Mathf.Clamp01(targetY / maxY);
        
        storyScrollRect.verticalNormalizedPosition = 1f - normalizedY;
    }
    
    /// <summary>
    /// 更新页面显示（用于实时更新生成进度）
    /// </summary>
    public void UpdatePageDisplay(int pageNumber)
    {
        if (currentStory == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        var page = currentStory.GetPage(pageNumber);
        if (page == null) return;
        
        int index = pageNumber - 1;
        if (index < pageObjects.Count && pageObjects[index] != null)
        {
            // 更新页面内容
            var pageObj = pageObjects[index];
            
            // 更新文本
            var pageText = pageObj.transform.Find("PageText")?.GetComponent<Text>();
            if (pageText != null)
                pageText.text = page.text;
            
            // 更新插画
            var illustrationImage = pageObj.transform.Find("Illustration")?.GetComponent<Image>();
            if (illustrationImage != null && page.illustration != null)
            {
                var sprite = Sprite.Create(page.illustration, 
                    new Rect(0, 0, page.illustration.width, page.illustration.height), 
                    new Vector2(0.5f, 0.5f));
                illustrationImage.sprite = sprite;
            }
            
            // 更新状态
            var statusText = pageObj.transform.Find("Status")?.GetComponent<Text>();
            if (statusText != null)
            {
                statusText.text = page.IsComplete() ? "✓ 完成" : "⏳ 生成中...";
                statusText.color = page.IsComplete() ? Color.green : Color.yellow;
            }
        }
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        ClearStoryPages();
    }
}

