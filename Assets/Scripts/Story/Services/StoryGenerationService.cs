using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 故事生成服务
/// 使用GeminiClient生成故事内容和插画
/// </summary>
public class StoryGenerationService
{
    private readonly GeminiClient geminiClient;
    
    public StoryGenerationService(GeminiClient client)
    {
        geminiClient = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    /// <summary>
    /// 生成完整的故事
    /// </summary>
    /// <param name="title">故事标题</param>
    /// <param name="theme">故事主题</param>
    /// <param name="pageCount">页数</param>
    /// <param name="artStyle">作画风格</param>
    /// <param name="onProgress">进度回调</param>
    /// <returns>生成的故事数据</returns>
    public async UniTask<StoryData> GenerateStoryAsync(string title, string theme, int pageCount, string artStyle,
        Action<float, string> onProgress = null)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(theme) || pageCount <= 0)
        {
            throw new ArgumentException("Invalid story parameters");
        }
        
        // 创建故事数据
        var storyData = new StoryData(title, theme, pageCount);
        
        try
        {
            // 1. 生成故事大纲
            onProgress?.Invoke(0.1f, "正在生成故事大纲...");
            var outline = await GenerateStoryOutlineAsync(title, theme, pageCount);
            
            // 2. 为每页生成内容
            for (int i = 1; i <= pageCount; i++)
            {
                var progress = 0.1f + (0.8f * i / pageCount);
                onProgress?.Invoke(progress, $"正在生成第 {i} 页内容...");
                
                await GeneratePageContentAsync(storyData, i, outline, artStyle);
            }
            
            // 3. 完成
            onProgress?.Invoke(1.0f, "故事生成完成！");
            return storyData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"故事生成失败: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 生成故事大纲
    /// </summary>
    private async UniTask<string> GenerateStoryOutlineAsync(string title, string theme, int pageCount)
    {
        var prompt = $@"请为以下故事创建一个详细的大纲：

标题：{title}
主题：{theme}
页数：{pageCount}

请为每一页提供一个简短的描述，包括：
1. 该页的主要情节
2. 适合该页的插画描述
3. 页面之间的连贯性

请用中文回答，格式如下：
第1页：[情节描述] | [插画描述]
第2页：[情节描述] | [插画描述]
...";

        var response = await geminiClient.AskAsync(prompt);
        return response;
    }
    
    /// <summary>
    /// 生成单页内容
    /// </summary>
    private async UniTask GeneratePageContentAsync(StoryData storyData, int pageNumber, string outline, string artStyle)
    {
        try
        {
            // 1. 生成页面文本
            var pageText = await GeneratePageTextAsync(storyData.title, storyData.theme, pageNumber, storyData.totalPages, outline);
            
            // 2. 生成插画提示词
            var illustrationPrompt = await GenerateIllustrationPromptAsync(storyData.title, storyData.theme, pageNumber, pageText, artStyle);
            
            // 3. 生成插画
            var illustrations = await geminiClient.GeneratePic(illustrationPrompt, 1, "16:9");
            
            // 4. 更新页面数据
            if (illustrations != null && illustrations.Length > 0)
            {
                storyData.UpdatePage(pageNumber, pageText, illustrations[0], illustrationPrompt);
            }
            else
            {
                // 如果插画生成失败，只更新文本
                var page = storyData.GetPage(pageNumber);
                if (page != null)
                {
                    page.SetText(pageText);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"生成第 {pageNumber} 页内容失败: {ex.Message}");
            // 即使失败也要设置基本文本
            var page = storyData.GetPage(pageNumber);
            if (page != null)
            {
                page.SetText($"第 {pageNumber} 页内容生成失败，请重试。");
            }
        }
    }
    
    /// <summary>
    /// 生成页面文本内容
    /// </summary>
    private async UniTask<string> GeneratePageTextAsync(string title, string theme, int pageNumber, int totalPages, string outline)
    {
        var prompt = $@"请为以下故事生成第 {pageNumber} 页的详细文本内容：

故事标题：{title}
故事主题：{theme}
总页数：{totalPages}
当前页数：{pageNumber}

故事大纲：
{outline}

要求：
1. 内容要符合故事主题和整体风格
2. 长度适中，适合一页显示（约50-100字）
3. 与前后页内容连贯
4. 用日文写作

请直接输出页面文本内容，不要包含其他说明：";

        var response = await geminiClient.AskAsync(prompt);
        return response?.Trim() ?? $"第 {pageNumber} 页内容";
    }
    
    /// <summary>
    /// 生成插画提示词
    /// </summary>
    private async UniTask<string> GenerateIllustrationPromptAsync(string title, string theme, int pageNumber, string pageText, string artStyle)
    {
        var prompt = $@"请为以下故事页面生成一个详细的插画提示词：

故事标题：{title}
故事主题：{theme}
页面内容：{pageText}
作画风格：{artStyle}

要求：
1. 插画要生动有趣，适合儿童
2. 风格要统一，色彩丰富
3. 要能准确表达页面内容
4. 用英文描述，适合AI图像生成
5. 必须体现指定的作画风格：{artStyle}
6. 包含艺术风格描述（如：cartoon style, watercolor, digital art等）

请直接输出插画提示词：";

        var response = await geminiClient.AskAsync(prompt);
        return response?.Trim() ?? $"A beautiful illustration for page {pageNumber} of {title}";
    }
    
    /// <summary>
    /// 生成示例故事（用于测试）
    /// </summary>
    public async UniTask<StoryData> GenerateSampleStoryAsync(Action<float, string> onProgress = null)
    {
        return await GenerateStoryAsync("小兔子的冒险", "友谊与勇气", 3, "童话", onProgress);
    }
}
