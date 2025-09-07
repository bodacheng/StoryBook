using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GeminiConfig geminiConfig;
    [SerializeField] private Transform target;
    [SerializeField] private Transform fullScreen;
 
    private GeminiClient geminiClient;
    public GeminiClient GeminiClient => geminiClient;
    
    public static SceneManager Instance { get; set; }
    
    void Awake()
    {
        geminiClient = new GeminiClient(geminiConfig);
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
        
        ProcessesRunner.Main.Clear();
        ProcessesRunner.Main.Add(MainSceneStep.Title, titleProcess);
        ProcessesRunner.Main.Add(MainSceneStep.Chapter, chapterProcess);
        ProcessesRunner.Main.Add(MainSceneStep.AskAndAnswer, askAndAnswerProcess);
        ProcessesRunner.Main.Add(MainSceneStep.PicGenerator, picGeneratorProcessProcess);
    }
}
