﻿using UnityEngine;
using UnityEngine.UI;

public static class PosCal
{
    public static Canvas Canvas;
    public static RectTransform SafeAreaRect;
    static CanvasScaler CanvasScaler => Canvas.GetComponent<CanvasScaler>();
    public static float CanvasWidth => Canvas.GetComponent<RectTransform>().rect.width;
    public static float CanvasHeight => Canvas.GetComponent<RectTransform>().rect.height;
    
    public static void TestIni()
    {
        // 画面サイズに合わせてUIも拡大する
        CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // キャンバスのサイズが基準の解像度よりも大きくなるようにする
        CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        // 基準の解像度がセーフエリア内に収まるようにCanvasScalerの解像度を調整
        Vector2 resolution = CanvasScaler.referenceResolution;
        resolution.x = (int)(resolution.x * (Screen.width / (float)Screen.safeArea.width));
        resolution.y = (int)(resolution.y * (Screen.height / (float)Screen.safeArea.height));
        CanvasScaler.referenceResolution = resolution;
        
        // 以下chatgpt
        
        // 获取当前设备的安全区域（单位：屏幕像素）
        Rect safeArea = Screen.safeArea;

        // 计算锚点（归一化到 0~1 范围）
        Vector2 anchorMin = new Vector2(
            safeArea.xMin / Screen.width,
            safeArea.yMin / Screen.height
        );
        Vector2 anchorMax = new Vector2(
            (safeArea.xMin + safeArea.width) / Screen.width,
            (safeArea.yMin + safeArea.height) / Screen.height
        );

        // 假设 safeAreaRect 是我们刚刚创建的子物体的 RectTransform
        SafeAreaRect.anchorMin = anchorMin;
        SafeAreaRect.anchorMax = anchorMax;
        // 保持 offset 为 0，让它刚好铺满 safeArea 对应的区域
        SafeAreaRect.offsetMin = Vector2.zero;
        SafeAreaRect.offsetMax = Vector2.zero;
    }

    public static (float, float) GetSafeAreaWidthAndHeightInCanvas()
    {
        Rect safeArea = Screen.safeArea;
        
        // 获取 Canvas 缩放因子
        float scaleFactor = Canvas.scaleFactor;

        // 将 safeArea 的宽高转换为 Canvas 的坐标
        float safeAreaWidthInCanvas = safeArea.width / scaleFactor;
        float safeAreaHeightInCanvas = safeArea.height / scaleFactor;
        return (safeAreaWidthInCanvas, safeAreaHeightInCanvas);
    }
    
    /// <summary>
    /// 九宫格的slot特效在比1920x1080更长的设备上并不会出现尺寸变不匹配问题
    /// 但是如果在长宽比例更低的设备上，比如ipad，就是出现尺寸错误
    /// 我们没有理清内部的具体逻辑，但是我们认为更低的长宽比设备上，这个设备长宽比/参考长宽比的数字就是slot特效的scale应该乘以的数字
    /// 而事实证明似乎没错
    ///
    /// 目前都不是理论完美解决方案。这个特效逻辑本身太复杂
    /// </summary>
    /// <returns></returns>
    public static float TempRate()
    {
        //方案1
        float screenAspect = (float)Screen.width / Screen.height;
        float refAspect = CanvasScaler.referenceResolution.x / CanvasScaler.referenceResolution.y;
        // if (screenAspect >= refAspect) // 这个处理（分歧，这种情况返回1）在横版项目是需要的，纵版却不需要，原因还没理解
        // {
        //     return 1;
        // }
        // return screenAspect / refAspect;
        
        //方案2
        return (screenAspect / refAspect) > 1 ?
            Mathf.Max((CanvasWidth / CanvasScaler.referenceResolution.x), (CanvasHeight/CanvasScaler.referenceResolution.y)):
            Mathf.Min((CanvasWidth / CanvasScaler.referenceResolution.x), (CanvasHeight/CanvasScaler.referenceResolution.y));
        
        //方案3
        // float screenAspect = CanvasWidth / CanvasHeight;
        // float refAspect = CanvasScaler.referenceResolution.x / CanvasScaler.referenceResolution.y;
        // return screenAspect / refAspect;
    }
    
