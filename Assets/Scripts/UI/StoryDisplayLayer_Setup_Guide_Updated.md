# StoryDisplayLayer 设置指南 (更新版)

## 新增的UI组件

为了支持用户输入故事参数，需要在Unity编辑器中为 `StoryDisplayLayer` 添加以下新的UI组件：

### 1. 输入面板 (Input Panel)
- 创建一个空的GameObject作为输入面板
- 命名为 "InputPanel"
- 将其拖拽到 `StoryDisplayLayer` 的 `inputPanel` 字段

### 2. 标题输入字段 (Title Input Field)
- 在InputPanel下创建一个InputField
- 命名为 "TitleInputField"
- 设置Placeholder文本为 "请输入故事标题"
- 将其拖拽到 `StoryDisplayLayer` 的 `titleInputField` 字段

### 3. 主题输入字段 (Theme Input Field)
- 在InputPanel下创建一个InputField
- 命名为 "ThemeInputField"
- 设置Placeholder文本为 "请输入故事主题"
- 将其拖拽到 `StoryDisplayLayer` 的 `themeInputField` 字段

### 4. 页数输入字段 (Page Count Input Field)
- 在InputPanel下创建一个InputField
- 命名为 "PageCountInputField"
- 设置Placeholder文本为 "页数 (1-10)"
- 设置Content Type为 "Integer Number"
- 将其拖拽到 `StoryDisplayLayer` 的 `pageCountInputField` 字段

### 5. 进度文本 (Progress Text)
- 创建一个Text组件
- 命名为 "ProgressText"
- 初始时设置为非激活状态
- 将其拖拽到 `StoryDisplayLayer` 的 `progressText` 字段

## 功能说明

### 新的工作流程
1. 用户进入故事显示界面时，会看到输入面板
2. 用户可以输入自定义的故事标题、主题和页数
3. 点击"生成故事"按钮后，系统会使用用户输入的参数生成故事
4. 生成过程中会显示进度信息
5. 生成完成后，输入面板会隐藏，显示生成的故事内容

### 参数验证
- 标题和主题不能为空
- 页数会自动限制在1-10页之间
- 如果输入无效，会显示相应的提示信息

### 默认值
- 标题默认值：小兔子的冒险
- 主题默认值：友谊与勇气
- 页数默认值：3

## 代码变更总结

1. **StoryDisplayLayer.cs**
   - 添加了输入字段的UI组件引用
   - 修改了生成函数类型，现在接受三个参数：title, theme, pageCount
   - 添加了参数获取和验证逻辑
   - 更新了UI状态管理

2. **StoryDisplayProcess.cs**
   - 修改了GenerateStoryAsync方法，现在接受参数
   - 使用StoryGenerationService.GenerateStoryAsync而不是GenerateSampleStoryAsync

3. **StoryGenerationService.cs**
   - 无需修改，因为GenerateStoryAsync方法已经支持参数

这样的设计让用户可以完全自定义故事的内容，而不是使用写死的参数。
