using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//攻击类型
public enum SkillSpType
{
    NormalAttack,//普攻
    BattleSkill//战绩
}

public enum SkillAttackType
{
    Melee, //近战
    Ranged //远战
}

//技能目标阵营
public enum SkillTargetCamp
{
    Ally,
    Enemy
}

//单个目标还是群体目标
public enum SkillTargetMode
{
    Single,
    Multi
}

//技能类型
public enum SkillEffectType
{
    Damage,
    Heal
}

//[CreateAssetMenu(menuName = "New Skill")]
public abstract class Skill : ScriptableObject
{
    public string SkillName;

    //数值，暂时不区分技能和治疗啥的
    public int Power;

    //攻击类型：近战/远程
    public SkillAttackType AttackType;

    //技能目标：我方/敌方
    public SkillTargetCamp TargetCamp;

    //技能目标模式：单体/群体
    public SkillTargetMode TargetMode;

    //技能类型：治疗，攻击，增益，减益
    public SkillEffectType EffectType;

    //技能类型：战绩/普攻
    public SkillSpType skillSpType;
    public int skillPointCost;


    public GameObject SkillEffectPrefab;
    //拖动技能特效预制体的地方，这样后面每个技能都能知道自己的特效
    // 这些技能最后一定要New Skill，然后不同的skill就拖动不同的技能特效。


    //每个Skill自己实现Execute（技能执行）
    public abstract void Execute(BattleVisual battleVisual);
    //技能执行逻辑（保留扩展性）
    /*public abstract IEnumerator Execute(
        BattleVisual casterVisual,
        List<BattleEntities> targets
    );*/
}

