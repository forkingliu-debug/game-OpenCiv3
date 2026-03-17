# OpenCiv3 项目导读

## 1. 这是什么项目

OpenCiv3 是一个用 Godot + C# 开发的开源《文明3》精神续作/重制项目。它不是简单做一个“能读 Civ3 资源的壳”，而是在尝试把《文明3》的规则、地图、单位、城市、科技、外交、AI、资源和原版素材兼容能力，逐步迁移到一套新的、可扩展的开源架构里。

从仓库当前状态看，它已经不是只有界面原型，而是具备了这些比较明确的目标：

- 用 Godot 做游戏前端、场景和交互。
- 用独立的 `C7Engine` 承载核心规则和回合推进。
- 用 `C7GameData` 风格的数据模型保存整个游戏状态。
- 用 `QueryCiv3` + `ImportCiv3` 读取 Civilization III 的 `BIQ` / `SAV` 数据。
- 支持两种资源思路：
  - 使用原版 Civ3 的素材和规则文件。
  - 使用 OpenCiv3 自己的 standalone 模式和现代图形配置。

一句话概括：这是一个“前端、引擎、数据、原版兼容层”明确分层的 Civilization III 开源重构工程。

## 2. 先看整体框架

这个项目的主干可以理解成四层：

1. `C7`
   负责 Godot 场景、UI、地图显示、输入处理、弹窗、顾问界面、主菜单等。
2. `C7Engine`
   负责回合循环、玩家行动、AI、地图生成、规则处理、消息派发、存档读写。
3. `C7GameData`
   负责保存游戏世界的核心状态，例如地图、城市、玩家、单位、科技、政府、资源。
4. `QueryCiv3` / `ConvertCiv3Media` / `Blast`
   负责兼容和读取原版 Civ3 的文件格式、压缩格式和媒体资源。

可以把它想成：

`Godot UI (C7) -> 消息 -> Engine (C7Engine) -> GameData`

以及旁边有一条“导入原版内容”的支线：

`Civ3 SAV/BIQ/素材 -> QueryCiv3 / ImportCiv3 / ConvertCiv3Media -> SaveGame / GameData -> 游戏运行`

这个分层是这个仓库最值得先理解的部分，因为作者明显在避免把规则逻辑直接写死在 Godot 界面层里。

## 3. 仓库各目录大概是做什么的

### `C7`

这是游戏客户端本体，也是 Godot 项目目录。

你可以把它理解成“表现层 + 交互层”。这里面主要有：

- `project.godot`
  - Godot 工程配置，主场景是 `UIElements/MainMenu/main_menu.tscn`
- `Game.cs`
  - 游戏主场景控制器，连接 UI 与 `C7Engine`
- `GlobalSingleton.cs`
  - Godot Autoload 单例，用来在场景切换时传递加载参数、世界配置、图形模式等
- `Map/`
  - 地图可视化图层，比如城市层、单位层、边界层、迷雾层、资源层
- `UIElements/`
  - 主菜单、新游戏设置、城市界面、外交界面、顾问界面、各种弹窗和按钮
- `Textures/`
  - 图像加载和纹理配置
- `Lua/texture_configs`
  - 纹理映射配置，可以在 OpenCiv3 图形和 Civ3 原版图形之间切换
- `Lua/game_modes`
  - 游戏模式/基础规则集定义
- `Lua/rules`
  - Lua 规则脚本

这是你以后“看这个游戏是怎么跑起来的”最常打开的目录。

### `C7Engine`

这是核心规则引擎，也是整个项目技术上最重要的目录。

这里的职责包括：

- 创建/加载游戏
- 维护全局游戏状态引用
- 推进回合
- 处理玩家行为消息
- 处理 AI
- 生成地图
- 存档导入导出
- 通过 Lua 扩展规则

关键子目录：

- `EntryPoints/`
  - UI 可以调用的引擎入口
  - 包括创建游戏、回合推进、单位交互、城市交互、消息定义
