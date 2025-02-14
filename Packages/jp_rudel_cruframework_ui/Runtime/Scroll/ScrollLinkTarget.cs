using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CruFramework.Engine.UI
{
    public class ScrollLinkTarget : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private ScrollRect scrollRect = null;
        /// <summary>Scroll</summary>
        public ScrollRect Scroll{get{return scrollRect;}}
        
        private ScrollLinker linker = null;

        private void Awake()
        {
            scrollRect = gameObject.GetComponentInParent<ScrollRect>();
        }
        
        internal void Initialize(ScrollLinker linker)
        {
            this.linker = linker;
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            linker.OnBeginDrag(this);
        }
        
        void IBeginDragHandler.OnBeginDrag(PointerEventData e)
        {
            linker.OnBeginDrag(this);
        }
        
        void IEndDragHandler.OnEndDrag(PointerEventData e)
        {
            linker.OnEndDrag(this);
        }
    }
}