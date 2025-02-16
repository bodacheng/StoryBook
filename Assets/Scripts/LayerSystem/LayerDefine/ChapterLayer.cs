using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChapterLayer : UILayer
{
    [SerializeField] private ScrollGrid scrollGrid;
    
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();

        List<ChapterItem.Data> items = new List<ChapterItem.Data>();

        for (int i = 0; i < 5; i++)
        {
            items.Add(new ChapterItem.Data());
        }
        
        scrollGrid.SetItems(items);
    }
}
