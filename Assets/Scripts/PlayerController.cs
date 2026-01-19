using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;
    private PartyManager partyManager;
    

    
    private const string IS_WALK_PARAM = "IsWalk";//正在行走的动画
    private const string BATTLE_SCENE = "BattleScene";//战斗场景的名字
    private const float TIME_PER_STEP = 0.5f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsToNextEncounter();//生成一些随机数用来遭遇
    }


    private void OnEnable()//执行频率为脚本启用次数
    {
        playerControls.Enable();//控制输入系统的开关，关闭是Disable()
    }

    //Strat发生在Awake和OnEnable下面
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if(partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;//获取角色x的值，【-1，1】，用于左右方向,L-R+
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;//获取角色y的值，【-1，1】，用于前后方向,W+S-

        //Debug.Log(x + "," + z);

        movement = new Vector3(x, 0, z).normalized;

        anim.SetBool(IS_WALK_PARAM, movement!=Vector3.zero);

        //向左走
        if(x!=0 && x<0)
        {
            playerSprite.flipX = true;
        }
        //向右走
        if(x!=0 && x>0)
        {
            playerSprite.flipX = false;
        }
    }

    // 刚体移动一般会在固定更新中实现
    private void FixedUpdate()
    {
        //角色移动时添加code
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);


        //设置草触发器时添加code
        Collider[] colliders = Physics.OverlapSphere(transform.position,1,grassLayer);
        //transform.position是球中心，1是半径，grassLayer是待检测的层，Collider数组用于存多个碰撞体
        movingInGrass = movement!=Vector3.zero && colliders.Length != 0;

        if(movingInGrass == true)
        {
            stepTimer += Time.fixedDeltaTime;
            if(stepTimer>TIME_PER_STEP)
            {
                stepTimer = 0;
                stepsInGrass++;

                //TODO：场景转换
                if(stepsInGrass>=stepsToEncounter)
                {
                    partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }

        }
    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter,maxStepsToEncounter);
    }



}
