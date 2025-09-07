using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryDisplayProcess : MainSceneProcess
{
    private StoryDisplayLayer layer;
    private StoryGenerationService storyGenerationService;
    private StoryData currentStory;
    
    public StoryDisplayProcess()
    {
        Step = MainSceneStep.Story;
    }
    
    public override async UniTask ProcessEnter()
    {
        // 初始化故事生成服务
        if (SceneManager.Instance?.GeminiClient != null)
        {
            storyGenerationService = new StoryGenerationService(SceneManager.Instance.GeminiClient);
        }
        else
        {
            Debug.LogError("SceneManager.Instance.GeminiClient is null! Cannot initialize story generation service.");
        }
        
        // 加载显示层
        layer = await UILayerLoader.LoadAsync<StoryDisplayLayer>();
        
        // 设置生成故事函数到显示层
        if (layer != null)
        {
            layer.SetGenerateStoryFunction(GenerateStoryAsync);
            // 显示空状态，等待用户点击生成按钮
            layer.DisplayStory(null);
        }
    }
    
    public override async UniTask ProcessEnd()
    {
        // 清理当前故事数据
        if (currentStory != null)
        {
            currentStory.Cleanup();
            currentStory = null;
        }
        
        // 移除显示层
        await UILayerLoader.Remove<StoryDisplayLayer>();
        layer = null;
        
        // 清理服务
        storyGenerationService = null;
    }
    
    /// <summary>
    /// 生成故事（供StoryDisplayLayer调用）
    /// </summary>
    private async UniTask<StoryData> GenerateStoryAsync()
    {
        if (storyGenerationService == null)
        {
            Debug.LogError("Story generation service is not initialized!");
            return null;
        }
        
        try
        {
            // 生成示例故事（实际项目中可以从用户输入或其他地方获取参数）
            currentStory = await storyGenerationService.GenerateSampleStoryAsync((progress, message) =>
            {
                Debug.Log($"Story Generation Progress: {progress:P0} - {message}");
                // 这里可以更新UI显示进度
            });
            
            // 保存到数据管理器
            if (currentStory != null && StoryDataManager.Instance != null)
            {
                StoryDataManager.Instance.AddStory(currentStory);
                Debug.Log($"Story generated successfully: {currentStory.GetSummary()}");
            }
            
            return currentStory;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to generate story: {ex.Message}");
            return null;
        }
    }
    
    
    /// <summary>
    /// 获取当前故事数据
    /// </summary>
    public StoryData GetCurrentStory()
    {
        return currentStory;
    }
}