    public static float TempToko() // 这完全是个主观数值，目的是让手机比较长的时候立绘更靠中间一点。没有太多道理
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float refAspect = CanvasScaler.referenceResolution.x / CanvasScaler.referenceResolution.y;
        if (screenAspect <= refAspect)
        {
            return 0;
        }
        return (((screenAspect / refAspect) -1)/2) * CanvasScaler.referenceResolution.x;
    }
    
    /// <summary>
    /// 这个函数在目前所用的地方为什么能得到正确的值我们压根不理解。主要不理解rect.transform.position到底是什么
    /// </summary>
    /// <param name="refC"></param>
    /// <param name="rect"></param>
    /// <param name="zOffset"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPos(Camera refC, RectTransform rect, float zOffset)
    {
        var rectPos = rect.transform.position;
        var trueAnchorPos = new Vector2(rectPos.x, rectPos.y);
        var worldPos = refC.ScreenToWorldPoint(trueAnchorPos);
        worldPos = new Vector3(worldPos.x, worldPos.y, refC.transform.position.z + zOffset);
        return worldPos;
    }
    
    /// 这个是一律和ConvertAnchorPos配合使用，rectPos指的是UI元素在Canvas内的anchoredPosition，它并不等同于Screen Position。
    /// 前者的最大值是Canvas的长和宽，后者的最大值是设备分辨率
    /// </param>
    /// <param name="rectPos"> 这个值是UI元素的坐标，指的应该是我们希望把某个特效给定到的位置 </param>
    /// <param name="zOffset"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPos(Camera refC, Vector3 rectPos, float zOffset)
    {
        var screenPos = new Vector2(Screen.width * rectPos.x/ CanvasWidth, Screen.height * rectPos.y/ CanvasHeight);
        var worldPos = refC.ScreenToWorldPoint(screenPos);
        worldPos = new Vector3(worldPos.x, worldPos.y, refC.transform.position.z + zOffset);
        return worldPos;
    }
    
    /// <summary>
    /// 获取某ui元素在某个不同的anchor下，所在位置的anchorPosition值
    /// </summary>
    /// <param name="anchoredPosition">
    ///  formerAnchor下的原RectTransform.anchorPosition
    /// </param>
    /// <param name="formerAnchor">
    /// 之前的Anchor，比如说1，1代表 anchor在右上角
    /// </param>
    /// <param name="targetAnchor">
    /// 目标Anchor，比如说0，0代表 anchor在左下角
    /// </param>
    /// <returns></returns>
    // public static Vector3 ConvertAnchorPos(Vector3 anchoredPosition, Vector2 formerAnchor, Vector2 targetAnchor)
    // {
    //     float newX = canvasWidth - anchoredPosition.x * (targetAnchor.x - formerAnchor.x) / 1;
    //     float newY = canvasHeight - anchoredPosition.y * (targetAnchor.y - formerAnchor.y) / 1;
    //     
    //     Debug.Log("目标屏幕位置："+ new Vector3(newX, newY, 0));
    //     return new Vector3(newX, newY, 0);
    // }
    
    public static Vector2 CalculateAnchoredPositionInNewAnchor(RectTransform uiElement, Vector2 targetAnchor, RectTransform refParent)
    {
        Vector2 parentSize = refParent.rect.size;
        
        // Calculate the current position in the parent RectTransform space.
        Vector2 currentPositionInParentSpace = uiElement.anchorMin * parentSize + uiElement.anchoredPosition;

        // Calculate the new anchored position in the target anchor without modifying the RectTransform.
        Vector2 newPositionInTargetAnchor = currentPositionInParentSpace - targetAnchor * parentSize;
        
        return newPositionInTargetAnchor;
    }
    
    public static float AdjustedViewPortHeight(float originHeight, float itemHeight, float space)
    {
        var linesToShowInViewPort = 0;
        while (true)
        {
            if ((linesToShowInViewPort + 1) * (itemHeight + space) < originHeight)
            {
                linesToShowInViewPort++;
            }
            else
            {
                break;
            }
        }
        var viewPortHeight = linesToShowInViewPort * (itemHeight + space) - space;
        return viewPortHeight;
    }
}
