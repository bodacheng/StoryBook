using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Engine.UI
{
    public abstract class ScrollDynamicDefaultItemSelector : ScrollDynamicItemSelector
    {
        [SerializeField]
        private ScrollDynamicItem item = null;
        
        /// <summary>アイテムの取得</summary>
        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}