- `C7GameData/`
  - 核心数据模型
  - 包括 `GameData`、`Player`、`City`、`MapUnit`、`GameMap`、`Tech`、`Government`、`Rules`
- `AI/`
  - 玩家 AI、野蛮人 AI、寻路、战略优先级、工人/移民/战斗单位 AI
- `Lua/`
  - Lua 规则加载器和 JSON/Lua 模式转换
- `MapGenerator.cs`
  - 地图生成逻辑
- `SaveManager.cs`
  - 存档加载和保存入口

如果你只想理解“这项目真正的游戏核心在哪”，答案基本就是这里。

### `QueryCiv3`

这是 Civilization III 文件读取层，主要读 `BIQ` 和 `SAV`。

它不是做完整游戏逻辑，而是专门从原版二进制文件里解析数据段、节头、偏移和结构。仓库里有大量 `BiqSections/` 和 `SavSections/`，说明它按 Civ3 文件中的 section 进行拆分读取。

作用可以理解为：

- 读取原版 scenario / rules / save 文件
- 提供给 `ImportCiv3` 做二次转换
- 让 OpenCiv3 能复用原版规则和存档中的大量信息

### `ConvertCiv3Media`

负责读取 Civ3 的媒体资源格式，例如：

- PCX 图片
- FLC 动画

也就是说，OpenCiv3 不只是读规则数据，也在尝试读原版美术资源。

### `Blast`

这是一个外部引入的解压库，用来处理 PKWare DCL 压缩格式。Civ3 的部分文件依赖这种压缩格式，所以 `QueryCiv3` 需要它。

### `EngineTests`

引擎层测试项目，使用 xUnit。

目前覆盖方向包括：

- 地图生成
- 存档
- 城市
- 外交关系
- AI 寻路
- AI 选址

这说明项目在尝试把“可测试的规则逻辑”从 UI 中拆出来，这是架构上比较健康的信号。

### `_Console/BuildDevSave`

开发辅助工具，用来把 Civ3 存档导入成项目自己的 JSON 存档格式，方便调试。

## 4. 游戏是怎么启动和跑起来的

这是你理解项目最关键的一条链路。

### 4.1 Godot 从主菜单启动

主场景在 `C7/project.godot` 里配置为：

- `res://UIElements/MainMenu/main_menu.tscn`

对应代码入口是：

- `C7/UIElements/MainMenu/MainMenu.cs`

主菜单负责：

- 检查是否能找到 Civ3 安装目录
- 决定是否显示原版资源相关选项
- 新游戏、快速开始、加载游戏、加载场景
- 切换 OpenCiv3 图形 / Civ3 原版图形

### 4.2 场景切换靠 `GlobalSingleton`

`GlobalSingleton.cs` 是 Godot 的 autoload 单例，主要用来跨场景传数据：

- 待加载的存档路径
- 新生成的 `SaveGame`
- 新世界配置 `WorldCharacteristics`
- 当前是否启用现代图形

也就是说，主菜单不会直接把复杂对象传给下一个场景，而是先放进全局单例，再切到游戏场景。

### 4.3 进入 `C7Game.tscn` 后由 `Game.cs` 接管

`Game.cs` 是运行时总控之一。

它在 `_Ready()` 里做的关键事情是：

- 根据 `GlobalSingleton` 判断是加载存档还是启动新局
- 调用 `CreateGame.createGame(...)`
- 初始化地图视图 `MapView`
- 在 `_Process()` 中持续轮询引擎消息和 UI 消息

这里已经能看出作者的思路：`Game.cs` 不是自己算规则，而是负责驱动 UI 和引擎之间的消息流。

### 4.4 `CreateGame` 把 `SaveGame` 变成运行态 `GameData`

`C7Engine/EntryPoints/CreateGame.cs` 是创建游戏的核心入口。

它做的事情很关键：

- 从存档路径或 `SaveGame` 构造 `GameData`
- 把 `GameData` 放进 `EngineStorage.gameData`
- 找出当前人类玩家
- 调用 `TurnHandling` 开始首回合

