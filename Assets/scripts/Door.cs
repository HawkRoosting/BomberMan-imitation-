using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //获取通关门贴图
    public Sprite doorSprite;//defultSp为物体初始设置，物体从对象池中取出后会带有上一关卡特征故需要重置
    private Sprite defultSp;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //暂存默认图
        defultSp = spriteRenderer.sprite;
    }


    public void ResetDoor()
    {
        tag = "Wall";
        gameObject.layer = 6;
        spriteRenderer.sprite = defultSp;
        GetComponent<Collider2D>().isTrigger = false;
    }

    //当炸弹炸毁门表面墙壁出现通关门实现思路：当检测到BombEffect时对贴图进行更换
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            tag = "Untagged";
            gameObject.layer = 0;
            //更换贴图
            spriteRenderer.sprite = doorSprite;
            //更换之后通关门是可通过的所以需要将Door的istrigger属性勾选
            GetComponent<Collider2D>().isTrigger = true;

            //此时出现人物穿过门时处于门的贴图后面：将spriteRenderer中order in Layer层级调高
        }
        //通关条件：敌人全部消灭，player与door产生碰撞
        if (collision.CompareTag(Tags.Player))
        {
            GameController.Instance.LoadNextLevel();
        }
    }
}
