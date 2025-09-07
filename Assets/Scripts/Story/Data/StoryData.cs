using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryData
{
    [Header("故事基本信息")]
    public string title;
    public string theme;
    public int totalPages;
    public DateTime createdTime;
    
    [Header("故事页面")]
    public List<StoryPage> pages;
    
    [Header("生成状态")]
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
        
        // 初始化页面
        for (int i = 0; i < totalPages; i++)
        {
            pages.Add(new StoryPage(i + 1));
        }
    }
    
    /// <summary>
    /// 获取指定页码的页面
    /// </summary>
    public StoryPage GetPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > totalPages)
            return null;
        return pages[pageNumber - 1];
    }
    
    /// <summary>
    /// 更新页面内容
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
    /// 更新生成进度
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
    /// 获取故事摘要
    /// </summary>
    public string GetSummary()
    {
        var summary = $"标题: {title}\n主题: {theme}\n页数: {totalPages}\n";
        summary += $"进度: {Mathf.RoundToInt(generationProgress * 100)}%\n";
        summary += $"创建时间: {createdTime:yyyy-MM-dd HH:mm:ss}";
        return summary;
    }
    
    /// <summary>
    /// 清理所有资源
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

