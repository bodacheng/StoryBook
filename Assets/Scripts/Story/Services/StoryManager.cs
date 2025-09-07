using System;
using System.Threading.Tasks;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private StoryGenerator storyGenerator;
    
    private StoryData currentStory;
    private bool isGenerating = false;
    
    // 事件定义
    public event Action<float> OnProgressUpdated;
    public event Action<string> OnStatusUpdated;
    public event Action<StoryData> OnStoryCompleted;
    
    /// <summary>
    /// 获取当前故事
    /// </summary>
    public StoryData GetCurrentStory()
    {
        return currentStory;
    }
    
    /// <summary>
    /// 检查是否正在生成
    /// </summary>
    public bool IsGenerating()
    {
        return isGenerating;
    }
    
    /// <summary>
    /// 生成故事
    /// </summary>
    public async Task<StoryData> GenerateStoryAsync(string title, string theme, int pageCount)
    {
        isGenerating = true;
        OnStatusUpdated?.Invoke("开始生成故事...");
        
        try
        {
            // 清理之前的故事
            if (currentStory != null)
            {
                currentStory.Cleanup();
            }
            
            // 订阅生成器事件
            if (storyGenerator != null)
            {
                storyGenerator.OnProgressUpdated += OnProgressUpdated;
                storyGenerator.OnStatusUpdated += OnStatusUpdated;
            }
            
            // 生成新故事
            currentStory = await storyGenerator.GenerateStoryAsync(title, theme, pageCount);
            
            OnStatusUpdated?.Invoke("故事生成完成！");
            OnStoryCompleted?.Invoke(currentStory);
            
            return currentStory;
        }
        catch (Exception e)
        {
            OnStatusUpdated?.Invoke($"生成失败: {e.Message}");
            Debug.LogError($"故事生成失败: {e}");
            throw;
        }
        finally
        {
            isGenerating = false;
            
            // 取消订阅事件
            if (storyGenerator != null)
            {
                storyGenerator.OnProgressUpdated -= OnProgressUpdated;
                storyGenerator.OnStatusUpdated -= OnStatusUpdated;
            }
        }
    }
    
    /// <summary>
    /// 更新进度
    /// </summary>
    public void UpdateProgress(float progress)
    {
        OnProgressUpdated?.Invoke(progress);
    }
    
    /// <summary>
    /// 保存故事
    /// </summary>
    public bool SaveStory(string fileName = null)
    {
        if (currentStory == null)
        {
            Debug.LogWarning("没有可保存的故事");
            return false;
        }
        
        try
        {
            string json = JsonUtility.ToJson(currentStory, true);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"{currentStory.title}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }
            
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            System.IO.File.WriteAllText(filePath, json);
            
            Debug.Log($"故事已保存: {fileName}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 加载故事
    /// </summary>
    public StoryData LoadStory(string fileName)
    {
        try
        {
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                currentStory = JsonUtility.FromJson<StoryData>(json);
                
                Debug.Log($"故事已加载: {fileName}");
                return currentStory;
            }
            else
            {
                Debug.LogWarning("文件不存在");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return null;
        }
    }
    
    private void OnDestroy()
    {
        if (currentStory != null)
        {
            currentStory.Cleanup();
        }
    }
}
