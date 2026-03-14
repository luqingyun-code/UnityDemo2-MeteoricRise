using UnityEngine;

//技能执行器不需要状态（不存储数据，执行完就结束），所以可以写成static
public static class SkillExecutor
{
    //构建特效的函数（已弃用）
    /*public static void SpawnSkillEffect(Skill skill, Transform spawnPoint)
    {
        if (skill.SkillEffectPrefab == null) return;

        GameObject.Instantiate(
            skill.SkillEffectPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }*/

    /*public static void ExecuteSkill(Skill skillData, BattleVisual battleVisual)
    {
        if (skillData.SkillEffectPrefab == null) return;

        Transform target = battleVisual.GetAttackTarget();
        //仅火球限定初始位置
        Vector3 spawnPos = battleVisual.transform.position + Vector3.up * 2;
        //这里的effect就是火球啦
        GameObject effect  = GameObject.Instantiate(
            skillData.SkillEffectPrefab,
            spawnPos,
            Quaternion.identity
        );

        FireballController controller = effect.GetComponent<FireballController>();
        if(controller != null) controller.SetTarget(target);
    }*/
    public static void ExecuteSkill(Skill skillData, BattleVisual battleVisual)
    {
        if (skillData == null) return;
        skillData.Execute(battleVisual);
    }
}
