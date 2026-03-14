using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState {Start, Selection, Battle, Won, Lost, Run}
    
    [Header("Battle State")]
    [SerializeField] private BattleState state;

    [Header("Spwan Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [Header("Battle Resources")]
    [SerializeField] private int skillPoints = 3;
    [SerializeField] private int maxSkillPoints = 5;

    [Header("UI")]
    [SerializeField] private GameObject[] selectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;
    [SerializeField] private GameObject floatingTextPrefab;

    //战绩点UI
    [SerializeField] private GameObject skillPointUI;
    [SerializeField] private TextMeshProUGUI skillPointText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer = 0;

    private const string ACTION_MESSAGE = "的回合";
    private const string WIN_MESSAGE = "梦境消散了，愿我们在清醒的世界再会！";
    private const string LOST_MESSAGE = "梦魇吞噬了你。。。";
    private const string RUN_SUCCESS_MESSAGE = "如有神助，你逃离了梦境！";
    private const string RUN_FAILED_MESSAGE = "深呼吸，头晕是正常的，你无法逃脱！";
    private const string OVERWORLDSCENE = "OverworldScene";

    private const string BATTLESCENE_PROMT = "遭遇噩梦，进入战斗~\n" + 
                                              "请击败梦魇活下来！！！";
    private const int TURN_DURATION = 2;

    private const int RUN_CHANCE = 50;

    

    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        StartCoroutine(ShowBattleMenuAfterDelay());
        DetermineBattleOrder();
    }

    private IEnumerator ShowBattleMenuAfterDelay()
    {
        // 1. 先显示提示词
        bottomTextPopUp.SetActive(true);
        skillPointUI.SetActive(true);
        bottomText.text = BATTLESCENE_PROMT;

        // 2. 等 2 秒
        yield return new WaitForSeconds(4);

        // 3. 再显示战斗菜单
        ShowBattleMenu();
    }

    private IEnumerator BattleRoutine()
    {
        selectionMenu.SetActive(false);
        bottomTextPopUp.SetActive(true);
        skillPointUI.SetActive(true);
        state = BattleState.Battle;
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(state == BattleState.Battle && allBattlers[i].CurrHealth >0)
            {
                switch (allBattlers[i].BattleAction)
                {
                    case BattleEntities.Action.NormalAttack:
                    case BattleEntities.Action.BattleSkill:
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        yield return StartCoroutine(RunRoutine());
                        break;
                    default:
                        Debug.Log("错误：不存在的实体状态");
                        break;
                }
            }
        }

        //把所有尸体移除
        RemoveDeadBattlers();

        //战斗胜利
        if(enemyBattlers.Count <= 0)
            {
                state = BattleState.Won;
                bottomText.text = WIN_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(OVERWORLDSCENE);
            }

        //战斗失败
        if(playerBattlers.Count <= 0)
        {
            state = BattleState.Lost;
            bottomText.text = LOST_MESSAGE;
            yield return new WaitForSeconds(TURN_DURATION);
            Debug.Log("很遗憾，前方的区域以后再来探索吧！");
            SceneManager.LoadScene(OVERWORLDSCENE);
        }



        if(state==BattleState.Battle)
        {
            //bottomTextPopUp.SetActive(false);
            //skillPointUI.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }
        yield return null;   
    }

    private IEnumerator AttackRoutine(int i)
    {
        if(allBattlers[i].IsPlayer == true)
        {
            BattleEntities currentAttacker = allBattlers[i];
            Skill skill = currentAttacker.SkillData;
            if(currentAttacker.Target == null ||
                currentAttacker.Target.CurrHealth <= 0)
            {
                currentAttacker.Target = GetRandomEnemy();
                // 如果已经没有敌人了，跳过行动
                if(currentAttacker.Target == null)
                {
                    yield break;
                }
                //Debug.Log("尸体在说话，触发了随机选择");
            }
            //BattleEntities currentTarget = allBattlers[currentAttacker.Target];
            List<BattleEntities> currentTargets = GetTargets(currentAttacker);
            yield return StartCoroutine(AttackAction(currentAttacker,currentTargets));
            yield return new WaitForSeconds(TURN_DURATION);
            foreach(var t in currentTargets)
            {
                if(t.CurrHealth <= 0)
                {
                    bottomText.text = string.Format("{0} 击败了 {1}", currentAttacker.Name, t.Name);
                    yield return new WaitForSeconds(TURN_DURATION);
                }
            }
            //玩家回合结束;
        }
        if(allBattlers[i].IsPlayer == false)
        {
            BattleEntities currentAttacker = allBattlers[i];
            Skill skill = currentAttacker.SkillData;
            currentAttacker.SetTarget(GetRandomPartyMember());
            if(currentAttacker.Target == null)
            {
                yield break;
            }
            List<BattleEntities> currentTargets = GetTargets(currentAttacker);
            yield return StartCoroutine(AttackAction(currentAttacker,currentTargets));
            yield return new WaitForSeconds(TURN_DURATION);
            foreach(var t in currentTargets)
            {
                if(t.CurrHealth <= 0)
                {
                    bottomText.text = string.Format("{0} 击败了 {1}", currentAttacker.Name, t.Name);
                    yield return new WaitForSeconds(TURN_DURATION);
                    //playerBattlers.Remove(t);
                    //allBattlers.Remove(t);
                }
            }
            //敌人回合结束;
        }
        
    }

    private IEnumerator RunRoutine()
    {
        if(state == BattleState.Battle)
        {
            if(UnityEngine.Random.Range(1,101) >= RUN_CHANCE)
            {
                bottomText.text = RUN_SUCCESS_MESSAGE;
                state = BattleState.Run;
                allBattlers.Clear();
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(OVERWORLDSCENE);
                yield break;
            }
            else
            {
                bottomText.text = RUN_FAILED_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }
    }



    //回合结束后统一删除血量低于0的角色
    private void RemoveDeadBattlers()
    {
        //倒着写防止删除跳过元素。
        for (int i = allBattlers.Count - 1; i >=0 ; i--)
        {
            BattleEntities entity = allBattlers[i];

            if(entity.CurrHealth<=0)
            {

                if (entity.IsPlayer)
                {
                    playerBattlers.Remove(entity);
                }
                else
                {
                    enemyBattlers.Remove(entity);
                }
                allBattlers.RemoveAt(i);
            }
        }
    }

    private void CreatePartyEntities()
    {
        //1.获取当前的小队
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();
        Debug.Log("currentParty.Count = " + currentParty.Count);
        //2.为小队成员创建实体
        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            /*tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth,
            currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength, 
            currentParty[i].Level, currentParty[i].AttackDuration, true, currentParty[i].IsMelee);*/
            tempEntity.SetPlayerEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth,
            currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength, 
            currentParty[i].Level, currentParty[i].AttackDuration, true, 
            currentParty[i].SkillData, currentParty[i].NormalAttack, currentParty[i].BattleSkill);


            //1.根据预制体生成战斗视觉效果与技能监听者
            BattleVisual tempBattleVisual = Instantiate(currentParty[i].MemberBattleVisualPrefab,
            partySpawnPoints[i].position,Quaternion.identity).GetComponent<BattleVisual>();
            SkillCaster tempSkillCaster = tempBattleVisual.GetComponent<SkillCaster>();
            //2.设置视觉战斗效果的初值，把实体传递给技能监听者
            tempBattleVisual.SetStartingValues(currentParty[i].CurrHealth,currentParty[i].MaxHealth,currentParty[i].Level);
            tempSkillCaster.SetCasterBattleEntities(tempEntity);
            //3.存储视觉效果到战斗系统中
            tempEntity.BattleVisual = tempBattleVisual;

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetEnemies();
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            /*BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth,
            currentEnemies[i].MaxHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength,
            currentEnemies[i].Level, currentEnemies[i].AttackDuration,  false, currentEnemies[i].IsMelee);*/
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEnemyEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth,
            currentEnemies[i].MaxHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength,
            currentEnemies[i].Level, currentEnemies[i].AttackDuration,  false, currentEnemies[i].SkillData);
            //1.根据预制体生成战斗视觉效果与技能监听者
            BattleVisual tempBattleVisual = Instantiate(currentEnemies[i].EnemyVisualPrefab,
            enemySpawnPoints[i].position,Quaternion.identity).GetComponent<BattleVisual>();
            SkillCaster tempSkillCaster = tempBattleVisual.GetComponent<SkillCaster>();
            //2.设置视觉战斗效果的初值，把实体传递给技能监听者
            tempBattleVisual.SetStartingValues(currentEnemies[i].MaxHealth,currentEnemies[i].MaxHealth,currentEnemies[i].Level);
            tempSkillCaster.SetCasterBattleEntities(tempEntity);
            //3.存储视觉效果到战斗系统中
            tempEntity.BattleVisual = tempBattleVisual;

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    public void ShowBattleMenu()
    {
        while(currentPlayer < playerBattlers.Count &&
            playerBattlers[currentPlayer].CurrHealth <= 0)
        {
            currentPlayer++;
        }

        if(currentPlayer >= playerBattlers.Count)
        {
            StartCoroutine(BattleRoutine());
            return;
        }

        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    //而且这个函数是玩家点击普攻/战技后触发，也就是onclick事件调用的。

    //普攻
    public void OnNormalAttackButton()
    {
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];

        currentPlayerEntity.SkillData = currentPlayerEntity.NormalAttack;
        HandleSkillPointCost(currentPlayerEntity.SkillData);

        ShowObjectSelectionMenu();
    }

    //战技
    public void OnBattleSkillButton()
    {
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];

        currentPlayerEntity.SkillData = currentPlayerEntity.BattleSkill;
        Debug.Log("战技按钮被点");
        if(!HandleSkillPointCost(currentPlayerEntity.SkillData))
        {

            return;
        }

        ShowObjectSelectionMenu();
    }

    public void ShowObjectSelectionMenu()
    {
        battleMenu.SetActive(false);
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        Skill skill = currentPlayerEntity.SkillData;
        // 单体技能 → 需要选择目标
        if (skill.TargetMode == SkillTargetMode.Single)
        {
            if (skill.TargetCamp == SkillTargetCamp.Enemy)
            {
                SetEnemySelectionButtons();
            }
            else if (skill.TargetCamp == SkillTargetCamp.Ally)
            {
                SetAllySelectionButtons();
            }
            selectionMenu.SetActive(true);
        }
        // 群体技能 → 不需要选择目标
        //所以不会有点击SetEnemySelectionButtons触发函数SelectEnemy
        //所以下面这段是在对标SelectEnemy
        else if (skill.TargetMode == SkillTargetMode.Multi)
        {
            //currentPlayerEntity.Target = null;
            if(skill.skillSpType == SkillSpType.NormalAttack)
            {
                currentPlayerEntity.BattleAction = BattleEntities.Action.NormalAttack;
            }
            else
            {
                currentPlayerEntity.BattleAction = BattleEntities.Action.BattleSkill;
            }

            currentPlayer++;

            if (currentPlayer >= playerBattlers.Count)
            {
                StartCoroutine(BattleRoutine());
            }
            else
            {
                ShowBattleMenu();
            }
        }
    }



    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].SetActive(false);
        }
        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            selectionButtons[i].SetActive(true);
            selectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;
        }
    }

    private void SetAllySelectionButtons()
    {
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].SetActive(false);
        }
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            selectionButtons[i].SetActive(true);
            selectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerBattlers[i].Name;
        }
    }

    //这个是单体攻击玩家选择了目标后执行的函数。
    public void SelectObject(int currentObjectIndex)
    {

        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        Skill skill = currentPlayerEntity.SkillData;

        if (skill.TargetCamp == SkillTargetCamp.Enemy)
        {
            currentPlayerEntity.SetTarget(enemyBattlers[currentObjectIndex]);
            Debug.Log("玩家 "+currentPlayer+" 的目标被设定为：敌人"+enemyBattlers[currentObjectIndex].Name);
        }
        else if (skill.TargetCamp == SkillTargetCamp.Ally)
        {
            currentPlayerEntity.SetTarget(playerBattlers[currentObjectIndex]);
            Debug.Log("玩家 "+currentPlayer+" 的目标被设定为：我方"+playerBattlers[currentObjectIndex].Name);
        }

        if(skill.skillSpType == SkillSpType.NormalAttack)
        {
            currentPlayerEntity.BattleAction = BattleEntities.Action.NormalAttack;
        }
        else
        {
            currentPlayerEntity.BattleAction = BattleEntities.Action.BattleSkill;
        }
        //currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        
        currentPlayer++;
        if(currentPlayer>=playerBattlers.Count)
        {
            StartCoroutine(BattleRoutine());
        }
        else
        {
            selectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    private void ShowFloatingText(BattleEntities target, int power, bool isHeal)
    {
        Vector3 spawnPos = target.BattleVisual.transform.position;
        GameObject ft = Instantiate(floatingTextPrefab);
        // 加上符号
        string text = (isHeal ? "+" : "-") + Mathf.Abs(power).ToString();
        ft.GetComponent<FloatingText>().Initialize(
            spawnPos,
            text,
            isHeal ? Color.green : Color.red
        );
    }

    private IEnumerator AttackAction(BattleEntities currAttacker, 
        List<BattleEntities> currTargets)
    {
        Skill skill = currAttacker.SkillData;
        //1.攻击者视觉（获取攻击者初始位置）
        Transform attackerTf = currAttacker.BattleVisual.transform;
        Vector3 originPos = attackerTf.position;

        
        //2.计算攻击位置
        Vector3 attackPos = CalculateAttackPosition(currAttacker, currTargets);

        //3.处理移动（只和攻击方式有关）
        //近战就移动到对应位置，远程就省略这一步，直接不动。
        if(skill.AttackType == SkillAttackType.Melee)
        {
            yield return StartCoroutine(
                MoveToPosition(attackerTf, attackPos, 0.25f)
            );
        }
        //3 播放攻击动画
        currAttacker.BattleVisual.SetAttackTargets(currTargets);
        currAttacker.BattleVisual.PlayAttackAnimation();
        //新增：生成技能特效
        //SkillExecutor.SpawnSkillEffect(currAttacker.SkillData,targetTf);
        //取消：通过事件调用。
        // 等待攻击动画播完
        yield return new WaitForSeconds(currAttacker.AttackDuration);

        //4.计算+结算伤害
        ResolveSkillEffect(currAttacker, currTargets);
        SaveHealth();
        // 5.近战回去，远战不用
        if (currAttacker.SkillData.AttackType == SkillAttackType.Melee)
        {
            yield return StartCoroutine(
                MoveToPosition(attackerTf, originPos, 0.25f)
            );
        }
    }

    private IEnumerator MoveToPosition(Transform mover, Vector3 targetPos, float duration)
    {
        Vector3 startPos = mover.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            mover.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        mover.position = targetPos;
    }

    private BattleEntities GetRandomPartyMember()
    {
        List<BattleEntities> partyMenbers = new List<BattleEntities>();
        foreach(var battler in allBattlers)
        {
            if(battler.IsPlayer && battler.CurrHealth > 0)
            {
                partyMenbers.Add(battler);
            }
        }
        if(partyMenbers.Count == 0) return null;
        return partyMenbers[UnityEngine.Random.Range(0,partyMenbers.Count)];
    }

    //用于玩家1，2同时选择敌人时，第一个玩家杀死了敌人，第二个玩家没有目标可以攻击。
    private BattleEntities  GetRandomEnemy()
    {
        List<BattleEntities> enemies = new List<BattleEntities>();
        foreach(var battler in allBattlers)
        {
            if(!battler.IsPlayer && battler.CurrHealth > 0)
            {
                enemies.Add(battler);
            }
        }
        if(enemies.Count == 0) return null;
        return enemies[UnityEngine.Random.Range(0,enemies.Count)];
    }


    //用于获取一组点的中间值（群体攻击获取敌方或我方中心点）
    private Vector3 GetCenter(Transform[] points)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in points)
        sum += p.position;

        return sum / points.Length;
    }

    //施法者获取技能的目标
    private List<BattleEntities> GetTargets(BattleEntities attacker)
    {
        Skill skill = attacker.SkillData;
        List<BattleEntities> targets = new List<BattleEntities>();

        // 单体技能
        if (skill.TargetMode == SkillTargetMode.Single)
        {
            targets.Add(attacker.Target);
        }
        // 群体技能
        else if (skill.TargetMode == SkillTargetMode.Multi)
        {
            attacker.Target = null;
            if (skill.TargetCamp == SkillTargetCamp.Enemy)
            {
                if (attacker.IsPlayer)
                    targets.AddRange(enemyBattlers);
                else
                    targets.AddRange(playerBattlers);
            }
            else if (skill.TargetCamp == SkillTargetCamp.Ally)
            {
                if (attacker.IsPlayer)
                    targets.AddRange(playerBattlers);
                else
                    targets.AddRange(enemyBattlers);
            }
        }

        return targets;
    }

    private Vector3 CalculateAttackPosition(BattleEntities attacker, 
        List<BattleEntities> targets)
    {
        Skill skill = attacker.SkillData;

        Transform attackerTf = attacker.BattleVisual.transform;
        Vector3 originPos = attackerTf.position;
        // 单体
        if(skill.TargetMode == SkillTargetMode.Single)
        {
            Transform targetTf = targets[0].BattleVisual.transform;

            return targetTf.position +
                (originPos - targetTf.position).normalized * 0.5f;
        }

        // 群体
        else
        {
            Vector3 center = originPos;   
            if(skill.TargetCamp == SkillTargetCamp.Enemy)
            {
                center = GetCenter(enemySpawnPoints);
            }
            else if(skill.TargetCamp == SkillTargetCamp.Ally)
            {
                center = GetCenter(partySpawnPoints);
            }

            return center + (originPos - center).normalized * 0.5f;
        }

    }


    //结算技能效果
    private void ResolveSkillEffect(BattleEntities attacker,
        List<BattleEntities> targets)
    {
        Skill skill = attacker.SkillData;

        foreach(var target in targets)
        {
            switch (skill.EffectType)
            {
                case SkillEffectType.Damage:
                    target.TakeDamage(skill.Power);
                    bottomText.text = 
                    $"{attacker.Name} 对 {target.Name} 造成了 {skill.Power} 点伤害";
                    break;

                case SkillEffectType.Heal:
                    target.Heal(skill.Power);
                    BattleStateEffectManager.Instance
                        .PlayStateEffect(StateEffectType.Health, target.BattleVisual.transform);
                    //绿色+号向上
                    bottomText.text = 
                    $"{attacker.Name} 为 {target.Name} 回了 {skill.Power} 点血";
                    break;
            }
            ShowFloatingText(target, skill.Power, skill.EffectType == SkillEffectType.Heal);
        }
    }


    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.Initiative.CompareTo(bi2.Initiative));

    }

    public void SelectRunAction()
    {
        state = BattleState.Selection;
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.BattleAction = BattleEntities.Action.Run;
        currentPlayer++;
        if(currentPlayer>=playerBattlers.Count)
        {
            StartCoroutine(BattleRoutine());
        }
    }


    //战绩点相关函数
    //消耗战绩点，输入使用战绩点数量，输出是否可以消耗
    public bool ConsumeSkillPoint(int amount)
    {
        if (skillPoints < amount)
            return false;

        skillPoints -= amount;
        UpdateSkillPointUI();
        return true;
    }


    //恢复战绩点，输入恢复战绩点数量
    public void AddSkillPoint(int amount)
    {
        skillPoints = Mathf.Min(skillPoints + amount, maxSkillPoints);
        UpdateSkillPointUI();
    }

    //更新战绩点
    private void UpdateSkillPointUI()
    {
        skillPointText.text = "战技点: " + skillPoints + "/5";
    }

    private bool HandleSkillPointCost(Skill skill)
    {
        if (skill.skillSpType == SkillSpType.BattleSkill)
        {
            if (!ConsumeSkillPoint(skill.skillPointCost))
            {
                bottomText.text = "战技点不足！";
                return false;
            }
        }

        if (skill.skillSpType == SkillSpType.NormalAttack)
        {
            AddSkillPoint(1);
        }

        return true;
    }

}

