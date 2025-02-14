using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CruFramework.Engine.UI
{
    [DefaultExecutionOrder(-1)]
    public class ScrollLinker : MonoBehaviour
    {
        [SerializeField]
        private ScrollLinkTarget[] linkTargets = null;
        
        // 現在操作中のスクロール
        private ScrollLinkTarget currentDragTarget = null;
        
        private void Awake()
        {
            foreach(ScrollLinkTarget target in linkTargets)
            {
                target.Initialize(this);
            }
        }
        
        internal void OnBeginDrag(ScrollLinkTarget target)
        {
            currentDragTarget = target;
        }
        
        internal void OnEndDrag(ScrollLinkTarget target)
        {
        }

        private void LateUpdate()
        {
            if(currentDragTarget != null)
            {
                foreach(ScrollLinkTarget target in linkTargets)
                {
                    target.Scroll.normalizedPosition = currentDragTarget.Scroll.normalizedPosition;
                }
            }
        }
    }
}