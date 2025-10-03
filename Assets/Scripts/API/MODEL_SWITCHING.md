# AI Model Switching System

This document describes the AI model switching system that allows the StoryBook project to use either OpenAI or Gemini models for story and image generation.

## Overview

The model switching system provides a unified interface for different AI providers, allowing seamless switching between OpenAI and Gemini models without changing the core application logic.

## Architecture

### Core Components

1. **IAIClient Interface** - Abstract interface for all AI clients
2. **AIServiceManager** - Singleton manager for handling model switching
3. **AIServiceConfig** - Configuration asset for model settings
4. **Model-specific Clients** - GeminiClient and OpenAIClient implementations

### File Structure

```
Assets/Scripts/API/
├── Interfaces/
│   └── IAIClient.cs                 # Abstract AI client interface
├── Config/
│   ├── AIModelType.cs              # Enum for supported models
│   ├── AIServiceConfig.cs          # Main configuration asset
│   ├── GeminiConfig.cs             # Gemini-specific config
│   └── OpenAIConfig.cs             # OpenAI-specific config
├── Clients/
│   ├── GeminiClient.cs             # Gemini implementation
│   ├── OpenAIClient.cs             # OpenAI implementation
│   ├── Imagen4Service.cs           # Gemini image generation
│   └── DALLEService.cs             # OpenAI image generation
└── Services/
    └── AIServiceManager.cs         # Model switching manager
```

## Configuration

### 1. Create AI Service Configuration

1. Right-click in Project window
2. Create → StoryBook → API → AI Service Config
3. Assign Gemini and OpenAI configurations to the respective fields

### 2. Configure Gemini

1. Create → StoryBook → API → Gemini Config
2. Set your Gemini API key
3. Configure model and timeout settings

### 3. Configure OpenAI

1. Create → StoryBook → API → OpenAI Config
2. Set your OpenAI API key
3. Configure text and image models

### 4. Update SceneManager

1. Open the SceneManager in the scene
2. Assign the AI Service Config to the `aiServiceConfig` field
3. Remove the old `geminiConfig` field

## Usage

### Basic Usage

```csharp
// Get the AI service manager
var aiManager = AIServiceManager.Instance;

// Switch to OpenAI
aiManager.SwitchToModel(AIModelType.OpenAI);

// Switch to Gemini
aiManager.SwitchToModel(AIModelType.Gemini);

// Use the current model
var response = await aiManager.AskAsync("Tell me a story");
var images = await aiManager.GeneratePic("A beautiful landscape");
```

### Direct Client Access

```csharp
// Get specific client
var geminiClient = aiManager.GetGeminiClient();
var openAIClient = aiManager.GetOpenAIClient();

// Use directly
var response = await geminiClient.AskAsync("Hello");
```

### Events

```csharp
// Subscribe to model changes
aiManager.OnModelChanged += (newModel) => {
    Debug.Log($"Switched to {newModel}");
};

// Subscribe to errors
aiManager.OnError += (error) => {
    Debug.LogError($"AI Error: {error}");
};
```

## UI Components

### AIModelSelector

A UI component for selecting AI models with the following features:

- Dropdown for model selection
- Status display
- Auto-apply or manual apply modes
- Refresh button
- Error handling

### SettingsLayer

A complete settings UI that includes:

- Model selector
- Auto-apply toggle
- Status display toggle
- Save/Reset buttons
- Information panel

## Model-Specific Features

### Gemini

- **Text Model**: gemini-2.5-flash (configurable)
- **Image Model**: Imagen 4.0
- **Features**: Fast text generation, high-quality images
- **API**: Google Generative AI

### OpenAI

- **Text Model**: gpt-4o (configurable)
- **Image Model**: DALL-E 3
- **Features**: Advanced reasoning, creative images
- **API**: OpenAI API

## Migration Guide

### From Old System

1. **Update SceneManager**: Replace `GeminiConfig` with `AIServiceConfig`
2. **Update Services**: Use `AIServiceManager` instead of direct `GeminiClient`
3. **Update UI**: Add model selector components where needed

### Example Migration

**Before:**
```csharp
public class MyService
{
    private GeminiClient geminiClient;
    
    public MyService(GeminiClient client)
    {
        geminiClient = client;
    }
    
    public async Task<string> GenerateText(string prompt)
    {
        return await geminiClient.AskAsync(prompt);
    }
}
```

**After:**
```csharp
public class MyService
{
    private AIServiceManager aiManager;
    
    public MyService(AIServiceManager aiManager)
    {
        this.aiManager = aiManager;
    }
    
    public async Task<string> GenerateText(string prompt)
    {
        return await aiManager.AskAsync(prompt);
    }
}
```

## Error Handling

The system provides comprehensive error handling:

- **Configuration Errors**: Missing API keys, invalid models
- **Network Errors**: Connection timeouts, API failures
- **Model Errors**: Unavailable models, rate limits
- **UI Errors**: Invalid selections, missing components

## Best Practices

1. **Always check if the service is configured** before making requests
2. **Handle errors gracefully** with user-friendly messages
3. **Use events** for UI updates when models change
4. **Test both models** to ensure compatibility
5. **Monitor API usage** to avoid rate limits

## Troubleshooting

### Common Issues

1. **"AI Service Manager not available"**
   - Ensure AIServiceManager is properly initialized
   - Check that the configuration is assigned

2. **"Model not configured"**
   - Verify API keys are set correctly
   - Check model names are valid

3. **"Failed to switch model"**
   - Ensure the target model is available
   - Check configuration validity

4. **UI not updating**
   - Verify event subscriptions
   - Check UI component references

### Debug Tips

- Enable debug logging in AIServiceManager
- Check Unity Console for error messages
- Verify API keys are valid
- Test with simple prompts first

## Future Extensions

The system is designed to be easily extensible:

1. **Add New Models**: Implement `IAIClient` interface
2. **Add New Features**: Extend `AIServiceManager`
3. **Add New UI**: Create components that use `AIServiceManager`
4. **Add Configuration**: Extend `AIServiceConfig`

## API Reference

### AIServiceManager

- `SwitchToModel(AIModelType)` - Switch to specified model
- `AskAsync(string, int?)` - Send text prompt
- `GeneratePic(string, int?, string)` - Generate images
- `GetAvailableModels()` - Get list of available models
- `IsModelAvailable(AIModelType)` - Check if model is available

### IAIClient

- `AskAsync(string, int?)` - Send text prompt
- `GeneratePic(string, int?, string)` - Generate images
- `ProviderName` - Get provider name
- `IsConfigured` - Check if properly configured

### Events

- `OnModelChanged` - Fired when model changes
- `OnError` - Fired when errors occur