[System.Serializable]
public class BattleEntities
{
    public enum Action{NormalAttack,BattleSkill,Run};
    public Action BattleAction;
    //这些是敌人实体和小队成员实体所具有的公共属性
    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;//用于判断实体为小队玩家还是敌人
    //public bool IsMelee;//用于判断实体是否近战


    //挂载战技和普攻的地方
    public Skill NormalAttack;
    public Skill BattleSkill;
    public Skill SkillData;//当前选择的技能

    public BattleEntities Target;//当前实体释放单体技能时的瞄准目标，会与玩家交互，可以为敌人或队友或自己
    public float AttackDuration;//攻击动画持续时长

    //存储视觉效果
    public BattleVisual BattleVisual;

    /*/public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, 
    int strength, int level, float attackDuration,bool isPlayer, bool isMelee)*/
    public void SetPlayerEntityValues(string name, int currHealth, int maxHealth, int initiative, 
    int strength, int level, float attackDuration,bool isPlayer, 
    Skill skillData, Skill normalAttack, Skill battleSkill)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        AttackDuration = attackDuration;
        IsPlayer = isPlayer;
        //IsMelee = isMelee;
        SkillData =  skillData;
        NormalAttack = normalAttack;
        BattleSkill = battleSkill;
    }

    public void SetEnemyEntityValues(string name, int currHealth, int maxHealth, int initiative, 
    int strength, int level, float attackDuration,bool isPlayer, 
    Skill skillData)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        AttackDuration = attackDuration;
        IsPlayer = isPlayer;
        //IsMelee = isMelee;
        SkillData =  skillData;
    }

    public void SetTarget(BattleEntities target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisual.ChangeHealth(CurrHealth);
    }

    public void TakeDamage(int DamagePower)
    {
        CurrHealth -= DamagePower;
        BattleVisual.PlayHitAnimation();
        UpdateUI();

            
    }
    public void Heal(int HealPower)
    {
        CurrHealth += HealPower;
        if(CurrHealth>MaxHealth) CurrHealth = MaxHealth;
        UpdateUI();
    }
}
