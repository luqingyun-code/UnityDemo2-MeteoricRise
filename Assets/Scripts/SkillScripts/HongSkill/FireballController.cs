using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*FireballController脚本负责：
1.火球怎么飞
2.什么时候销毁
3.碰撞
4.移动
*/


public class FireballController : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    
    //这删除函数被动画调用了嗷，可不是没人要的函数，也不是凭空删除
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
