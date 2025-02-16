using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TitleLayer : UILayer
{
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();
    }

    public void ToChapter()
    {
        ProcessesRunner.Main.TrySwitchToStep(MainSceneStep.Chapter).Forget();
    }
}
