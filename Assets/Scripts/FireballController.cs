using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    public void DeleteSelf()
    {
        Destroy(gameObject);
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position;
        //火球中心和目标重合了，所以有一半在土里，上移0.5f
        targetPosition += Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

}
