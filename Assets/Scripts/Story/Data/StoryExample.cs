using UnityEngine;

/// <summary>
/// 故事生成系统使用示例
/// </summary>
public class StoryExample : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private StoryManager storyManager;
    
    [Header("示例参数")]
    [SerializeField] private string exampleTitle = "小兔子的冒险";
    [SerializeField] private string exampleTheme = "友谊与勇气";
    [SerializeField] private int examplePageCount = 5;
    
    /// <summary>
    /// 运行示例
    /// </summary>
    [ContextMenu("运行示例")]
    public void RunExample()
    {
        if (storyManager == null)
        {
            Debug.LogError("StoryManager 未设置！");
            return;
        }
        
        Debug.Log("开始运行故事生成示例...");
        Debug.Log($"标题: {exampleTitle}");
        Debug.Log($"主题: {exampleTheme}");
        Debug.Log($"页数: {examplePageCount}");
        
        // 这里可以调用StoryManager的方法来生成故事
        // 由于StoryManager的生成方法是私有的，这里只是示例
        Debug.Log("示例运行完成！请通过UI界面进行实际的故事生成。");
    }
    
    /// <summary>
    /// 快速生成一个测试故事
    /// </summary>
    [ContextMenu("快速测试")]
    public async void QuickTest()
    {
        if (storyManager == null)
        {
            Debug.LogError("StoryManager 未设置！");
            return;
        }
        
        Debug.Log("开始快速测试...");
        
        // 这里可以添加快速测试的逻辑
        // 由于需要UI组件，这里只是示例
        Debug.Log("快速测试完成！");
    }
}

