using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

}

[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
}
