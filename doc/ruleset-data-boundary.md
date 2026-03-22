# Ruleset Data Boundary

## 1. 目的

本文档用于落实 `M1.2` 数据源梳理，明确：

- OpenCiv3 当前主数据源是什么
- 哪些内容已经由 standalone/ruleset 独立维护
- 哪些内容仍然残留在代码默认值中
- 后续应如何继续把兼容数据与主数据分离

## 2. 当前主数据源

当前新游戏主数据源定义为：

- `C7/Lua/game_modes/base-ruleset.json`
- `C7/Lua/game_modes/standalone.lua`
- `C7/Lua/rules/civ3.lua`

运行时入口已经明确：

- `New Game`
- `Quick Start`
- world / player setup

这些入口默认都通过 `GamePaths.DefaultNewGameMode` 装载 `SaveGame`。

## 3. 已进入 ruleset 的数据

以下内容已经以 standalone/ruleset 为主来源：

- 地形
- 地形改良
- 资源
- 单位原型
- 建筑
- 科技
- 政体
- 市民类型
- terraform 定义
- 难度
- 地图尺寸参数
- 文明数据
- 起始单位规则

说明：

- `standalone.lua` 当前主要负责对 `base-ruleset.json` 做裁剪和替换，尤其是单位美术可用性约束
- `GameModeLoader` 现在会把 ruleset 元数据写入 `SaveGame`

## 4. 当前已显式化的规则集元数据

当前 `SaveGame` 已记录：

- `DataSourceId`
- `DataSourceDisplayName`
- `DataSourceBasePath`
- `DataSourceAddons`
- `RulesScript`

意义：

- 存档和运行时数据不再只靠外部约定判断自己来自哪个 ruleset
- 后续可以在读档、调试、模组扩展时显式区分主数据源和兼容导入源

## 5. 仍残留在代码层的默认值

以下内容仍然存在代码默认值，尚未完全进入数据层：

- `GameSetup` 中玩家初始金币
- `GameSetup` 中初始时代文案
- 若干新游戏默认选项仍按 UI 代码直接选择，例如默认文明、默认难度
- legacy 导入链仍需要 `DefaultBicPath`

这些点不再阻塞主数据源成立，但仍是 `M1.2` 后续需要继续清理的目标。

## 6. 边界结论

当前可以明确判定：

- standalone/ruleset 已是主游戏唯一默认数据源
- legacy `.sav/.biq` 属于兼容导入链
- 兼容导入链不再定义新游戏默认规则
- ruleset 身份已经进入存档元数据

## 7. 下一步建议

建议在后续 `M1.2` / `M1.3` 继续推进：

- 把 `GameSetup` 中剩余默认值迁入 ruleset
- 把默认文明 / 默认难度从 UI 硬编码收口为 ruleset 可配置项
- 为 OpenCiv3 主 ruleset 增加更清晰的命名与版本字段
- 继续收缩 legacy 导入链对主流程接口的影响
