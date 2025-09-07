using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class StoryDisplayLayerTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private StoryDisplayLayer storyDisplayLayer;
    [SerializeField] private Button testButton;
    [SerializeField] private Button clearButton;
    
    private void Start()
    {
        if (testButton != null)
            testButton.onClick.AddListener(TestStoryDisplay);
        
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearStory);
    }
    
    /// <summary>
    /// 测试故事显示功能
    /// </summary>
    public void TestStoryDisplay()
    {
        if (storyDisplayLayer == null)
        {
            Debug.LogError("StoryDisplayLayer is not assigned!");
            return;
        }
        
        // 创建测试故事数据
        var testStory = CreateTestStory();
        
        // 显示故事
        storyDisplayLayer.DisplayStory(testStory);
        
        Debug.Log("Test story displayed successfully!");
    }
    
    /// <summary>
    /// 清除故事显示
    /// </summary>
    public void ClearStory()
    {
        if (storyDisplayLayer != null)
        {
            storyDisplayLayer.DisplayStory(null);
            Debug.Log("Story cleared!");
        }
    }
    
    /// <summary>
    /// 创建测试故事数据
    /// </summary>
    private StoryData CreateTestStory()
    {
        var story = new StoryData("测试故事", "冒险", 3);
        
        // 添加测试页面
        for (int i = 1; i <= 3; i++)
        {
            var page = story.GetPage(i);
            if (page != null)
            {
                page.SetText($"这是第 {i} 页的内容。\n\n这里是一个关于冒险的故事，讲述了主人公的精彩经历。");
                page.SetIllustration(null, $"第{i}页插画描述");
            }
        }
        
        return story;
    }
    
    /// <summary>
    /// 测试页面更新功能
    /// </summary>
    public async void TestPageUpdate()
    {
        if (storyDisplayLayer == null) return;
        
        var testStory = CreateTestStory();
        storyDisplayLayer.DisplayStory(testStory);
        
        // 模拟页面逐步更新
        for (int i = 1; i <= 3; i++)
        {
            await UniTask.Delay(1000); // 等待1秒
            
            var page = testStory.GetPage(i);
            if (page != null)
            {
                page.SetText($"更新后的第 {i} 页内容！\n\n这是更新后的内容，包含了更多详细信息。");
                storyDisplayLayer.UpdatePageDisplay(i);
            }
        }
    }
    
    /// <summary>
    /// 测试滚动功能
    /// </summary>
    public void TestScrollToPage()
    {
        if (storyDisplayLayer == null) return;
        
        // 滚动到第2页
        storyDisplayLayer.ScrollToPage(2);
        Debug.Log("Scrolled to page 2");
    }
    
    private void OnDestroy()
    {
        if (testButton != null)
            testButton.onClick.RemoveListener(TestStoryDisplay);
        
        if (clearButton != null)
            clearButton.onClick.RemoveListener(ClearStory);
    }
}
