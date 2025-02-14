
///////////////////////////////
// 表示するUIのサイズが動的に変わったりする場合に使用するスクロール
// 縦or横 １列のみ対応
//////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;

namespace CruFramework.Sample
{
    public class SampleScrollDynamic : MonoBehaviour
    {
        [SerializeField]
        private ScrollDynamic scrollDynamic = null;


        private void Awake()
        {
            int[] list = new int[100];
            for(int i=0;i<list.Length;i++)
            {
                list[i] = i;
            }

            // スクロールにアイテムを渡す
            scrollDynamic.SetItems(list);
        }
    }
}