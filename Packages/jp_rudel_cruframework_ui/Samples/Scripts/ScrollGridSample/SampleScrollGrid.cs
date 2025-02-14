using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.Engine.UI;

namespace CruFramework.Sample
{
    public class SampleScrollGrid : MonoBehaviour
    {
        [SerializeField]
        private ScrollGrid scrollGrid = null;


        private void Start()
        {
            int[] itemDatas = new int[100];
            for(int i=0;i<itemDatas.Length;i++)
            {
                itemDatas[i] = i;
            }
            
            scrollGrid.SetItems(itemDatas);
        }
    }
}
