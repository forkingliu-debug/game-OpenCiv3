# Project Identity

## 1. 当前身份

当前项目对外身份统一为：

- 名称：`OpenCiv3`
- 定位：`独立演进的新文明类单机回合制 4X 策略项目`
- 关系：`受 Civilization III 启发，并保留兼容导入能力，但主流程不再由原版依赖定义`

## 2. 身份表达原则

在 UI、文档、配置中应保持以下表达一致：

- OpenCiv3 是主产品名
- standalone ruleset / built-in assets 是默认主路径
- Civilization III 相关能力属于 import / compatibility / legacy 路径
- 内部遗留命名 `C7` 允许暂时保留在代码结构中，但不应再作为主要对外身份

## 3. 当前已统一的部分

- `project.godot` 显示名为 `OpenCiv3`
- 主菜单提示文案已把 OpenCiv3 作为默认玩法
- README 已将项目描述调整为 standalone-first 新文明类项目

## 4. 当前仍存在的遗留项

仍可见的历史命名包括：

- 目录名 `C7`
- 部分类名和脚本名中的 `Civ3`
- 若干技术文档中对上游 OpenCiv3/C7 历史的说明

这些遗留命名当前可接受，但应视为内部演化痕迹，而不是产品身份本身。

## 5. 后续建议

- 新增对外文案优先使用 `OpenCiv3`
- 新增主流程功能命名避免继续扩大 `Civ3*` 前缀
- 仅在兼容导入、格式解析、原版资源桥接场景下使用 Civilization III / Civ3 命名
