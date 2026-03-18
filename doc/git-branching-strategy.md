# Git Branching Strategy

## 1. 目的

本文档定义 OpenCiv3 当前阶段的 Git 分支策略，用于统一以下事项：

- 哪个分支作为长期主干
- 日常开发从哪个分支切出
- 功能分支如何命名
- 提交、合并、同步 `origin` / `upstream` 的基本规则

本策略优先贴合仓库当前实际状态，而不是假设未来再改名为 `main` / `develop`。

## 2. 长期分支模型

当前采用单主干开发模型，长期分支如下：

- 主分支：`Development`
- 开发分支：`Development`

说明：

- 上游仓库 `upstream` 的默认分支当前为 `Development`
- 本地与 `origin` 当前默认工作分支也使用 `Development`
- 在后续没有明确执行分支改名迁移前，不额外引入第二条长期开发分支

结论：

- `Development` 既是主干分支，也是日常集成分支
- 所有短期开发工作都应从 `Development` 切出，并最终合回 `Development`

## 3. 短期分支命名

短期分支统一从本地最新的 `Development` 切出，推荐命名如下：

- 功能开发：`feature/<topic>`
- 缺陷修复：`fix/<topic>`
- 文档整理：`docs/<topic>`
- 重构整理：`refactor/<topic>`
- 实验验证：`spike/<topic>`

命名规则：

- 使用小写字母、数字和连字符
- 不使用空格和中文
- `<topic>` 应直接表达任务目标

示例：

- `feature/standalone-new-game-flow`
- `fix/ruleset-loader-null-check`
- `docs/m0-1-2-branching-strategy`

## 4. 日常开发流程

标准流程：

1. 先同步本地 `Development`
2. 从 `Development` 切出短期分支
3. 在短期分支提交小步可回滚的变更
4. 完成后合并回 `Development`
5. 将结果推送到 `origin`

约束：

- 不直接在旧分支上叠加新的无关任务
- 一个短期分支只解决一类问题
- 分支生命周期尽量短，避免长期漂移

## 5. 提交规则

提交要求：

- 一次提交只表达一个清晰意图
- 提交前保证代码至少通过本次改动相关的基本验证
- 文档、重构、功能、修复尽量分开提交

推荐提交前缀：

- `feat:`
- `fix:`
- `docs:`
- `refactor:`
- `test:`
- `chore:`

示例：

- `feat: add standalone ruleset bootstrap path`
- `fix: guard null result in map setup`
- `docs: define repository branching strategy`

## 6. 合并规则

默认合并目标：

- 所有短期分支默认合并回 `Development`

合并要求：

- 合并前先把目标分支更新到最新
- 先处理冲突，再做一次最小必要验证
- 不把未验证完成的临时实验直接合入 `Development`

建议：

- 小任务优先使用 `--no-ff` 合并保留分支语义，或在代码托管平台使用 squash merge 保持主干整洁
- 具体采用 merge 还是 squash，以当次变更是否需要保留中间提交历史为准

## 7. `origin` 与 `upstream` 关系

远程定义：

- `origin`：个人或团队自有仓库，用于日常推送
- `upstream`：原始 OpenCiv3 仓库，用于跟踪上游变化

规则：

- 日常开发分支推送到 `origin`
- 上游同步基线以 `upstream/Development` 为准
- 不直接向 `upstream` 推送本仓库的日常实验性分支

何时从 `upstream` 同步：

- 开始新任务前，发现本地主干落后时
- 准备做较大改动前，减少后续冲突
- 计划长期维护分支前，先对齐上游基线

何时推送到 `origin`：

- 本地阶段性可用时
- 需要跨机器继续工作时
- 需要保留检查点或分享进展时

## 8. 常用命令示例

同步主干：

```powershell
git checkout Development
git fetch upstream
git merge upstream/Development
git push origin Development
```

开始新功能：

```powershell
git checkout Development
git pull origin Development
git checkout -b feature/standalone-new-game-flow
```

完成功能后合并回主干：

```powershell
git checkout Development
git pull origin Development
git merge --no-ff feature/standalone-new-game-flow
git push origin Development
```

清理已合并分支：

```powershell
git branch -d feature/standalone-new-game-flow
git push origin --delete feature/standalone-new-game-flow
```

## 9. 当前阶段决策

在 `M0` 阶段，正式采用以下策略：

- 以 `Development` 作为唯一长期主干分支
- 以 `feature/*` 为主的短期分支承载具体任务开发
- 先保证主干稳定推进，再视需要决定是否未来拆分出独立 `main` / `develop`

若未来仓库决定做分支迁移，应先更新本文件，再执行实际分支改名或保护规则调整。
