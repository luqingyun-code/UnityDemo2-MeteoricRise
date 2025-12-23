using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    //公共常量
    public string EnemyName;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseInitiative;
    public GameObject EnemyVisualPrefab;//战斗场景中
}
