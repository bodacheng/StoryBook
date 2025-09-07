# StoryDisplayLayer 预制体设置指南

## 概述
StoryDisplayLayer预制体用于显示完整的故事内容，包括故事信息、页面列表和操作按钮。

## 预制体结构

### 主要组件
1. **StoryDisplayLayer** - 主脚本组件
2. **HeaderPanel** - 头部信息面板
3. **ScrollView** - 滚动视图容器
4. **ButtonPanel** - 底部按钮面板

### UI组件引用
需要在Inspector中设置以下组件引用：

#### 文本组件
- `titleText` - 显示故事标题
- `themeText` - 显示故事主题
- `pageCountText` - 显示页数信息

#### 滚动视图
- `storyScrollRect` - 滚动视图组件
- `storyContainer` - 内容容器（ScrollView/Viewport/Content）

#### 按钮
- `backButton` - 返回按钮
- `saveButton` - 保存按钮
- `shareButton` - 分享按钮

#### 预制体引用
- `pagePrefab` - 单个页面预制体（StoryPage.prefab）

## 创建步骤

### 方法1：使用编辑器工具
1. 在Unity菜单栏选择 `Tools > StoryBook > Create StoryDisplayLayer Prefab`
2. 工具会自动创建完整的预制体结构
3. 在Inspector中设置组件引用

### 方法2：手动创建
1. 创建Canvas作为根对象
2. 添加StoryDisplayLayer脚本
3. 创建HeaderPanel、ScrollView、ButtonPanel
4. 设置所有UI组件的引用

## 详细设置

### HeaderPanel设置
```
位置: 顶部 (Anchor: 0,1 to 1,1, Offset: 0,-100)
包含:
- TitleText (故事标题)
- ThemeText (主题信息)
- PageCountText (页数信息)
```

### ScrollView设置
```
位置: 中间 (Anchor: 0,0 to 1,1, Offset: 0,60 to 0,-60)
包含:
- Viewport (遮罩区域)
- Content (内容容器，使用VerticalLayoutGroup)
```

### ButtonPanel设置
```
位置: 底部 (Anchor: 0,0 to 1,0, Offset: 0,0 to 0,60)
包含:
- BackButton (返回)
- SaveButton (保存)
- ShareButton (分享)
```

## 使用示例

```csharp
// 获取StoryDisplayLayer实例
var storyDisplay = UILayerLoader.LoadAsync<StoryDisplayLayer>();

// 显示故事
storyDisplay.DisplayStory(storyData);

// 更新特定页面
storyDisplay.UpdatePageDisplay(pageNumber);

// 滚动到指定页面
storyDisplay.ScrollToPage(pageNumber);
```

## 注意事项

1. 确保StoryPage预制体已创建并正确引用
2. 所有UI组件引用必须在Inspector中正确设置
3. 滚动视图的Content需要设置VerticalLayoutGroup
4. 按钮事件会在StoryDisplayLayer脚本中自动绑定

## 样式设置

### 颜色方案
- 背景色: #F2F2F2 (浅灰)
- 头部背景: #E6E6E6 (中灰)
- 按钮背景: #3399FF (蓝色)
- 文本颜色: #000000 (黑色)

### 字体设置
- 标题: 24px, Bold
- 副标题: 18px, Normal
- 正文: 16px, Normal
- 按钮: 16px, Normal

## 故障排除

### 常见问题
1. **页面不显示**: 检查pagePrefab引用和storyContainer设置
2. **滚动不工作**: 确认ScrollRect和Content设置正确
3. **按钮无响应**: 检查按钮事件绑定
4. **布局错乱**: 确认LayoutGroup设置正确

### 调试建议
1. 使用Unity的RectTransform工具检查布局
2. 查看Console中的错误信息
3. 检查组件引用是否为空
4. 验证预制体结构完整性
