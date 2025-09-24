using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class StoryDisplayLayer : UILayer
{
    [Header("UI Components")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text themeText;
    [SerializeField] private Text pageCountText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button generateStoryButton;
    [SerializeField] private Text progressText;
    
    [Header("Pagination Controls")]
    [SerializeField] private Button prevPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Text pageInfoText;
    
    [Header("Single Page Display Components")]
    [SerializeField] private Image currentPageImage;
    [SerializeField] private Text currentPageText;
    
    [Header("Story Parameter Input")]
    [SerializeField] private InputField titleInputField;
    [SerializeField] private InputField themeInputField;
    [SerializeField] private InputField pageCountInputField;
    [SerializeField] private InputField artStyleInputField;
    [SerializeField] private GameObject inputPanel;
    
    private StoryData currentStory;
    
    // Pagination related
    private int currentPageIndex = 0;
    
    // Story generation related
    private System.Func<string, string, int, string, UniTask<StoryData>> generateStoryFunction;
    private bool isGenerating = false;
    
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();
        InitializeUI();
    }
    
    /// <summary>
    /// Initialize UI
    /// </summary>
    private void InitializeUI()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        
        if (shareButton != null)
            shareButton.onClick.AddListener(OnShareButtonClicked);
        
        if (generateStoryButton != null)
            generateStoryButton.onClick.AddListener(OnGenerateStoryButtonClicked);
        
        // Pagination button events
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(OnPrevPageClicked);
        
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(OnNextPageClicked);
    }
    
    /// <summary>
    /// Set story generation function
    /// </summary>
    public void SetGenerateStoryFunction(System.Func<string, string, int, string, UniTask<StoryData>> generateFunction)
    {
        generateStoryFunction = generateFunction;
    }
    
    /// <summary>
    /// Display story data
    /// </summary>
    public void DisplayStory(StoryData story)
    {
        currentStory = story;
        
        if (story == null)
        {
            // Show empty state
            ShowEmptyState();
            return;
        }
        
        // Hide generate button
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(false);
        
        // Hide progress text
        if (progressText != null)
            progressText.gameObject.SetActive(false);
        
        // Hide input panel
        if (inputPanel != null)
            inputPanel.SetActive(false);
        
        // Show pagination controls
        ShowPaginationControls(true);
        
        UpdateStoryInfo();
        
        // Show first page
        currentPageIndex = 0;
        ShowCurrentPage();
    }
    
    /// <summary>
    /// Show empty state
    /// </summary>
    private void ShowEmptyState()
    {
        // Hide pagination controls
        ShowPaginationControls(false);
        
        // Clear current page display
        ClearCurrentPageDisplay();
        
        // Show input panel
        if (inputPanel != null)
            inputPanel.SetActive(true);
        
        // Show empty state information
        if (titleText != null)
            titleText.text = "Please enter story parameters";
        
        if (themeText != null)
            themeText.text = "Ready";
        
        if (pageCountText != null)
            pageCountText.text = "Waiting for generation...";
        
        // Show generate button
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(true);
        
        // Hide progress text
        if (progressText != null)
            progressText.gameObject.SetActive(false);
        
        // Set default values
        SetDefaultInputValues();
    }
    
    /// <summary>
    /// Set default input values
    /// </summary>
    private void SetDefaultInputValues()
    {
        if (titleInputField != null)
            titleInputField.text = "The Little Rabbit's Adventure";
        
        if (themeInputField != null)
            themeInputField.text = "Friendship and Courage";
        
        if (pageCountInputField != null)
            pageCountInputField.text = "3";
        
        if (artStyleInputField != null)
            artStyleInputField.text = "Fairy Tale";
    }
    
    /// <summary>
    /// Get user input story parameters
    /// </summary>
    private (string title, string theme, int pageCount, string artStyle) GetStoryParameters()
    {
        string title = titleInputField != null ? titleInputField.text.Trim() : "The Little Rabbit's Adventure";
        string theme = themeInputField != null ? themeInputField.text.Trim() : "Friendship and Courage";
        string artStyle = artStyleInputField != null ? artStyleInputField.text.Trim() : "Fairy Tale";
        
        int pageCount = 3;
        if (pageCountInputField != null && int.TryParse(pageCountInputField.text.Trim(), out int parsedCount))
        {
            pageCount = Mathf.Max(1, Mathf.Min(10, parsedCount)); // Limit between 1-10 pages
        }
        
        return (title, theme, pageCount, artStyle);
    }
    
    /// <summary>
    /// Update story basic information display
    /// </summary>
    private void UpdateStoryInfo()
    {
        if (titleText != null)
            titleText.text = currentStory.title;
        
        if (themeText != null)
            themeText.text = $"Theme: {currentStory.theme}";
        
        if (pageCountText != null)
            pageCountText.text = $"Total {currentStory.totalPages} pages";
    }
    
    /// <summary>
    /// Clear current page display
    /// </summary>
    private void ClearCurrentPageDisplay()
    {
        if (currentPageImage != null)
        {
            currentPageImage.sprite = null;
        }
        
        if (currentPageText != null)
        {
            currentPageText.text = "";
        }
        
        if (pageInfoText != null)
        {
            pageInfoText.text = "";
        }
    }
    
    
    /// <summary>
    /// Back button click event
    /// </summary>
    private void OnBackButtonClicked()
    {
        Pop();
    }
    
    /// <summary>
    /// Save button click event
    /// </summary>
    private void OnSaveButtonClicked()
    {
        if (currentStory == null)
        {
            Debug.LogWarning("No story to save");
            return;
        }
        
        SaveStory();
    }
    
    /// <summary>
    /// Share button click event
    /// </summary>
    private void OnShareButtonClicked()
    {
        if (currentStory == null)
        {
            Debug.LogWarning("No story to share");
            return;
        }
        
        ShareStory();
    }
    
    /// <summary>
    /// Generate story button click event
    /// </summary>
    private async void OnGenerateStoryButtonClicked()
    {
        if (isGenerating)
        {
            Debug.LogWarning("Story is being generated, please wait...");
            return;
        }
        
        if (generateStoryFunction == null)
        {
            Debug.LogError("Story generation function not set!");
            ShowMessage("Story generation feature not initialized");
            return;
        }
        
        await GenerateStoryAsync();
    }
    
    /// <summary>
    /// Asynchronously generate story
    /// </summary>
    private async UniTask GenerateStoryAsync()
    {
        if (isGenerating) return;
        
        isGenerating = true;
        
        try
        {
            // Get user input parameters
            var (title, theme, pageCount, artStyle) = GetStoryParameters();
            
            // Validate input
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(theme))
            {
                ShowMessage("Please enter story title and theme");
                isGenerating = false;
                return;
            }
            
            // Show generating state
            ShowGeneratingState();
            
            // Call generation function with parameters
            currentStory = await generateStoryFunction(title, theme, pageCount, artStyle);
            
            if (currentStory != null)
            {
                // Display generated story
                DisplayStory(currentStory);
                ShowMessage("Story generation completed!");
            }
            else
            {
                ShowMessage("Story generation failed, please retry");
                ShowEmptyState();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error occurred while generating story: {ex.Message}");
            ShowMessage($"Generation failed: {ex.Message}");
            ShowEmptyState();
        }
        finally
        {
            isGenerating = false;
        }
    }
    
    /// <summary>
    /// Show generating state
    /// </summary>
    private void ShowGeneratingState()
    {
        // Show generating information
        if (titleText != null)
            titleText.text = "Generating story...";
        
        if (themeText != null)
            themeText.text = "Please wait";
        
        if (pageCountText != null)
            pageCountText.text = "Generating...";
        
        // Hide generate button
        if (generateStoryButton != null)
            generateStoryButton.gameObject.SetActive(false);
        
        // Show progress text
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "Generating story content, please wait...";
        }
    }
    
    /// <summary>
    /// Save story
    /// </summary>
    private void SaveStory()
    {
        try
        {
            if (StoryDataManager.Instance != null)
            {
                // Use data manager to save
                StoryDataManager.Instance.UpdateStory(currentStory);
                ShowMessage("Story saved successfully!");
            }
            else
            {
                // Fallback save method
                string json = JsonUtility.ToJson(currentStory, true);
                string fileName = $"{currentStory.title}_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
                
                System.IO.File.WriteAllText(filePath, json);
                Debug.Log($"Story saved: {fileName}");
                ShowMessage("Story saved successfully!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            ShowMessage($"Save failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Share story
    /// </summary>
    private void ShareStory()
    {
        if (currentStory == null) return;
        
        string shareText = $"I created an interesting story: {currentStory.title}\n";
        shareText += $"Theme: {currentStory.theme}\n";
        shareText += $"Total {currentStory.totalPages} pages\n";
        shareText += "Come read it together!";
        
        // Here you can integrate specific sharing functionality
        Debug.Log($"Share content: {shareText}");
        ShowMessage("Sharing feature under development...");
    }
    
    /// <summary>
    /// Show message
    /// </summary>
    private void ShowMessage(string message)
    {
        Debug.Log($"StoryDisplayLayer: {message}");
        // Here you can show Toast or other UI prompts
    }
    
    /// <summary>
    /// Update page display (for real-time generation progress updates)
    /// </summary>
    public void UpdatePageDisplay(int pageNumber)
    {
        if (currentStory == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        // If currently displaying this page, update the display
        if (currentPageIndex + 1 == pageNumber)
        {
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// Show/hide pagination controls
    /// </summary>
    private void ShowPaginationControls(bool show)
    {
        if (prevPageButton != null)
            prevPageButton.gameObject.SetActive(show);
        
        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(show);
        
        if (pageInfoText != null)
            pageInfoText.gameObject.SetActive(show);
    }
    
    /// <summary>
    /// Show current page
    /// </summary>
    private void ShowCurrentPage()
    {
        if (currentStory == null) return;
        
        var page = currentStory.GetPage(currentPageIndex + 1);
        if (page == null) return;
        
        // Update page text
        if (currentPageText != null)
            currentPageText.text = page.text;
        
        // Update page image
        if (currentPageImage != null)
        {
            if (page.illustration != null)
            {
                var sprite = Sprite.Create(page.illustration, 
                    new Rect(0, 0, page.illustration.width, page.illustration.height), 
                    new Vector2(0.5f, 0.5f));
                currentPageImage.sprite = sprite;
                currentPageImage.SetNativeSize();
            }
            else
            {
                currentPageImage.sprite = null;
            }
        }
        
        // Update pagination button state
        UpdatePaginationButtons();
        
        // Update page information display
        UpdateCurrentPageInfo();
    }
    
    /// <summary>
    /// Update pagination button state
    /// </summary>
    private void UpdatePaginationButtons()
    {
        if (prevPageButton != null)
            prevPageButton.interactable = currentPageIndex > 0;
        
        if (nextPageButton != null)
            nextPageButton.interactable = currentPageIndex < currentStory.totalPages - 1;
    }
    
    /// <summary>
    /// Update current page information display
    /// </summary>
    private void UpdateCurrentPageInfo()
    {
        if (pageInfoText != null && currentStory != null)
        {
            pageInfoText.text = $"Page {currentPageIndex + 1} / {currentStory.totalPages}";
        }
    }
    
    /// <summary>
    /// Previous page button click event
    /// </summary>
    private void OnPrevPageClicked()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// Next page button click event
    /// </summary>
    private void OnNextPageClicked()
    {
        if (currentPageIndex < currentStory.totalPages - 1)
        {
            currentPageIndex++;
            ShowCurrentPage();
        }
    }
    
    /// <summary>
    /// Jump to specified page
    /// </summary>
    public void GoToPage(int pageNumber)
    {
        if (currentStory == null || pageNumber < 1 || pageNumber > currentStory.totalPages)
            return;
        
        currentPageIndex = pageNumber - 1;
        ShowCurrentPage();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ClearCurrentPageDisplay();
    }
}

