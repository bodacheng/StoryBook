using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StoryGenerator : MonoBehaviour
{
    [Header("API配置")]
    [SerializeField] private GeminiConfig geminiConfig;
    
    private GeminiClient geminiClient;
    
    [Header("生成设置")]
    [SerializeField] private string storyStyle = "儿童插画风格，温馨可爱";
    [SerializeField] private int maxRetries = 3;
    
    // 进度更新事件
    public event Action<float> OnProgressUpdated;
    public event Action<string> OnStatusUpdated;
    
    private void Awake()
    {
        if (geminiConfig != null)
        {
            geminiClient = new GeminiClient(geminiConfig);
        }
        else
        {
            Debug.LogError("GeminiConfig is not assigned!");
        }
    }
    
    /// <summary>
    /// 生成完整故事
    /// </summary>
    public async Task<StoryData> GenerateStoryAsync(string title, string theme, int pageCount)
    {
        var storyData = new StoryData(title, theme, pageCount);
        
        try
        {
            OnStatusUpdated?.Invoke("正在生成故事大纲...");
            
            // 1. 生成故事大纲
            string storyOutline = await GenerateStoryOutlineAsync(title, theme, pageCount);
            Debug.Log($"故事大纲生成完成: {storyOutline}");
            
            OnStatusUpdated?.Invoke("开始生成页面内容...");
            
            // 2. 生成每页的文本和插画
            for (int i = 1; i <= pageCount; i++)
            {
                OnStatusUpdated?.Invoke($"正在生成第 {i} 页...");
                await GeneratePageAsync(storyData, i, storyOutline);
                
                // 更新进度
                float progress = (float)i / pageCount;
                OnProgressUpdated?.Invoke(progress);
            }
            
            Debug.Log($"故事生成完成: {title}");
            return storyData;
        }
        catch (Exception e)
        {
            Debug.LogError($"故事生成失败: {e.Message}");
            storyData.Cleanup();
            throw;
        }
    }
    
    /// <summary>
    /// 生成故事大纲
    /// </summary>
    private async Task<string> GenerateStoryOutlineAsync(string title, string theme, int pageCount)
    {
        string prompt = $@"请为以下故事创建一个{pageCount}页的详细大纲：

标题：{title}
主题：{theme}

要求：
1. 每页都要有明确的故事情节
2. 故事要有起承转合，逻辑连贯
3. 适合儿童阅读
4. 每页内容要简洁明了

请直接返回故事大纲，格式如下：
第1页：[情节描述]
第2页：[情节描述]
...
第{pageCount}页：[情节描述]";

        return await geminiClient.AskAsync(prompt);
    }
    
    /// <summary>
    /// 生成单页内容
    /// </summary>
    private async Task GeneratePageAsync(StoryData storyData, int pageNumber, string storyOutline)
    {
        try
        {
            // 生成页面文本
            string pageText = await GeneratePageTextAsync(storyData.title, storyData.theme, pageNumber, storyOutline);
            
            // 生成插画提示词
            string illustrationPrompt = await GenerateIllustrationPromptAsync(pageText, storyData.theme);
            
            // 生成插画
            Texture2D[] illustrations = await geminiClient.Imagen4Service.GenerateImagesImagenAsync(
                illustrationPrompt,
                1,
                "16:9",
                1);
            
            // 更新故事数据
            if (illustrations != null && illustrations.Length > 0)
            {
                storyData.UpdatePage(pageNumber, pageText, illustrations[0], illustrationPrompt);
                Debug.Log($"第{pageNumber}页生成完成");
            }
            else
            {
                Debug.LogError($"第{pageNumber}页插画生成失败");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"第{pageNumber}页生成失败: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 生成页面文本
    /// </summary>
    private async Task<string> GeneratePageTextAsync(string title, string theme, int pageNumber, string storyOutline)
    {
        string prompt = $@"基于以下信息，为故事的第{pageNumber}页生成具体的文本内容：

故事标题：{title}
故事主题：{theme}
故事大纲：
{storyOutline}

要求：
1. 文本要简洁生动，适合儿童阅读
2. 长度控制在50-100字
3. 要与故事大纲中第{pageNumber}页的描述相符
4. 语言要温馨有趣

请直接返回第{pageNumber}页的文本内容：";

        return await geminiClient.AskAsync(prompt);
    }
    
    /// <summary>
    /// 生成插画提示词
    /// </summary>
    private async Task<string> GenerateIllustrationPromptAsync(string pageText, string theme)
    {
        string prompt = $@"基于以下文本内容，生成一个详细的插画提示词：

文本内容：{pageText}
故事主题：{theme}

要求：
1. 提示词要详细描述画面内容
2. 风格：{storyStyle}
3. 画面要生动有趣，符合儿童审美
4. 要体现文本中的关键情节
5. 用英文生成提示词

请直接返回插画提示词：";

        string illustrationPrompt = await geminiClient.AskAsync(prompt);
        
        // 确保提示词是英文
        if (IsChinese(illustrationPrompt))
        {
            string translatePrompt = $@"请将以下中文提示词翻译成英文：

{illustrationPrompt}

要求：
1. 保持原意不变
2. 使用专业的插画描述词汇
3. 直接返回英文翻译结果";

            illustrationPrompt = await geminiClient.AskAsync(translatePrompt);
        }
        
        return illustrationPrompt;
    }
    
    /// <summary>
    /// 检查文本是否包含中文
    /// </summary>
    private bool IsChinese(string text)
    {
        foreach (char c in text)
        {
            if (c >= 0x4e00 && c <= 0x9fff)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取API Key
    /// </summary>
    private string GetApiKey()
    {
        return geminiConfig?.ApiKey ?? "";
    }
}
