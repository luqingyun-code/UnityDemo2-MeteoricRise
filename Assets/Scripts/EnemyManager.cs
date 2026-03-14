using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnenies;
    [SerializeField] private EnemyInfo defaultEneny;

    private static GameObject instance;
    //常量：用来设置等级修改器,就是一个系数，用来根据等级修改怪物战力
    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateEnemyByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnenies.Clear();
        int temNumEnemies = UnityEngine.Random.Range(1, maxNumEnemies+1);
        for (int i = 0; i < temNumEnemies; i++)
        {
            Encounter temEncounter = encounters[Random.Range(0, encounters.Length)];
            int level = Random.Range(temEncounter.LevelMin,temEncounter.LevelMax+1);
            GenerateEnemyByName(temEncounter.Enemy.EnemyName, level);
        }
    }

    public void GenerateEnemyByName(string enemyName, int level)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].EnemyName == enemyName)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.EnemyName = allEnemies[i].EnemyName;
                newEnemy.Level = level;
                //创建等级修改器应用到敌人所有的每一个属性上
                float levelModifier = (LEVEL_MODIFIER * newEnemy.Level);
                newEnemy.MaxHealth = Mathf.RoundToInt(allEnemies[i].BaseHealth + (allEnemies[i].BaseHealth * levelModifier));
                newEnemy.CurrHealth = newEnemy.MaxHealth;
                newEnemy.Strength = Mathf.RoundToInt(allEnemies[i].BaseStrength + (allEnemies[i].BaseStrength * levelModifier));
                newEnemy.Initiative = Mathf.RoundToInt(allEnemies[i].BaseInitiative + (allEnemies[i].BaseInitiative * levelModifier));
                newEnemy.EnemyVisualPrefab = allEnemies[i].EnemyVisualPrefab;
                newEnemy.AttackDuration = allEnemies[i].AttackDuration;
                //newEnemy.IsMelee = allEnemies[i].IsMelee;
                newEnemy.SkillData = allEnemies[i].SkillData;
                currentEnenies.Add(newEnemy);
            }
        }
    }
    public List<Enemy> GetEnemies()
    {
        return currentEnenies;
    }

}

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int MaxHealth;
    public int CurrHealth;
    public int Initiative;
    public int Strength;
    //public bool IsMelee;
    public Skill SkillData;
    public float AttackDuration;
    public GameObject EnemyVisualPrefab;
}
