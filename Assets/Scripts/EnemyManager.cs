using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnenies;
    //[SerializeField] private EnemyInfo defaultEneny;

    //常量：用来设置等级修改器,就是一个系数，用来根据等级修改怪物战力
    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        GenerateEnemyByName("Slime", 1);
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

                currentEnenies.Add(newEnemy);
            }
        }
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
    public GameObject EnemyVisualPrefab;
}
