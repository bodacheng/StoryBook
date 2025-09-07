using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChapterProcess : MainSceneProcess
{
    private ChapterLayer layer;
    
    public ChapterProcess()
    {
        Step = MainSceneStep.Chapter;
    }
    
    public override async UniTask ProcessEnter()
    {
        layer = await UILayerLoader.LoadAsync<ChapterLayer>();
    }
    
    public override async UniTask ProcessEnd()
    {
        await UILayerLoader.Remove<ChapterLayer>();
    }
}
