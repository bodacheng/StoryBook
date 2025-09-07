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
        
        // 生成并显示故事
        await GenerateAndDisplayStory();
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
    /// 生成并显示故事
    /// </summary>
    private async UniTask GenerateAndDisplayStory()
    {
        if (storyGenerationService == null || layer == null)
        {
            Debug.LogError("Story generation service or layer is not initialized!");
            return;
        }
        
        try
        {
            // 显示加载状态
            layer.DisplayStory(null); // 显示空状态
            
            // 生成示例故事（实际项目中可以从用户输入或其他地方获取参数）
            currentStory = await storyGenerationService.GenerateSampleStoryAsync((progress, message) =>
            {
                Debug.Log($"Story Generation Progress: {progress:P0} - {message}");
                // 这里可以更新UI显示进度
            });
            
            // 显示生成的故事
            if (currentStory != null)
            {
                layer.DisplayStory(currentStory);
                
                // 保存到数据管理器
                if (StoryDataManager.Instance != null)
                {
                    StoryDataManager.Instance.AddStory(currentStory);
                }
                
                Debug.Log($"Story generated successfully: {currentStory.GetSummary()}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to generate story: {ex.Message}");
            // 显示错误状态
            ShowErrorState($"故事生成失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 显示错误状态
    /// </summary>
    private void ShowErrorState(string errorMessage)
    {
        if (layer != null)
        {
            // 创建一个错误状态的故事数据
            var errorStory = new StoryData("生成失败", "错误", 1);
            errorStory.UpdatePage(1, errorMessage, null, "");
            layer.DisplayStory(errorStory);
        }
    }
    
    /// <summary>
    /// 重新生成故事
    /// </summary>
    public async UniTask RegenerateStoryAsync()
    {
        if (storyGenerationService != null && layer != null)
        {
            await GenerateAndDisplayStory();
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
