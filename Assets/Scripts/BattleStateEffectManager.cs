using UnityEngine;

public enum StateEffectType
{
    Health, //治疗效果
    Buff, //增益
    Debuff, //减益
    Dizzy, //眩晕
}
public class BattleStateEffectManager : MonoBehaviour
{
    public static BattleStateEffectManager Instance;

    public GameObject healthStateEffectPrefab;
    public GameObject buffStateEffectPrefabt;
    public GameObject debuffStateEffectPrefab;
    public GameObject dizzyStateEffectPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayStateEffect(StateEffectType type, Transform target)
    {
        GameObject prefab = null;

         switch(type)
        {
            case StateEffectType.Health:
                prefab = healthStateEffectPrefab;
                break;

            case StateEffectType.Buff:
                prefab = buffStateEffectPrefabt;
                break;

            case StateEffectType.Debuff:
                prefab = debuffStateEffectPrefab;
                break;

            case StateEffectType.Dizzy:
                prefab = dizzyStateEffectPrefab;
                break;    
        }

        if (prefab != null)
        {
            Vector3 pos = target.position + Vector3.up;
            GameObject effect = Instantiate(prefab, pos, Quaternion.identity);
            Destroy(effect, 3f);
        }
    }
}