这里能看出该项目把“存档态”和“运行态”分开了：

- `SaveGame`
  - 更像序列化/导入导出的中间格式
- `GameData`
  - 更像运行中的真实内存模型

这是比较标准也比较成熟的设计。

## 5. 引擎层是怎么组织的

### 5.1 `EngineStorage` 是运行时总线

`EngineStorage` 保存：

- 当前 `GameData`
- 当前 UI 控制玩家 ID
- 发往引擎的消息队列
- 发往 UI 的消息队列
- 动画消息队列

也就是说，这个项目目前不是事件总线 + ECS，也不是严格 DI 架构，而是一个较务实的“单例状态 + 消息队列”模型。

好处是实现成本低，适合原型快速迭代。
代价是后期如果做多人同步、回放或更严格线程模型，可能需要进一步抽象。

### 5.2 UI 和引擎通过消息交互

项目里有两类消息：

- `MessageToEngine`
  - 例如移动单位、结束回合、切换政体、建城、科研选择
- `MessageToUI`
  - 例如开始回合、显示城市界面、显示科技顾问、显示战争提示

这套机制的意义是：

- UI 不直接到处改数据
- 引擎也不直接依赖 Godot 控件
- 两边靠明确消息联动

对一个回合制策略游戏来说，这种模式很合理。

### 5.3 回合循环在 `TurnHandling`

`TurnHandling.AdvanceTurn()` 是核心回合推进器。

它做的大致流程是：

- 依次遍历玩家
- AI 玩家自动行动
- 当轮到 UI 控制的人类玩家时，发 `MsgStartTurn` 并把控制权交回前端
- 玩家结束回合后继续推进
- 回合结束时处理：
  - 野蛮人刷新
  - 每城成长/生产
  - 金钱和科研
  - 市民情绪
  - 单位回合开始状态

因此，这个项目的主循环不是 Godot 场景树驱动规则，而是：

- Godot 驱动帧刷新
- 引擎驱动回合推进

## 6. 数据模型是什么样的

### 6.1 `GameData` 是全局游戏状态

`C7Engine/C7GameData/GameData.cs` 是整个项目的“世界状态中心”。

里面包含：

- `map`
- `players`
- `cities`
- `mapUnits`
- `terrainTypes`
- `terrainImprovements`
- `Resources`
- `unitPrototypes`
- `Buildings`
- `techs`
- `governments`
- `difficulties`
- `rules`
- `barbarianInfo`

这意味着项目当前更接近传统 4X 游戏的领域模型，而不是 ECS 风格。

### 6.2 `GameData` 里不只是静态数据，还有规则辅助行为

它不只是 DTO，还包含了一些业务逻辑，比如：

- 更新地块归属
- 单位移除和生成
- 科技成本计算
- 缓存贸易网络
- 向 UI 发 Lua 消息

所以它更像一个“富领域模型”。

### 6.3 存档态和运行态做了分离

从代码结构看，这个项目明显有两套相互转换的数据：

- `C7GameData.Save/*`
  - 更偏向存档和导入导出
- `C7GameData/*`
  - 更偏向运行时对象

这是个很重要的设计点，因为 Civ3 兼容导入、新游戏初始化、JSON 存档保存，都更容易落在 `SaveGame` 这层做。

## 7. 它怎样兼容 Civilization III

这是这个项目非常有特色的一块。

### 7.1 先读原版文件

`QueryCiv3` 负责读取：

- `.BIQ`
- `.SAV`

它不是靠“猜固定偏移”硬读到底，而是通过 section header + offset 的方式组织读取。代码里按 `BiqSections/`、`SavSections/` 拆了很多结构体，说明作者在认真做 Civ3 文件格式逆向与工程化封装。

### 7.2 再转成 OpenCiv3 自己的存档模型

`ImportCiv3.cs` 是最重的兼容桥接文件之一。

