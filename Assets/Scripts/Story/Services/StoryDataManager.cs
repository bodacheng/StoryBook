using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 故事数据管理器
/// 负责故事的存储、加载和管理
/// </summary>
public class StoryDataManager : MonoBehaviour
{
    public static StoryDataManager Instance { get; private set; }
    
    [Header("存储设置")]
    [SerializeField] private string storiesFolder = "Stories";
    [SerializeField] private string fileExtension = ".json";
    
    private List<StoryData> stories = new List<StoryData>();
    private string storiesPath;
    
    public event Action<StoryData> OnStoryAdded;
    public event Action<StoryData> OnStoryUpdated;
    public event Action<StoryData> OnStoryRemoved;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化管理器
    /// </summary>
    private void Initialize()
    {
        // 设置存储路径
        storiesPath = Path.Combine(Application.persistentDataPath, storiesFolder);
        
        // 确保目录存在
        if (!Directory.Exists(storiesPath))
        {
            Directory.CreateDirectory(storiesPath);
        }
        
        // 加载现有故事
        LoadAllStories();
    }
    
    /// <summary>
    /// 添加新故事
    /// </summary>
    public void AddStory(StoryData story)
    {
        if (story == null) return;
        
        stories.Add(story);
        SaveStory(story);
        OnStoryAdded?.Invoke(story);
        
        Debug.Log($"Story added: {story.title}");
    }
    
    /// <summary>
    /// 更新故事
    /// </summary>
    public void UpdateStory(StoryData story)
    {
        if (story == null) return;
        
        var index = stories.FindIndex(s => s.title == story.title && s.createdTime == story.createdTime);
        if (index >= 0)
        {
            stories[index] = story;
            SaveStory(story);
            OnStoryUpdated?.Invoke(story);
            
            Debug.Log($"Story updated: {story.title}");
        }
    }
    
    /// <summary>
    /// 删除故事
    /// </summary>
    public void RemoveStory(StoryData story)
    {
        if (story == null) return;
        
        if (stories.Remove(story))
        {
            DeleteStoryFile(story);
            story.Cleanup();
            OnStoryRemoved?.Invoke(story);
            
            Debug.Log($"Story removed: {story.title}");
        }
    }
    
    /// <summary>
    /// 获取所有故事
    /// </summary>
    public List<StoryData> GetAllStories()
    {
        return new List<StoryData>(stories);
    }
    
    /// <summary>
    /// 根据标题查找故事
    /// </summary>
    public StoryData GetStoryByTitle(string title)
    {
        return stories.Find(s => s.title == title);
    }
    
    /// <summary>
    /// 获取故事数量
    /// </summary>
    public int GetStoryCount()
    {
        return stories.Count;
    }
    
    /// <summary>
    /// 保存故事到文件
    /// </summary>
    private void SaveStory(StoryData story)
    {
        try
        {
            var fileName = GetStoryFileName(story);
            var filePath = Path.Combine(storiesPath, fileName);
            
            var json = JsonUtility.ToJson(story, true);
            File.WriteAllText(filePath, json);
            
            Debug.Log($"Story saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save story: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 从文件加载故事
    /// </summary>
    private StoryData LoadStory(string fileName)
    {
        try
        {
            var filePath = Path.Combine(storiesPath, fileName);
            
            if (!File.Exists(filePath))
                return null;
            
            var json = File.ReadAllText(filePath);
            var story = JsonUtility.FromJson<StoryData>(json);
            
            return story;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load story from {fileName}: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 加载所有故事
    /// </summary>
    private void LoadAllStories()
    {
        try
        {
            if (!Directory.Exists(storiesPath))
                return;
            
            var files = Directory.GetFiles(storiesPath, $"*{fileExtension}");
            
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var story = LoadStory(fileName);
                
                if (story != null)
                {
                    stories.Add(story);
                }
            }
            
            Debug.Log($"Loaded {stories.Count} stories from {storiesPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load stories: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 删除故事文件
    /// </summary>
    private void DeleteStoryFile(StoryData story)
    {
        try
        {
            var fileName = GetStoryFileName(story);
            var filePath = Path.Combine(storiesPath, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Story file deleted: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to delete story file: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 获取故事文件名
    /// </summary>
    private string GetStoryFileName(StoryData story)
    {
        var safeTitle = story.title.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        var timestamp = story.createdTime.ToString("yyyyMMdd_HHmmss");
        return $"{safeTitle}_{timestamp}{fileExtension}";
    }
    
    /// <summary>
    /// 清理所有数据
    /// </summary>
    public void ClearAllStories()
    {
        foreach (var story in stories)
        {
            story.Cleanup();
        }
        
        stories.Clear();
        
        // 删除所有文件
        try
        {
            if (Directory.Exists(storiesPath))
            {
                var files = Directory.GetFiles(storiesPath, $"*{fileExtension}");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to clear story files: {ex.Message}");
        }
        
        Debug.Log("All stories cleared");
    }
    
    void OnDestroy()
    {
        // 保存所有故事
        foreach (var story in stories)
        {
            SaveStory(story);
        }
    }
}
