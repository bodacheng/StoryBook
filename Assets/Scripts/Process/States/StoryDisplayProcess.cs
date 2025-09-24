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
        // Initialize story generation service
        if (SceneManager.Instance?.GeminiClient != null)
        {
            storyGenerationService = new StoryGenerationService(SceneManager.Instance.GeminiClient);
        }
        else
        {
            Debug.LogError("SceneManager.Instance.GeminiClient is null! Cannot initialize story generation service.");
        }
        
        // Load display layer
        layer = await UILayerLoader.LoadAsync<StoryDisplayLayer>();
        
        // Set story generation function to display layer
        if (layer != null)
        {
            layer.SetGenerateStoryFunction(GenerateStoryAsync);
            // Show empty state, wait for user to click generate button
            layer.DisplayStory(null);
        }
    }
    
    public override async UniTask ProcessEnd()
    {
        // Clean up current story data
        if (currentStory != null)
        {
            currentStory.Cleanup();
            currentStory = null;
        }
        
        // Remove display layer
        await UILayerLoader.Remove<StoryDisplayLayer>();
        layer = null;
        
        // Clean up service
        storyGenerationService = null;
    }
    
    /// <summary>
    /// Generate story (called by StoryDisplayLayer)
    /// </summary>
    private async UniTask<StoryData> GenerateStoryAsync(string title, string theme, int pageCount, string artStyle)
    {
        if (storyGenerationService == null)
        {
            Debug.LogError("Story generation service is not initialized!");
            return null;
        }
        
        try
        {
            // Generate story using passed parameters
            currentStory = await storyGenerationService.GenerateStoryAsync(title, theme, pageCount, artStyle, (progress, message) =>
            {
                Debug.Log($"Story Generation Progress: {progress:P0} - {message}");
                // Here you can update UI progress display
            });
            
            // Save to data manager
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
    /// Get current story data
    /// </summary>
    public StoryData GetCurrentStory()
    {
        return currentStory;
    }
}