它会把 Civ3 数据转成 OpenCiv3 的 `SaveGame`，内容包括：

- 文明
- 玩家
- 科技
- 地图
- 单位
- 城市
- 地形
- 资源
- 建筑
- 地图包裹方式
- 野蛮人强度
- 原版规则参数

也就是说，项目不是直接在原版数据结构上跑逻辑，而是：

- 读原版
- 转成自己的统一模型
- 用自己的引擎运行

这比“原版数据直接绑 UI”要稳很多。

### 7.3 还能读原版素材

`ConvertCiv3Media` 可以读 PCX/FLC。

再加上 `C7/Lua/texture_configs/civ3.lua` 这一类配置，可以看出项目有明确的目标：尽可能在新引擎里复用原版美术资源，而不是完全重做资源管线。

## 8. Lua 在这个项目里扮演什么角色

Lua 不是用来写整个游戏，而是用来做“规则扩展层”。

### 8.1 `RulesEngine` 负责把 Lua 函数导入为 C# 委托

`C7Engine/Lua/RulesEngine.cs` 会：

- 初始化 MoonSharp Lua 虚拟机
- 注册 `C7GameData` 里的公共类型和枚举
- 暴露 `GAME_DATA()` 等全局接口
- 从 Lua 脚本里按路径取函数
- 转成 C# delegate 后调用

这意味着：

- 核心执行框架在 C#
- 具体规则可以部分下放到 Lua

这是一种很典型的“引擎强类型 + 脚本规则可配”的做法。

### 8.2 `GameModeLoader` 支持 JSON 基础模式 + Lua 补丁

`C7/Lua/game_modes/base-ruleset.json` 是基础模式，
`standalone.lua` 这类脚本像是对基础模式的二次加工。

`GameModeLoader` 的流程是：

- 先读 JSON 或 Lua 的基础模式
- 再按顺序应用 addon Lua
- 最后转成 JSON 再反序列化为 `SaveGame`

因此，这个项目的一个重要方向是“可模组化规则装配”，而不是只有一套写死规则。

## 9. 地图生成和 AI 已经做到什么程度

### 9.1 地图生成不是占位符，而是一套完整流程

`MapGenerator.cs` 代码量很大，而且结构清楚，说明这部分不是临时写的。

它大致包含：

- 地表形状生成
- 大陆架修正
- 山地丘陵分布
- 生物群系划分
- 河流生成
- 奢侈/战略/奖励资源分布
- 玩家出生点选择
- 野蛮人营地生成
- 地形贴图细节分配

这已经是标准 4X 地图生成器的规模了。

### 9.2 AI 也不是空壳

`C7Engine/AI/` 里已经有：

- 路径搜索
- 战略优先级
- 野蛮人 AI
- 城市地块分配 AI
- 单位 AI
  - 工人
  - 移民
  - 探索者
  - 护卫
  - 防守者
  - 战斗单位

这说明项目虽然整体仍然是 pre-alpha，但“引擎底子”已经在往真正可玩方向走，而不只是 UI 展示。

## 10. 前端层目前是什么风格

从 `C7` 目录看，当前前端不是追求全新审美的重设计，而更像：

- 尽量还原 Civ3 交互结构
- 同时逐步替换为自己的 Godot 组件和纹理配置系统

你会看到这些典型界面：

- 主菜单
- 新游戏设置
- 城市界面
- 外交界面
- 科技顾问
- 军事顾问
- 宫殿界面
- 单位按钮
- 右键菜单
- 各类弹窗

地图部分也不是一个单 Node 画到底，而是拆分成很多 layer，这符合策略游戏地图渲染的常见做法。

## 11. 这个项目当前成熟度怎么判断

我的判断是：

- 它已经超过“概念验证”
- 但还没有到“内容完整、可长期游玩”的程度

更具体一点：

- 架构层面：已经有比较成熟的分层和职责边界
- 规则层面：已有相当多系统，但明显仍在持续填充
- UI 层面：核心界面已有雏形，仍处在迭代中
- 工具链层面：有测试、有导入工具、有资源兼容层
- 内容层面：离完整 Civilization III 替代品还有距离

