using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*FelixSkillMagic脚本负责：
1.什么时候生成魔法
2.谁发射魔法
3.目标是谁
*/
[CreateAssetMenu(menuName = "New Skill/Felix/BattleSkill")]
public class FelixBattleSkill : Skill
{
    [SerializeField] private float effectLifetime = 2f;
    //继承的虚函数，用于执行特效
     public override void Execute(BattleVisual battleVisual)
    {
        if (SkillEffectPrefab == null) return;
        List<Transform> targets = battleVisual.GetAttackTargets();
        if (targets == null || targets.Count == 0) return;
        foreach(var t in targets)
        {
            if(t == null) continue;
            Vector3 skillPosition = t.position;
            GameObject effect = Instantiate(SkillEffectPrefab, skillPosition,
            Quaternion.identity);
            Destroy(effect, effectLifetime);
        }
    }
}
