using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChapterLayer : UILayer
{
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();
    }
}
