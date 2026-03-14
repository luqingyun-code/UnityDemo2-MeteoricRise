using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SkillCaster : MonoBehaviour
{
    private BattleVisual battleVisual;
    private BattleEntities  battleEntities;
    void Awake()
    {
        battleVisual = GetComponent<BattleVisual>();

        battleVisual.OnAttackEvent += CastSkill;
    }
    void OnDestroy()
    {
        if(battleVisual != null)
        {
            battleVisual.OnAttackEvent -= CastSkill;
        }        
    }
    //设置实体
    public void SetCasterBattleEntities(BattleEntities entity)
    {
        battleEntities = entity;   
    }


    //攻击事件触发攻击特效函数
    void CastSkill()
    {
        if(battleEntities == null)
        {
            Debug.Log("battleEntities为空");
        }
        //SkillExecutor是静态类，所以直接用，不需要new实例
        SkillExecutor.ExecuteSkill(battleEntities.SkillData, battleVisual);
    }
}
