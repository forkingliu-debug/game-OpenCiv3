# Runtime Path Baseline

## 1. 目的

本文档用于落实 `M0.3` 主运行路径确认，明确以下问题：

- 当前新游戏启动链路是什么
- 哪些部分已经可以走 standalone / ruleset 主路径
- 哪些部分仍然依赖 Civ3 安装目录或原版媒体
- 进入 `M1` 之前，主运行路径应收敛到什么边界

本文档不把“已经有 standalone 模式”误写成“已经完成去原版依赖”，而是区分现状、残余耦合和下一步目标。

## 2. 当前结论

当前仓库已经具备 standalone 主路径雏形，但尚未完全成为默认且完整的主运行路径。

已成立的事实：

- 新游戏数据源可以直接从 `base-ruleset.json + standalone.lua` 构造
- `QuickStart` 和 `New Game` 已能通过 `GameModeLoader` 生成 `SaveGame`
- 主菜单在未找到 Civ3 路径时，允许用户切换到 standalone 模式继续进入游戏
- standalone 模式下，媒体解析优先使用 OpenCiv3 自带资源

尚未成立的事实：

- 游戏整体运行层还没有完全摆脱对 `DefaultBicPath` 的默认假设
- 主菜单音频和部分 UI/资源加载仍可能走 `Civ3MediaPath`
- “standalone 是默认主流程”在产品行为层面还没有彻底固化

结论：

- `M0.3` 当前应判定为“进行中”
- `M1` 可以在本基线文档基础上继续拆分启动流程、数据源和兼容模块职责

## 3. 当前启动链路

### 3.1 主菜单入口

