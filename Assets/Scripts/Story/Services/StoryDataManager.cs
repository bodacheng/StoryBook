using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Story Data Manager
/// Responsible for story storage, loading and management
/// </summary>
public class StoryDataManager : MonoBehaviour
{
    public static StoryDataManager Instance { get; private set; }
    
    [Header("Storage Settings")]
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
    /// Initialize manager
    /// </summary>
    private void Initialize()
    {
        // Set storage path
        storiesPath = Path.Combine(Application.persistentDataPath, storiesFolder);
        
        // Ensure directory exists
        if (!Directory.Exists(storiesPath))
        {
            Directory.CreateDirectory(storiesPath);
        }
        
        // Load existing stories
        LoadAllStories();
    }
    
    /// <summary>
    /// Add new story
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
    /// Update story
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
    /// Remove story
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
    /// Get all stories
    /// </summary>
    public List<StoryData> GetAllStories()
    {
        return new List<StoryData>(stories);
    }
    
    /// <summary>
    /// Find story by title
    /// </summary>
    public StoryData GetStoryByTitle(string title)
    {
        return stories.Find(s => s.title == title);
    }
    
    /// <summary>
    /// Get story count
    /// </summary>
    public int GetStoryCount()
    {
        return stories.Count;
    }
    
    /// <summary>
    /// Save story to file
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
    /// Load story from file
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
    /// Load all stories
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
    /// Delete story file
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
    /// Get story file name
    /// </summary>
    private string GetStoryFileName(StoryData story)
    {
        var safeTitle = story.title.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        var timestamp = story.createdTime.ToString("yyyyMMdd_HHmmss");
        return $"{safeTitle}_{timestamp}{fileExtension}";
    }
    
    /// <summary>
    /// Clear all data
    /// </summary>
    public void ClearAllStories()
    {
        foreach (var story in stories)
        {
            story.Cleanup();
        }
        
        stories.Clear();
        
        // Delete all files
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
        // Save all stories
        foreach (var story in stories)
        {
            SaveStory(story);
        }
    }
}
