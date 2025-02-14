using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CruFramework.Engine.UI {

    [RequireComponent(typeof(ScrollRect))]
    public class NestedScrollEventThrower : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {
        ScrollRect scrollRect = null;

        bool routeToParent = false;
        
        // Handlers
        List<IInitializePotentialDragHandler> initializePotentialDragHandlers = new List<IInitializePotentialDragHandler>();
        List<IDragHandler> dragHandlers = new List<IDragHandler>();
        List<IBeginDragHandler> beginDragHandler = new List<IBeginDragHandler>();
        List<IEndDragHandler> endDragHandler = new List<IEndDragHandler>();

        public void Start(){
            scrollRect = GetComponent<ScrollRect>();
            //最初にターゲットになる親のEventHandlerをキャッシュしておく
            FindAndSetEventThrowParents();
        }

        /// <summary>
        /// イベントを投げる親の検索と設定
        /// </summary>
        void FindAndSetEventThrowParents(){
            initializePotentialDragHandlers = FindParentEventSystemHandler<IInitializePotentialDragHandler>();
            dragHandlers = FindParentEventSystemHandler<IDragHandler>();
            beginDragHandler = FindParentEventSystemHandler<IBeginDragHandler>();
            endDragHandler = FindParentEventSystemHandler<IEndDragHandler>();
        }
	
        public void OnInitializePotentialDrag (PointerEventData eventData) {
            DoEventSystemHandlers<IInitializePotentialDragHandler>( initializePotentialDragHandlers, (parent) => { parent.OnInitializePotentialDrag(eventData); });
        }
        
        public void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(routeToParent) {
                DoEventSystemHandlers<IDragHandler>( dragHandlers, (parent) => { parent.OnDrag(eventData); });
            }
        }
        
        public void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if( !scrollRect.horizontal && Math.Abs (eventData.delta.x) > Math.Abs (eventData.delta.y) ) {
                routeToParent = true;
            } else if ( !scrollRect.vertical && Math.Abs (eventData.delta.x) < Math.Abs (eventData.delta.y) ){
                routeToParent = true;
            } else {
                routeToParent = false;
            }

            if(routeToParent) {
                DoEventSystemHandlers<IBeginDragHandler>( beginDragHandler, (parent) => { parent.OnBeginDrag(eventData); });
            } 
        }
        
        public void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(routeToParent){
                DoEventSystemHandlers<IEndDragHandler>( endDragHandler, (parent) => { parent.OnEndDrag(eventData); });
            }
            routeToParent = false;
        }

        /// <summary>
        /// 指定してEventHandlerを検索してリストで返す
        /// </summary>
        List<T> FindParentEventSystemHandler<T>() where T : class, IEventSystemHandler
        {
            var parent = transform.parent;
            var parentHandlers = new List<T>();
            while(parent != null) {
                foreach(var component in parent.GetComponents<Component>()) {
                    if(component is T) {
                        parentHandlers.Add( component as T );
                    }
                }
                parent = parent.parent;
            }
            return parentHandlers;
        }
        
        /// <summary>
        /// 渡されたEventHandlerListに対してActionを実行する
        /// </summary>
        void DoEventSystemHandlers<T>( List<T> handlers , Action<T> action) where T: class, IEventSystemHandler
        {
            for( int i=0; i<handlers.Count; ++i ) {
                var handler = handlers[i];
                if( handler == null ) {
                    continue;
                }
                action( handler as T );
            }
        }
    
    }
 
}