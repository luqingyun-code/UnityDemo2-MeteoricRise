# MeteoricRise

一个基于 Unity 的 2.5D 回合制角色扮演游戏 Demo，展示游戏客户端开发的核心技术与设计模式。

## 项目概述

MeteoricRise 是一款原创的回合制战斗游戏原型，采用 2.5D 视角呈现。项目展示了完整的游戏战斗循环、可扩展的技能系统和数据驱动的角色管理。此项目作为游戏客户端开发实习的技术展示，体现了扎实的编程基础和架构设计能力。

## 功能特性

- **完整的回合制战斗系统** - 基于先攻值排序的行动序列、普攻/战技/逃跑多选决策
- **技能资源管理** - 战技点（SP）系统，普攻恢复、战技消耗的战术循环
- **多角色队伍管理** - 3 人小队编队、跨场景状态持久化、动态存活判定
- **可扩展技能框架** - ScriptableObject 数据驱动，支持单体/群体、近战/远程、攻击/治疗多种组合
- **2.5D 战斗表现** - 角色移动、攻击动画、受击反馈、漂浮伤害数字
- **遇敌系统** - 场景化敌人生成、等级区间配置

## 技术栈

| 类别 | 技术 |
|------|------|
| 引擎 | Unity 2022.3.15f1c1 |
| 语言 | C# |
| UI | TextMeshPro |
| 架构模式 | ScriptableObject 数据驱动、单例模式、状态机 |

## 项目结构

```
Assets/
├── Scripts/                      # 核心游戏逻辑
│   ├── BattleSystem.cs           # 战斗状态机与回合调度
│   ├── BattleVisual.cs           # 战斗视觉表现控制
│   ├── BattleStateEffectManager.cs   # 状态特效管理
│   ├── PartyManager.cs           # 玩家队伍管理（单例）
│   ├── EnemyManager.cs           # 敌人生成与管理
│   ├── EncounterSystem.cs        # 场景遇敌触发
│   ├── Skill.cs                  # 技能抽象基类
│   ├── SkillCaster.cs            # 技能施放监听器
│   ├── SkillExecutor.cs          # 技能执行静态类
│   ├── PlayerController.cs       # 大地图玩家控制
│   ├── PartyMemberInfo.cs        # 角色数据配置
│   ├── EnemyInfo.cs              # 敌人数据配置
│   ├── UIabout/
│   │   └── FloatingText.cs       # 伤害数字飘字
│   └── SkillScripts/             # 具体技能实现
│       ├── ClarieSkill/          # 角色专属技能
│       ├── FelixSkill/
│       ├── HongSkill/
│       └── JohnSkill/
├── Input Settings/               # 输入配置
├── Settings/                     # 游戏配置数据
└── UnityShopDownload/            # 第三方资源
```

## 核心系统说明

### 1. 战斗系统 (BattleSystem)

战斗系统采用状态机设计，包含以下状态：

- **Start** - 初始化战斗实体、确定行动顺序
- **Selection** - 玩家为每个角色选择行动（普攻/战技/目标）
- **Battle** - 按先攻值顺序执行所有单位行动
- **Won/Lost** - 战斗结算与场景切换

核心特性：
- 先攻值驱动的行动顺序
- 动态目标选择（尸体自动重定向）
- 逃跑概率判定（50% 成功率）

### 2. 技能系统

采用 `ScriptableObject` 实现数据驱动的技能配置：

```csharp
// 技能配置维度
public enum SkillAttackType  { Melee, Ranged }      // 攻击方式
public enum SkillTargetCamp  { Ally, Enemy }        // 目标阵营
public enum SkillTargetMode  { Single, Multi }      // 单体/群体
public enum SkillEffectType  { Damage, Heal }       // 效果类型
public enum SkillSpType      { NormalAttack, BattleSkill } // 技能分类
```

扩展新技能仅需：
1. 继承 `Skill` 类
2. 实现 `Execute(BattleVisual)` 方法
3. 创建对应的 ScriptableObject 资源

### 3. 队伍与状态管理

- **PartyManager** - 跨场景单例，管理队伍血量、位置持久化
- **DontDestroyOnLoad** - 保证战斗与大地图数据同步
- **存活过滤** - `GetCurrentParty()` 自动移除阵亡成员

### 4. 技能资源循环

| 行动 | SP 变化 |
|------|---------|
| 普通攻击 | +1 |
| 释放战技 | -技能配置值 |
| 上限 | 5 |

设计意图：鼓励普攻积累资源，战技作为爆发手段

## 快速开始

### 环境要求

- Unity 2022.3 LTS 或更高版本
- 支持 TextMeshPro 的 Unity 版本

### 运行步骤

1. 克隆或下载本项目
2. 使用 Unity Hub 打开项目根目录
3. 等待 Unity 完成导入与编译
4. 打开 `OverworldScene` 开始游戏

### 操作说明

- **WASD** - 移动角色
- **战斗界面** - 点击按钮选择行动与目标

## 设计亮点

- **高内聚低耦合** - 技能系统通过抽象基类与执行器分离，易于扩展
- **数据驱动** - 角色与技能配置通过 ScriptableObject 管理，策划可独立调整
- **状态同步** - 战斗结果正确持久化到大地图场景
- **视觉反馈** - 完整的攻击、受击、治疗、飘字表现

## 扩展方向

- [ ] Buff/Debuff 状态系统
- [ ] 元素属性克制
- [ ] 技能冷却机制
- [ ] AI 决策树（目前敌人随机攻击）
- [ ] 经验与升级系统（框架已预留 `CurrExp/MaxExp`）
- [ ] 战斗 camera 运镜

## 开发日志

完整的开发记录见 [2.5D回合制demoReadMe.md](./2.5D回合制demoReadMe.md)

---

**开发时间**：2024 年 11 月 - 2025 年 3 月
**项目性质**：个人原创项目，用于求职游戏客户端开发实习岗位
