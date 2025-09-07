# API 重构说明

## 概述

本次重构将 `GeminiClient` 和 `Imagen4` 从 MonoBehaviour 重构为普通类，使用 ScriptableObject 来管理配置。

## 主要变化

### 1. GeminiConfig ScriptableObject
- 位置：`Assets/Scripts/API/Config/GeminiConfig.cs`
- 功能：集中管理所有API相关配置
- 包含：API Key、模型名称、超时设置、图片生成参数等

### 2. GeminiClient 重构
- 不再继承 MonoBehaviour
- 通过构造函数接收 GeminiConfig
- 使用 MonoBehaviourRunner 执行协程

### 3. Imagen4Service 重构
- 替代原来的 Imagen4 MonoBehaviour
- 同样通过构造函数接收 GeminiConfig
- 提供图片生成功能

### 4. MonoBehaviourRunner
- 单例模式，用于在非MonoBehaviour类中执行协程
- 自动创建和管理生命周期

## 使用方法

### 1. 创建配置
1. 在 Project 窗口右键 → Create → StoryBook → API → Gemini Config
2. 设置 API Key 和其他参数
3. 将配置资源分配给需要使用的组件

### 2. 在代码中使用
```csharp
// 在 MonoBehaviour 中
[SerializeField] private GeminiConfig config;
private GeminiClient client;

private void Awake()
{
    if (config != null)
    {
        client = new GeminiClient(config);
    }
}

// 使用
var result = await client.AskAsync("Hello");
var images = await client.GeneratePic("A cat", 2, "16:9");
```

### 3. 更新现有组件
- `GeminiDemoUI`：现在需要分配 GeminiConfig 而不是 GeminiClient
- `StoryUILayer`：直接使用 SceneManager.Instance.GeminiClient

## 优势

1. **解耦**：API客户端不再依赖Unity的MonoBehaviour生命周期
2. **配置集中**：所有API设置在一个ScriptableObject中管理
3. **可重用**：同一个配置可以被多个组件使用
4. **测试友好**：更容易进行单元测试
5. **性能**：减少了不必要的GameObject和组件

## 迁移指南

1. 将现有的 GeminiClient 和 Imagen4 引用替换为 GeminiConfig
2. 在 Awake() 或 Start() 中初始化客户端
3. 更新预制体中的引用
4. 测试所有功能是否正常工作
