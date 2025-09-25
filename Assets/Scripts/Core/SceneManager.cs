using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private AIServiceConfig aiServiceConfig;
    [SerializeField] private Transform target;
    [SerializeField] private Transform fullScreen;
 
    private AIServiceManager aiServiceManager;
    public AIServiceManager AIServiceManager => aiServiceManager;
    
    // Backward compatibility
    public IAIClient GeminiClient => aiServiceManager?.CurrentClient;
    
    public static SceneManager Instance { get; set; }
    
    void Awake()
    {
        // Initialize AI Service Manager
        var aiServiceManagerGO = new GameObject("AIServiceManager");
        aiServiceManager = aiServiceManagerGO.AddComponent<AIServiceManager>();
        
        // Set the configuration
        if (aiServiceConfig != null)
        {
            aiServiceManager.ServiceConfig = aiServiceConfig;
            Debug.Log("AI Service Manager initialized with configuration");
        }
        else
        {
            Debug.LogWarning("AI Service Config is not assigned in SceneManager");
        }
        
        Application.targetFrameRate = 60;
        UILayerLoader.SetHanger(target, fullScreen);
        BasicPhase();
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ProcessesRunner.Main.TrySwitchToStep(MainSceneStep.Title).Forget();
    }

    // void Update()
    // {
    //     ProcessesRunner.Main.ProcessUpdate();
    // }
    
    void BasicPhase()
    {
        var titleProcess = new TitleProcess();
        var chapterProcess = new ChapterProcess();
        var picGeneratorProcessProcess = new PicGeneratorProcessProcess();
        var askAndAnswerProcess = new AskAndAnswerProcess();
        var storyDisplayProcess = new StoryDisplayProcess();
        
        ProcessesRunner.Main.Clear();
        ProcessesRunner.Main.Add(MainSceneStep.Title, titleProcess);
        ProcessesRunner.Main.Add(MainSceneStep.Chapter, chapterProcess);
        ProcessesRunner.Main.Add(MainSceneStep.AskAndAnswer, askAndAnswerProcess);
        ProcessesRunner.Main.Add(MainSceneStep.PicGenerator, picGeneratorProcessProcess);
        ProcessesRunner.Main.Add(MainSceneStep.Story, storyDisplayProcess);
    }
}
