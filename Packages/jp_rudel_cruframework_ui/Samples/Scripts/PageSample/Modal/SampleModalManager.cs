using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using UnityEngine;
using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;

namespace CruFramework.Sample
{
    
    public enum SampleModalType
    {
        Message
    }
    
    public class SampleModalManager : Engine.UI.ModalManager<SampleModalType>
    {
        private static SampleModalManager instance = null;
        /// <summary>インスタンス</summary>
        public static SampleModalManager Instance{get{return instance;}}

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        protected override UniTask<Engine.UI.ModalWindow> OnLoadModalResource(SampleModalType modal)
        {
            return new UniTask<Engine.UI.ModalWindow>( transform.Find(modal.ToString()).GetComponent<Engine.UI.ModalWindow>() );
        }
    }
}