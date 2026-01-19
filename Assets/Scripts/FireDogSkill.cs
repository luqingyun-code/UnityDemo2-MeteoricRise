using UnityEngine;

public class FireDogSkill : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    //[SerializeField] private Transform firePoint;

    private BattleVisual battleVisual;
    private GameObject currentFireball;

    void Awake()
    {
        battleVisual = GetComponent<BattleVisual>();
        battleVisual.OnAttackEvent += CastFireball;
    }

    void OnDestroy()
    {
        if (battleVisual != null)
            battleVisual.OnAttackEvent -= CastFireball;
    }

    private void CastFireball()
    {
        Transform target = battleVisual.GetAttackTarget();
        if(target == null) return;
        Vector3 startPosition = transform.position + Vector3.up * 2;
        currentFireball = Instantiate(fireballPrefab,startPosition,Quaternion.identity);
        currentFireball.GetComponent<FireballController>().SetTarget(target);
        Animator anim = currentFireball.GetComponent<Animator>();
        anim.Play("Boom", 0, 0);  // 从头播放动画
    }
}
