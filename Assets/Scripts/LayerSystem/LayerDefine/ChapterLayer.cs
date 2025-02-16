using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChapterLayer : UILayer
{
    [SerializeField] private ScrollGrid scrollGrid;
    [SerializeField] private ChapterItem itemPrefab;
    
    public override async UniTask OnPreOpen()
    {
        await base.OnPreOpen();

        List<ChapterItem> items = new List<ChapterItem>();

        for (int i = 0; i < 5; i++)
        {
            var item = GameObject.Instantiate(itemPrefab);
            items.Add(item);
        }
        
        scrollGrid.SetItems(items);
    }
}
