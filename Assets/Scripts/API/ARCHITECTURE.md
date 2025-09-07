# API 架构图

## 重构前
```
MonoBehaviour Components
├── GeminiClient (MonoBehaviour)
│   ├── apiKey (SerializeField)
│   ├── model (SerializeField)
│   └── pic (Imagen4 reference)
└── Imagen4 (MonoBehaviour)
    └── API methods
```

## 重构后
```
Configuration Layer
└── GeminiConfig (ScriptableObject)
    ├── apiKey
    ├── model
    ├── timeout settings
    └── image generation settings

Service Layer
├── GeminiClient (Plain Class)
│   ├── Constructor(GeminiConfig)
│   ├── AskAsync()
│   └── GeneratePic()
└── Imagen4Service (Plain Class)
    ├── Constructor(GeminiConfig)
    └── GenerateImagesImagenAsync()

Infrastructure
└── MonoBehaviourRunner (Singleton)
    └── StartCoroutine() for non-MonoBehaviour classes

UI Layer
├── GeminiDemoUI (MonoBehaviour)
│   ├── GeminiConfig reference
│   └── GeminiClient instance
└── StoryUILayer (MonoBehaviour)
    └── 直接使用 SceneManager.Instance.GeminiClient
```

## 数据流
1. Unity Inspector 中分配 GeminiConfig
2. MonoBehaviour 在 Awake() 中创建 GeminiClient(config)
3. GeminiClient 使用配置进行API调用
4. 协程通过 MonoBehaviourRunner 执行
5. 结果返回给调用者

## 优势
- 配置与逻辑分离
- 更好的可测试性
- 减少GameObject依赖
- 配置可重用
- 更清晰的职责分离
