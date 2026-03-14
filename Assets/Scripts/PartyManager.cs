using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{

    [SerializeField] private PartyMemberInfo[] allMember;
    [SerializeField] private List<PartyMember> currentParty;


    
    //[SerializeField] private PartyMemberInfo defaultPartyMember;
    [SerializeField] private PartyMemberInfo PartyMember1;
    [SerializeField] private PartyMemberInfo PartyMember2;
    [SerializeField] private PartyMemberInfo partyMember3;
    private Vector3 playerPosition;

    public static GameObject instance;
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
            AddMemberToPartyByName(PartyMember1.MemberName);
            AddMemberToPartyByName(PartyMember2.MemberName);
            AddMemberToPartyByName(partyMember3.MemberName);
            //AddMemberToPartyByName(defaultPartyMember.MemberName);
        }
        DontDestroyOnLoad(gameObject);
    }

    //通过名字给小队成员列表添加成员
    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMember.Length; i++)
        {
            if(allMember[i].MemberName == memberName)
            {
                //Debug.Log("add");
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMember[i].MemberName;
                newPartyMember.Level = allMember[i].StartingLevel;
                newPartyMember.CurrHealth = allMember[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMember[i].BaseStrength;
                newPartyMember.Initiative = allMember[i].BaseInitiative;
                newPartyMember.MemberBattleVisualPrefab = allMember[i].MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = allMember[i].MemberOverworldVisualPrefab;
                newPartyMember.AttackDuration = allMember[i].AttackDuration;
                //newPartyMember.IsMelee = allMember[i].IsMelee;
                newPartyMember.SkillData = allMember[i].SkillData;
                newPartyMember.NormalAttack = allMember[i].NormalAttack;
                newPartyMember.BattleSkill = allMember[i].BattleSkill;
                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        List<PartyMember> aliveParty = new List<PartyMember>();
        aliveParty = currentParty;
        for (int i = 0; i < aliveParty.Count; i++)
        {
            if(aliveParty[i].CurrHealth<=0)
            {
                aliveParty.RemoveAt(i);
            }
        }
        return aliveParty;
    }


    public void SaveHealth(int partyMember, int health)
    {
        currentParty[partyMember].CurrHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }  

    public Vector3 GetPosition()
    {
        return playerPosition;
    }    
}


//设置系统可见，否则上文的List<PartyMember> currentParty在unity中不可见
[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    //当前经验值
    public int CurrExp;
    //最大经验值
    public int MaxExp;

    public float AttackDuration;

    //public bool IsMelee;
    public Skill NormalAttack;
    public Skill BattleSkill;
    public Skill SkillData;

    //战斗可视
    public GameObject MemberBattleVisualPrefab;
    //过场可视
    public GameObject MemberOverworldVisualPrefab;

}
