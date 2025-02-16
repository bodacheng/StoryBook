using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterItem : ScrollGridItem
{
    
    
    protected override void OnSetView(object value)
    {
        gameObject.SetActive(true);
    }
}
