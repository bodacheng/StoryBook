using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryData
{
    [Header("Story Basic Information")]
    public string title;
    public string theme;
    public int totalPages;
    public DateTime createdTime;
    
    [Header("Story Pages")]
    public List<StoryPage> pages;
    
    [Header("Generation Status")]
    public bool isComplete;
    public float generationProgress;
    
    public StoryData(string storyTitle, string storyTheme, int pageCount)
    {
        title = storyTitle;
        theme = storyTheme;
        totalPages = pageCount;
        createdTime = DateTime.Now;
        pages = new List<StoryPage>();
        isComplete = false;
        generationProgress = 0f;
        
        // Initialize pages
        for (int i = 0; i < totalPages; i++)
        {
            pages.Add(new StoryPage(i + 1));
        }
    }
    
    /// <summary>
    /// Get page by page number
    /// </summary>
    public StoryPage GetPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > totalPages)
            return null;
        return pages[pageNumber - 1];
    }
    
    /// <summary>
    /// Update page content
    /// </summary>
    public void UpdatePage(int pageNumber, string text, Texture2D illustration, string illustrationPrompt)
    {
        var page = GetPage(pageNumber);
        if (page != null)
        {
            page.SetText(text);
            page.SetIllustration(illustration, illustrationPrompt);
            UpdateProgress();
        }
    }
    
    /// <summary>
    /// Update generation progress
    /// </summary>
    private void UpdateProgress()
    {
        int completedPages = 0;
        foreach (var page in pages)
        {
            if (page.IsComplete())
                completedPages++;
        }
        
        generationProgress = (float)completedPages / totalPages;
        isComplete = generationProgress >= 1f;
    }
    
    /// <summary>
    /// Get story summary
    /// </summary>
    public string GetSummary()
    {
        var summary = $"Title: {title}\nTheme: {theme}\nPages: {totalPages}\n";
        summary += $"Progress: {Mathf.RoundToInt(generationProgress * 100)}%\n";
        summary += $"Created: {createdTime:yyyy-MM-dd HH:mm:ss}";
        return summary;
    }
    
    /// <summary>
    /// Cleanup all resources
    /// </summary>
    public void Cleanup()
    {
        foreach (var page in pages)
        {
            page.Cleanup();
        }
        pages.Clear();
    }
}

