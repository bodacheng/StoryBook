using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform fullScreen;
    
    void Awake()
    {
        Application.targetFrameRate = 60;
        UILayerLoader.SetHanger(target, fullScreen);
        BasicPhase();
    }

    // Start is called before the first frame update
    void Start()
    {
        ProcessesRunner.Main.TrySwitchToStep(MainSceneStep.Title).Forget();
    }

    void Update()
    {
        ProcessesRunner.Main.ProcessUpdate();
    }
    
    void BasicPhase()
    {
        var titleProcess = new TitleProcess();
        var chapterProcess = new ChapterProcess();
        
        ProcessesRunner.Main.Clear();
        ProcessesRunner.Main.Add(MainSceneStep.Title, titleProcess);
        ProcessesRunner.Main.Add(MainSceneStep.Chapter, chapterProcess);
    }
}
