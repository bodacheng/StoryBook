using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TitleLayer : UILayer
{
    public override async UniTask<LayerResult> OnPreOpen()
    {
        return await base.OnPreOpen();
    }
}
