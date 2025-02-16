using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterItem : ScrollGridItem
{
    public class Data : ScrollData
    {
        
    }
    
    protected override void OnSetView(object value)
    {
        gameObject.SetActive(true);
    }
}
