using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    //公共的属性
    //公共都是大写字母开头
    //角色名
    public string MemberName;
    //初始等级
    public int StartingLevel;
    //基础生命值
    public int BaseHealth;
    //基础强度
    public int BaseStrength;
    //基础速度（崩铁里的速度）
    public int BaseInitiative;

    //公共游戏对象变量
    //战斗视觉预制体：在战斗中显示的内容
    public GameObject MemberBattleVisualPrefab;
    //过场视觉预制体：在过场中显示的内容
    public GameObject MemberOverworldVisualPrefab;
    

    
}
