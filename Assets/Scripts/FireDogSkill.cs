using UnityEngine;

public class FireDogSkill : MonoBehaviour
{
    [SerializeField] private GameObject fireballObject;
    //[SerializeField] private Transform firePoint;

    private BattleVisual battleVisual;

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
        fireballObject.SetActive(true);
        Animator anim = fireballObject.GetComponent<Animator>();
        anim.Play("FireballExplode", 0, 0);  // 从头播放动画
    }

    public void HideFireball()
    {
        fireballObject.SetActive(false);
    }
}
