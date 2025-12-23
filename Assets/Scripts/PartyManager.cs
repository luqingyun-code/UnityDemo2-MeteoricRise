using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{

    [SerializeField] private PartyMemberInfo[] allMember;
    [SerializeField] private List<PartyMember> currentParty;


    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private void Awake()
    {
        AddMemberToPartyByName(defaultPartyMember.MemberName);
    }

    //通过名字给函数添加成员
    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMember.Length; i++)
        {
            if(allMember[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMember[i].MemberName;
                newPartyMember.Level = allMember[i].StartingLevel;
                newPartyMember.CurrHealth = allMember[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMember[i].BaseStrength;
                newPartyMember.Initiative = allMember[i].BaseInitiative;
                newPartyMember.MemberBattleVisualPrefab = allMember[i].MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = allMember[i].MemberOverworldVisualPrefab;

                currentParty.Add(newPartyMember);
            }
        }
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
    //战斗可视
    public GameObject MemberBattleVisualPrefab;
    //过场可视
    public GameObject MemberOverworldVisualPrefab;

}
