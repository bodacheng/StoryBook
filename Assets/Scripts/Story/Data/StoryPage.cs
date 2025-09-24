using UnityEngine;

[System.Serializable]
public class StoryPage
{
    [Header("Page Content")]
    public int pageNumber;
    public string text;
    public Texture2D illustration;
    
    [Header("Generation Info")]
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
    /// Set page text content
    /// </summary>
    public void SetText(string pageText)
    {
        text = pageText;
    }
    
    /// <summary>
    /// Set illustration
    /// </summary>
    public void SetIllustration(Texture2D image, string prompt)
    {
        illustration = image;
        illustrationPrompt = prompt;
        isGenerated = true;
    }
    
    /// <summary>
    /// Check if page is completely generated
    /// </summary>
    public bool IsComplete()
    {
        return !string.IsNullOrEmpty(text) && illustration != null && isGenerated;
    }
    
    /// <summary>
    /// Cleanup resources
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

