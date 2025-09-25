using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// Story Generation Service
/// Uses AI Service Manager to generate story content and illustrations
/// </summary>
public class StoryGenerationService
{
    private readonly AIServiceManager aiServiceManager;
    
    public StoryGenerationService(AIServiceManager aiServiceManager)
    {
        this.aiServiceManager = aiServiceManager ?? throw new ArgumentNullException(nameof(aiServiceManager));
    }
    
    /// <summary>
    /// Generate complete story
    /// </summary>
    /// <param name="title">Story title</param>
    /// <param name="theme">Story theme</param>
    /// <param name="pageCount">Number of pages</param>
    /// <param name="artStyle">Art style</param>
    /// <param name="onProgress">Progress callback</param>
    /// <returns>Generated story data</returns>
    public async UniTask<StoryData> GenerateStoryAsync(string title, string theme, int pageCount, string artStyle,
        Action<float, string> onProgress = null)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(theme) || pageCount <= 0)
        {
            throw new ArgumentException("Invalid story parameters");
        }
        
        // Create story data
        var storyData = new StoryData(title, theme, pageCount);
        
        try
        {
            // 1. Generate story outline
            onProgress?.Invoke(0.1f, "Generating story outline...");
            var outline = await GenerateStoryOutlineAsync(title, theme, pageCount);
            
            // 2. Generate content for each page
            for (int i = 1; i <= pageCount; i++)
            {
                var progress = 0.1f + (0.8f * i / pageCount);
                onProgress?.Invoke(progress, $"Generating page {i} content...");
                
                await GeneratePageContentAsync(storyData, i, outline, artStyle);
            }
            
            // 3. Complete
            onProgress?.Invoke(1.0f, "Story generation completed!");
            return storyData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Story generation failed: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Generate story outline
    /// </summary>
    private async UniTask<string> GenerateStoryOutlineAsync(string title, string theme, int pageCount)
    {
        var prompt = $@"Please create a detailed outline for the following story:

Title: {title}
Theme: {theme}
Pages: {pageCount}

Please provide a brief description for each page, including:
1. The main plot of that page
2. A suitable illustration description for that page
3. Continuity between pages

Please respond in English, in the following format:
Page 1: [Plot description] | [Illustration description]
Page 2: [Plot description] | [Illustration description]
...";

        var response = await aiServiceManager.AskAsync(prompt);
        return response;
    }
    
    /// <summary>
    /// Generate single page content
    /// </summary>
    private async UniTask GeneratePageContentAsync(StoryData storyData, int pageNumber, string outline, string artStyle)
    {
        try
        {
            // 1. Generate page text
            var pageText = await GeneratePageTextAsync(storyData.title, storyData.theme, pageNumber, storyData.totalPages, outline);
            
            // 2. Generate illustration prompt
            var illustrationPrompt = await GenerateIllustrationPromptAsync(storyData.title, storyData.theme, pageNumber, pageText, artStyle);
            
            // 3. Generate illustration
            var illustrations = await aiServiceManager.GeneratePic(illustrationPrompt, 1, "16:9");
            
            // 4. Update page data
            if (illustrations != null && illustrations.Length > 0)
            {
                storyData.UpdatePage(pageNumber, pageText, illustrations[0], illustrationPrompt);
            }
            else
            {
                // If illustration generation fails, only update text
                var page = storyData.GetPage(pageNumber);
                if (page != null)
                {
                    page.SetText(pageText);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to generate content for page {pageNumber}: {ex.Message}");
            // Even if failed, set basic text
            var page = storyData.GetPage(pageNumber);
            if (page != null)
            {
                page.SetText($"Page {pageNumber} content generation failed, please retry.");
            }
        }
    }
    
    /// <summary>
    /// Generate page text content
    /// </summary>
    private async UniTask<string> GeneratePageTextAsync(string title, string theme, int pageNumber, int totalPages, string outline)
    {
        var prompt = $@"Please generate detailed text content for page {pageNumber} of the following story:

Story Title: {title}
Story Theme: {theme}
Total Pages: {totalPages}
Current Page: {pageNumber}

Story Outline:
{outline}

Requirements:
1. Content should match the story theme and overall style
2. Appropriate length for one page display (about 50-100 words)
3. Coherent with previous and next pages
4. Write in English

Please output the page text content directly, without any other explanations:";

        var response = await aiServiceManager.AskAsync(prompt);
        return response?.Trim() ?? $"Page {pageNumber} content";
    }
    
    /// <summary>
    /// Generate illustration prompt
    /// </summary>
    private async UniTask<string> GenerateIllustrationPromptAsync(string title, string theme, int pageNumber, string pageText, string artStyle)
    {
        var prompt = $@"Please generate a detailed illustration prompt for the following story page:

Story Title: {title}
Story Theme: {theme}
Page Content: {pageText}
Art Style: {artStyle}

Requirements:
1. Illustration should be vivid and interesting, suitable for children
2. Style should be consistent with rich colors
3. Should accurately express the page content
4. Describe in English, suitable for AI image generation
5. Must reflect the specified art style: {artStyle}
6. Include art style descriptions (such as: cartoon style, watercolor, digital art, etc.)

Please output the illustration prompt directly:";

        var response = await aiServiceManager.AskAsync(prompt);
        return response?.Trim() ?? $"A beautiful illustration for page {pageNumber} of {title}";
    }
    
    /// <summary>
    /// Generate sample story (for testing)
    /// </summary>
    public async UniTask<StoryData> GenerateSampleStoryAsync(Action<float, string> onProgress = null)
    {
        return await GenerateStoryAsync("The Little Rabbit's Adventure", "Friendship and Courage", 3, "Fairy Tale", onProgress);
    }
}
