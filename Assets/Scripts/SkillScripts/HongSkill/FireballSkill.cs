using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*FireballSkill脚本负责：
1.什么时候生成火球
2.谁发射火球
3.目标是谁
*/

//在创建技能那里New Skill下面会有Fireball
[CreateAssetMenu(menuName = "New Skill/Fireball")]
public class FireballSkill : Skill
{
    //继承的虚函数，用于执行特效
     public override void Execute(BattleVisual battleVisual)
    {
        if (SkillEffectPrefab == null) return;

        List<Transform> targets = battleVisual.GetAttackTargets();
        if(targets == null || targets.Count != 1) return;

        Vector3 spawnPos = battleVisual.transform.position + Vector3.up * 2;

        GameObject effect = Instantiate(
            SkillEffectPrefab,
            spawnPos,
            Quaternion.identity
        );

        FireballController controller = effect.GetComponent<FireballController>();

        if (controller != null)
            controller.SetTarget(targets[0]);
    }
}
