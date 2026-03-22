# Compatibility Module Boundary

## 1. 目的

本文档用于落实 `M1.3` 兼容模块降级，明确：

- 哪些模块属于 OpenCiv3 主游戏
- 哪些模块属于 Civilization III 兼容链
- 兼容链在当前阶段允许承担什么职责
- 后续应如何避免兼容模块重新主导主流程

## 2. 当前结论

以下模块当前应被视为兼容/导入工具链，而不是主游戏基础依赖：

- `QueryCiv3`
- `ConvertCiv3Media`
- `Blast`

它们仍然重要，但职责已经收缩为：

- 读取 legacy `.sav/.biq`
- 读取 legacy 场景资源
- 支持兼容研究、导入和调试

它们不应继续定义：

- 新游戏默认数据源
- 新游戏默认启动条件
- 普通 OpenCiv3 存档读写
- 主菜单是否可进入游戏

## 3. 主流程与兼容链的职责切分

主流程：

- 启动进入 OpenCiv3 主菜单
- `New Game`
- `Quick Start`
- OpenCiv3 存档读写
- standalone ruleset / OpenCiv3 assets

兼容链：

- 导入 Civilization III 图形
- 导入 Civilization III 场景
- 加载 legacy `.sav/.biq`
- 为兼容导入解析 `PediaIcons.txt`
- 解码 legacy 媒体与压缩格式

## 4. 当前代码边界

当前已经落地的边界包括：

- 新游戏默认使用 OpenCiv3 ruleset
- 普通 OpenCiv3 存档不再附带 `DefaultBicPath`
- `DefaultBicPath` 仅用于 legacy `.sav/.biq`
- 主菜单将 legacy 场景入口表述为导入能力，而非普通新局入口

## 5. 模块定位

### QueryCiv3

职责：

- 读取 Civilization III 的 `BIQ` / `SAV` 二进制数据
- 为 `ImportCiv3` 提供结构化原始输入

不再承担：

- OpenCiv3 主 ruleset 的数据来源

### ConvertCiv3Media

职责：

- 解析 Civilization III 旧媒体格式
- 服务于 legacy 图形和动画兼容路径

不再承担：

- OpenCiv3 自带资源的主入口定义

### Blast

职责：

- 支持 legacy 压缩格式解码

不再承担：

- 主流程任何独占依赖

## 6. 后续建议

后续 `M1.3` 可以继续推进：

- 把更多 `Civ3*` 命名的主流程工具类改成更中性的项目命名
- 继续把主菜单、通用 UI、基础音频迁到更明确的 OpenCiv3 资源入口
- 让兼容导入入口在 UI 上持续保持“辅助工具”语义
