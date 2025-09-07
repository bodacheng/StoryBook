using UnityEngine;

[System.Serializable]
public class StoryPage
{
    [Header("页面内容")]
    public int pageNumber;
    public string text;
    public Texture2D illustration;
    
    [Header("生成信息")]
    public string illustrationPrompt;
    public bool isGenerated;
    
    public StoryPage(int pageNum)
    {
        pageNumber = pageNum;
        text = "";
        illustration = null;
        illustrationPrompt = "";
        isGenerated = false;
    }
    
    /// <summary>
    /// 设置页面文本内容
    /// </summary>
    public void SetText(string pageText)
    {
        text = pageText;
    }
    
    /// <summary>
    /// 设置插画
    /// </summary>
    public void SetIllustration(Texture2D image, string prompt)
    {
        illustration = image;
        illustrationPrompt = prompt;
        isGenerated = true;
    }
    
    /// <summary>
    /// 检查页面是否完整生成
    /// </summary>
    public bool IsComplete()
    {
        return !string.IsNullOrEmpty(text) && illustration != null && isGenerated;
    }
    
    /// <summary>
    /// 清理资源
    /// </summary>
    public void Cleanup()
    {
        if (illustration != null)
        {
            Object.DestroyImmediate(illustration);
            illustration = null;
        }
    }
}

