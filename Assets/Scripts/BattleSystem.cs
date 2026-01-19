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
    [SerializeField] private Transform[] partySpwanPoints;
    [SerializeField] private Transform[] enemySpwanPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();


    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer = 0;

    private const string ACTION_MESSAGE = "的回合";
    private const string WIN_MESSAGE = "梦境消散了，愿我们在清醒的世界再会！";
    private const string LOST_MESSAGE = "梦魇吞噬了你。。。";
    private const string RUN_SUCCESS_MESSAGE = "如有神助，你逃离了梦境！";
    private const string RUN_FAILED_MESSAGE = "深呼吸，头晕是正常的，你无法逃脱！";
    private const string OVERWORLDSCENE = "OverworldScene";

    private const string BATTLESCENE_PROMT = "光怪陆离，意识陷于绮丽的梦境~\n" + 
                                              "梦境中的异兽并不欢迎清醒之人\n" + 
                                              "请活下去！！！";
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
        //ShowBattleMenu();
        //bottomTextPopUp.SetActive(true);
        //bottomText.text = BATTLESCENE_PROMT;
        //AttackAction(allBattlers[0],allBattlers[1]);
        //ShowEnemySelectionMenu();
    }

    private IEnumerator ShowBattleMenuAfterDelay()
    {
        // 1. 先显示提示词
        bottomTextPopUp.SetActive(true);
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
        state = BattleState.Battle;
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(state == BattleState.Battle && allBattlers[i].CurrHealth >0)
            {
                switch (allBattlers[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
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

        if(state==BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
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
            if(allBattlers[currentAttacker.Target].CurrHealth <= 0)
            {
                currentAttacker.SetTarget(GetRandomEnemy());
                Debug.Log("尸体在说话，触发了随机选择");
            }
            BattleEntities currentTarget = allBattlers[currentAttacker.Target];
            yield return StartCoroutine(AttackAction(currentAttacker,currentTarget));
            yield return new WaitForSeconds(TURN_DURATION);
            if(currentTarget.CurrHealth<=0)
            {
                bottomText.text = string.Format("{0} 击败了 {1}", currentAttacker.Name, currentTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyBattlers.Remove(currentTarget);
                if(enemyBattlers.Count <= 0)
                {
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    SceneManager.LoadScene(OVERWORLDSCENE);
                }
            }
            //bottomText.text = "玩家回合结束";
        }
        if(allBattlers[i].IsPlayer == false)
        {
            BattleEntities currentAttacker = allBattlers[i];
            currentAttacker.SetTarget(GetRandomPartyMember());
            BattleEntities currentTarget = allBattlers[currentAttacker.Target];
            yield return StartCoroutine(AttackAction(currentAttacker,currentTarget));
            yield return new WaitForSeconds(TURN_DURATION);
            if(currentTarget.CurrHealth<=0)
            {
                bottomText.text = string.Format("{0} 击败了 {1}", currentAttacker.Name, currentTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerBattlers.Remove(currentTarget);
                if(playerBattlers.Count <= 0)
                {
                    state = BattleState.Lost;
                    bottomText.text = LOST_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("很遗憾，前方的区域以后再来探索吧！");
                }
            }
            //bottomText.text = "敌人回合结束";
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

    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(allBattlers[i].CurrHealth<=0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }

    private void CreatePartyEntities()
    {
        //1.获取当前的小队
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();
        //2.为小队成员创建实体
        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth,
            currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength, 
            currentParty[i].Level, currentParty[i].AttackDuration, true, currentParty[i].IsMelee);
            
            //1.根据预制体生成战斗视觉效果
            BattleVisual tempBattleVisual = Instantiate(currentParty[i].MemberBattleVisualPrefab,
            partySpwanPoints[i].position,Quaternion.identity).GetComponent<BattleVisual>();
            //2.设置视觉战斗效果的初值
            tempBattleVisual.SetStartingValues(currentParty[i].CurrHealth,currentParty[i].MaxHealth,currentParty[i].Level);
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
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth,
            currentEnemies[i].MaxHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength,
            currentEnemies[i].Level, currentEnemies[i].AttackDuration,  false, currentEnemies[i].IsMelee);

            //1.根据预制体生成战斗视觉效果
            BattleVisual tempBattleVisual = Instantiate(currentEnemies[i].EnemyVisualPrefab,
            enemySpwanPoints[i].position,Quaternion.identity).GetComponent<BattleVisual>();
            //2.设置视觉战斗效果的初值
            tempBattleVisual.SetStartingValues(currentEnemies[i].MaxHealth,currentEnemies[i].MaxHealth,currentEnemies[i].Level);
            //3.存储视觉效果到战斗系统中
            tempEntity.BattleVisual = tempBattleVisual;

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        selectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }
        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            enemySelectionButtons[i].SetActive(true);
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;
        }
    }

    public void SelectEnemy(int currentEnemy)
    {
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));
        Debug.Log("玩家 "+currentPlayer+" 的敌人被设定为：敌人"+allBattlers.IndexOf(enemyBattlers[currentEnemy]));
        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        currentPlayer++;
        if(currentPlayer>=playerBattlers.Count)
        {
            //Debug.Log("开始战斗！！！");
            StartCoroutine(BattleRoutine());
            /*for (int i = 0; i < playerBattlers.Count; i++)
            {
                int tempCurrentPlayer = allBattlers.IndexOf(playerBattlers[i]);
                int tempCurrentEnemy = allBattlers[tempCurrentPlayer].Target;
                AttackAction(allBattlers[tempCurrentPlayer],allBattlers[tempCurrentEnemy]);
            }*/
        }
        else
        {
            selectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    private IEnumerator AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
    {
        int damage = currAttacker.Strength;
        //获取攻击者和敌人的位置
        Transform attackerTf = currAttacker.BattleVisual.transform;
        Transform targetTf = currTarget.BattleVisual.transform;
        //记录原始位置与攻击位置
        Vector3 originPos = attackerTf.position;
        Vector3 attackPos = targetTf.position + (originPos - targetTf.position).normalized * 0.5f;
        //目标位置传递
        currAttacker.BattleVisual.SetAttackTarget(targetTf);
        //if近战，currAttacker.position = currTarget.position
        //远程就站在原地放技能
        if(currAttacker.IsMelee)
        {
            yield return StartCoroutine(MoveToPosition(attackerTf,attackPos,0.25f));
        }
        currAttacker.BattleVisual.PlayAttackAnimation();
        // 等待攻击动画播完
        yield return new WaitForSeconds(currAttacker.AttackDuration);
        currTarget.CurrHealth -= damage;
        currTarget.BattleVisual.PlayHitAnimation();
        //近战回去，远战不用
        currTarget.UpdateUI();
        bottomText.text = string.Format("{0} 对 {1} 造成了 {2} 点伤害", currAttacker.Name, currTarget.Name, damage);
        SaveHealth();
        // 8. 回到原位
        if (currAttacker.IsMelee)
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

    private int GetRandomPartyMember()
    {
        List<int> partyMenbers = new List<int>();
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(allBattlers[i].IsPlayer == true)
            {
                partyMenbers.Add(i);
            }
        }
        return partyMenbers[UnityEngine.Random.Range(0,partyMenbers.Count)];
    }

    //用于玩家1，2同时选择敌人时，第一个玩家杀死了敌人，第二个玩家没有目标可以攻击。
    private int GetRandomEnemy()
    {
        List<int> enemys = new List<int>();
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(allBattlers[i].IsPlayer == false)
            {
                enemys.Add(i);
            }
        }
        return enemys[UnityEngine.Random.Range(0,enemys.Count)];
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
}

[System.Serializable]
public class BattleEntities
{
    public enum Action{Attack,Run};
    public Action BattleAction;
    //这些是敌人实体和小队成员实体所具有的公共属性
    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;//用于判断实体为小队玩家还是敌人
    public bool IsMelee;//用于判断实体是否近战
    public int Target;//当前实体的瞄准目标，可以为敌人或队友或自己
    public float AttackDuration;//攻击动画持续时长

    //存储视觉效果
    public BattleVisual BattleVisual;

    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, float attackDuration,bool isPlayer, bool isMelee)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        AttackDuration = attackDuration;
        IsPlayer = isPlayer;
        IsMelee = isMelee;
    }

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisual.ChangeHealth(CurrHealth);
    }
}
