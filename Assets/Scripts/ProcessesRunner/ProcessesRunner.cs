using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ProcessesRunner
{
    static ProcessesRunner _instanceMain;
    static readonly List<ReturnAction> ReturnMissionList = new List<ReturnAction>();
    public enum ProcessResult
    {
        Success, Failed, Cancel
    }

    private class ReturnAction
    {
        public MainSceneStep ReturnToStep;
        public Func<UniTask<ProcessResult>> ReturnProcess;
    }
    
    public static ProcessesRunner Main
    {
        get
        {
            if (_instanceMain == null)
            {
                _instanceMain = new ProcessesRunner();
            }
            return _instanceMain;
        }
    }
    
    public MainSceneProcess LastProcess;
    public MainSceneProcess CurrentProcess;

    readonly ReactiveProperty<MainSceneStep> currentStep = new ReactiveProperty<MainSceneStep>();
    public ReactiveProperty<MainSceneStep> CurrentStep => currentStep;
    readonly IDictionary<MainSceneStep, MainSceneProcess> dic = new Dictionary<MainSceneStep, MainSceneProcess>();
    
    public MainSceneProcess GetProcess(MainSceneStep step)
    {
        return dic[step];
    }

    public void Clear()
    {
        LastProcess = null;
        CurrentProcess = null;
        dic.Clear();
    }

    public void Add(MainSceneStep step, MainSceneProcess process)
    {
        dic.TryAdd(step, process);
    }

    public void ProcessUpdate()
    {
        CurrentProcess?.LocalUpdate();
    }

    async UniTask<ProcessResult> ChangeProcess(MainSceneStep sceneStep)
    {
        var result = await ChangeProcess<Any>(sceneStep, null);
        return result;
    }
    
    async UniTask<ProcessResult> ChangeProcess<T>(MainSceneStep sceneStep, T t)
    {
        if (CurrentProcess != null)
        {
            await CurrentProcess.ProcessEnd();
        }
        
        LastProcess = CurrentProcess;
        dic.TryGetValue(sceneStep, out CurrentProcess);
        if (CurrentProcess != null)
        {
            currentStep.Value = sceneStep;
            
            if (t != null)
                await CurrentProcess.ProcessEnter(t);
            else
                await CurrentProcess.ProcessEnter();
        }
        else
        {
            Debug.Log("empty state key:" + sceneStep);
        }
        return ProcessResult.Success;
    }
    
    public async UniTask<ProcessResult> TrySwitchToStep(MainSceneStep nextStep, bool forward = true)
    {
        ProcessResult result;
        if (forward && CurrentProcess != null)
        {
            var returnToStep = CurrentProcess.Step;
            async UniTask<ProcessResult> ReturnToCurrent()
            {
                result = await TrySwitchToStep(returnToStep, false);
                return result;
            }
                
            ReturnAction returnAction = new ReturnAction
            {
                ReturnToStep = returnToStep,
                ReturnProcess = ReturnToCurrent
            };
            
            result =  await ChangeProcess(nextStep);
            if (result == ProcessResult.Success)
                Push(returnAction);
            return result;
        }
        
        result = await ChangeProcess(nextStep);
        return result;
    }
    
    static void Push(ReturnAction returnAction)
    {
        void RegisterReturn()
        {
            var exist = ReturnMissionList.Find(x => x.ReturnToStep == returnAction.ReturnToStep);
            if (exist != null)
                ReturnMissionList.Remove(exist);
            ReturnMissionList.Add(returnAction);
        }
        if (ReturnMissionList.Count > 0)
        {
            var last = ReturnMissionList[^1];
            if (last.ReturnToStep != returnAction.ReturnToStep)
            {
                RegisterReturn();
            }
        }
        else
        {
            RegisterReturn();
        }
    }
    
    public async UniTask<ProcessResult> Pop()
    {
        if (ReturnMissionList.Count == 0)
            return ProcessResult.Cancel;
        
        var targetMission = ReturnMissionList[^1];
        ReturnMissionList.RemoveAt(ReturnMissionList.Count - 1);
        var result = await targetMission.ReturnProcess.Invoke();
        return result;
    }
    
    private class Any{}
}