README 也明确说它还是 early pre-alpha，这和代码现状是吻合的。

## 12. 我读仓库后觉得值得你注意的几个点

### 12.1 这是“以兼容 Civ3 为起点”，不是“纯复刻”

项目既尊重原版数据和素材，又在引擎和规则组织上做现代化改造，比如：

- UI / Engine / Data 分层
- Lua 可配置规则
- 独立测试
- standalone 模式

所以它更像“站在 Civ3 肩膀上的新项目”。

### 12.2 代码里能看出历史演进痕迹

仓库中还能看到旧命名 `C7`，而项目展示名已经是 `OpenCiv3`。这说明仓库不是从零按最终名字设计的，而是在持续演化。

这种演化痕迹也体现在文档上：

- `doc/dev_environment.md` 还写着 `.NET 6` 和较旧的 Godot/IDE 说明
- 但当前 `.csproj` 已经是 `net8.0`
- `C7` 项目使用的是 `Godot.NET.Sdk/4.4.1`

所以你阅读时要注意：仓库里部分文档已经落后于代码。

### 12.3 当前环境验证结果也说明了这点

我尝试运行测试时，当前机器默认 `dotnet` 是 5.0 SDK，结果无法构建 `net8.0` 目标。

这不是仓库本身报错，而是本机 SDK 版本不够。换句话说，这个项目现在实际需要的是更高版本 .NET SDK，而不是旧文档里写的配置。

## 13. 推荐你怎么阅读这个项目

如果你想高效理解它，我建议按这个顺序：

1. 先看 `README.md`
   - 了解项目定位和各目录职责
2. 再看 `C7/project.godot`、`C7/UIElements/MainMenu/MainMenu.cs`
   - 看游戏如何进入主菜单
3. 再看 `C7/GlobalSingleton.cs`、`C7/Game.cs`
   - 看前端与引擎怎么接起来
4. 再看 `C7Engine/EntryPoints/CreateGame.cs`
   - 看新局/读档如何进入运行态
5. 再看 `C7Engine/EngineStorage.cs`、`C7Engine/EntryPoints/MessageToEngine.cs`、`C7Engine/EntryPoints/MessageToUI.cs`
   - 看消息机制
6. 再看 `C7Engine/EntryPoints/TurnHandling.cs`
   - 看回合主循环
7. 再看 `C7Engine/C7GameData/GameData.cs`
   - 看整个世界状态模型
8. 然后选一条专题深入
   - 想看 Civ3 兼容：读 `QueryCiv3` 和 `ImportCiv3.cs`
   - 想看地图算法：读 `MapGenerator.cs`
   - 想看 AI：读 `C7Engine/AI/`
   - 想看界面：读 `C7/UIElements/` 和 `C7/Map/`
   - 想看规则扩展：读 `C7Engine/Lua/` 与 `C7/Lua/rules/`

## 14. 一份简短结论

如果把它当成一个开源游戏项目来看，OpenCiv3 最大的价值不只是“它是 Godot 做的文明3”，而是它已经形成了一套比较清楚的工程骨架：

- Godot 前端
- 独立 C# 引擎
- 明确的数据模型
- 原版 Civ3 文件兼容层
- Lua 规则扩展层
- AI 与地图生成系统
- 测试与导入辅助工具

所以它最适合研究的地方不是某个单独功能，而是“它如何把一个老牌 4X 游戏拆成现代开源项目可维护的模块”。

如果你后面要继续深挖，我建议优先研究两条线：

- 一条是 `Game.cs -> CreateGame -> EngineStorage -> TurnHandling -> GameData`
- 一条是 `QueryCiv3 -> ImportCiv3 -> SaveGame -> GameData`

前一条告诉你“游戏怎么跑”；
后一条告诉你“它怎么把 Civ3 的世界搬进来”。