主菜单入口在 [MainMenu.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/MainMenu/MainMenu.cs#L37)。

当前逻辑：

- 若未启用 standalone 且找不到经典 Civ3 图形，则显示 `NoCiv3Options`
- 用户可以选择设置 Civ3 目录
- 用户也可以直接点击“Play in standalone mode”

对应 UI 位于 [main_menu.tscn](D:/Project-AI/game-OpenCiv3/C7/UIElements/MainMenu/main_menu.tscn#L84)。

这说明项目已经承认“没有 Civ3 目录也应可进入游戏”，这是 `M0.3` 的正确方向。

### 3.2 新游戏入口

快速开始入口在 [QuickStartSetup.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/NewGame/QuickStartSetup.cs#L12)。

当前逻辑：

- 调用 `GameModeLoader.Load(GamePaths.GameModesDir, GamePaths.GameMode)`
- `GamePaths.GameMode` 会根据 `UseStandaloneMode()` 选择 `basic` 或 `standalone`
- `standalone` 的定义是 `base-ruleset.json + standalone.lua`

对应定义位于 [GamePaths.cs](D:/Project-AI/game-OpenCiv3/C7/GamePaths.cs#L30) 和 [GameModeLoader.cs](D:/Project-AI/game-OpenCiv3/C7Engine/Lua/GameModeLoader.cs#L29)。

世界设置 / 玩家设置链路同样基于 `GameModeLoader`，说明“新游戏数据构造”已经具备脱离 Civ3 目录的能力。

### 3.3 进入游戏场景

游戏场景加载入口在 [Game.cs](D:/Project-AI/game-OpenCiv3/C7/Game.cs#L112)。

当前逻辑：

- `Game` 场景从 `GlobalSingleton` 读取 `SaveGame` 或存档路径
- 再调用 `CreateGame.createGame(...)`
- `CreateGameParams` 当前始终带入 `GamePaths.DefaultBicPath`

这意味着：

- 新游戏数据可以来自 standalone ruleset
- 但运行时创建流程仍保留了“默认 BIQ 路径始终存在”的接口假设

这正是 `M0.3` 和 `M1.1/M1.2` 之间的边界。

## 4. standalone 已具备的能力

### 4.1 规则数据装载

`GameModeLoader` 已支持：

- 从 JSON 或 Lua 读取基础规则
- 通过 addon Lua 对规则继续加工
- 最终生成 `SaveGame`

见 [GameModeLoader.cs](D:/Project-AI/game-OpenCiv3/C7Engine/Lua/GameModeLoader.cs#L25)。

### 4.2 standalone 规则裁剪

`standalone.lua` 已在 ruleset 层做了明确裁剪：

- 只保留已有替代美术的单位
- 对无替代资源的升级链做截断

见 [standalone.lua](D:/Project-AI/game-OpenCiv3/C7/Lua/game_modes/standalone.lua#L36)。

这说明 standalone 并不是“仅仅换个开关”，而是已有一条独立规则变体链路。

### 4.3 自带资源回退

`Util.Civ3MediaPath` 当前行为是：

- 先尝试 scenario mod 路径
- standalone 模式下优先查找 C7 自带资源
- 非 standalone 模式下优先查找 Civ3 目录，找不到时再回退到 C7 资源

见 [Util.cs](D:/Project-AI/game-OpenCiv3/C7/Util.cs#L113)。

这表明项目已经具备“自带资源可运行”的基础设施，只是仍与旧路径共存。

## 5. 仍然存在的 Civ3 依赖点

### 5.1 默认 BIQ 路径假设

[GamePaths.cs](D:/Project-AI/game-OpenCiv3/C7/GamePaths.cs#L45) 仍定义：

- `DefaultBicPath = Util.GetCiv3Path() + "/Conquests/conquests.biq"`

[Game.cs](D:/Project-AI/game-OpenCiv3/C7/Game.cs#L117) 和 [ScenarioSetup.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/NewGame/ScenarioSetup.cs#L107) 仍把它传入创建或加载流程。

风险：

- 即使 standalone 新游戏可生成 `SaveGame`，底层接口仍默认携带 Civ3 规则文件路径
- 这会阻碍后续把主流程彻底定义为“自有 ruleset first”

### 5.2 主菜单音频仍走 Civ3 资源解析

[MainMenu.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/MainMenu/MainMenu.cs#L151) 的按钮音效仍通过 `LoadCiv3WAVFromDisk("Sounds/Button1.wav")` 加载。

影响：

- standalone 模式下如果 C7 资源中没有对应音频，主菜单行为会部分退化
- 虽然当前空值被容错处理，但仍属于“主菜单依赖旧资源约定”

### 5.3 部分 UI/媒体系统仍复用 Civ3 媒体入口

例如：

- [MainMenuMusicPlayer.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/MainMenu/MainMenuMusicPlayer.cs)
- [PopupOverlay.cs](D:/Project-AI/game-OpenCiv3/C7/UIElements/Popups/PopupOverlay.cs)
- [AnimationManager.cs](D:/Project-AI/game-OpenCiv3/C7/Animations/AnimationManager.cs)
- [TextureLoader.cs](D:/Project-AI/game-OpenCiv3/C7/Textures/TextureLoader.cs)

这些模块多数已经能借由 `Civ3MediaPath -> C7 fallback` 运行，但职责仍混在“Civ3 解析入口”里。

### 5.4 scenario / 导入流程仍属于兼容链

场景读取和导入流程仍显著依赖：

- `DefaultBicPath`
- `PediaIcons.txt`
- `scenarioSearchPath`
- `Util.setModPath(...)`

这类逻辑应保留，但应从“主流程默认路径”降级为“兼容/导入工具链”。

## 6. `M0.3` 的目标运行链路

`M0.3` 完成时，建议主运行链路定义为：

1. 启动游戏
2. 默认进入 OpenCiv3 主菜单
3. 玩家可直接开始 `New Game` / `Quick Start`
4. 新游戏默认从 OpenCiv3 ruleset 生成 `SaveGame`
5. 进入游戏场景时，不要求预先存在 Civ3 安装目录
6. Civ3 路径仅在“导入原版素材 / 加载原版场景 / 兼容研究”时才需要

对应原则：

- standalone / ruleset 是主产品路径
- Civ3 import 是辅助工具路径
- 两者可以共存，但不能再由兼容路径定义主流程

## 7. 过渡期兼容策略

在 `M1` 正式重构前，建议采用以下过渡策略：

- 保留 `QueryCiv3`、`ConvertCiv3Media`、`Blast`，但明确其职责是兼容层
- 保留 scenario / 原版存档读取能力，但不再作为默认“开始新游戏”的前提
- 保留 `Civ3MediaPath` 的回退能力，但逐步把主菜单、通用 UI、基础音频迁移到明确的 OpenCiv3 资源入口
- 在接口层允许 `CreateGameParams` 走“无 DefaultBicPath 的 standalone 创建”

## 8. 进入 `M1` 前必须达成的边界

进入 `M1` 前，至少应确认以下边界：

- `New Game` 和 `Quick Start` 被正式认定为 standalone-first
- “未设置 Civ3 目录”不再阻断主流程进入
- `DefaultBicPath` 不再是所有创建流程的隐含必需参数
- 主菜单和基础 UI 不依赖 Civ3 音频/基础媒体才能正常工作
- 场景加载、原版导入、原版兼容被归类到辅助路径

## 9. 推荐的 `M1` 切入顺序

建议按以下顺序进入 `M1`：

1. `M1.1` 启动流程重构
2. `M1.2` 数据源梳理
3. `M1.3` 兼容模块降级

原因：

- 先把主菜单和新游戏路径定为 standalone-first
- 再把 ruleset / 数据源从接口上独立出来
- 最后再处理兼容模块的职责收缩，风险最低
