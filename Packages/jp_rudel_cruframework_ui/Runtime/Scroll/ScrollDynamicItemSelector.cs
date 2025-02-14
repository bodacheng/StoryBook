using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Engine.UI
{
    public abstract class ScrollDynamicItemSelector : MonoBehaviour
    {
        /// <summary>アイテムの取得</summary>
        public abstract ScrollDynamicItem GetItem(object item);
    }
}
