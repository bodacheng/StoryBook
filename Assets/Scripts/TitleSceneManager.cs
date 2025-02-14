using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform fullScreen;
    
    void Awake()
    {
        UILayerLoader.SetHanger(target, fullScreen);
    }

    // Start is called before the first frame update
    void Start()
    {
        UILayerLoader.LoadAsync<TitleLayer>().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
