using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropType
{
    Hp,
    Speed,
    Bomb,
    Range,
    Time
}

[System.Serializable]
public class PropType_Sprite
{
    public PropType type;
    public Sprite sp;
}

public class Prop : MonoBehaviour
{
    public PropType_Sprite[] propType_Sprites;
    private SpriteRenderer spriteRenderer;
    private PropType propType;
    private Sprite defultSp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defultSp = spriteRenderer.sprite;

    }
    /// <summary>
    /// 重置道具
    /// </summary>
    private void ResetProp()
    {
        tag = "Wall";
        gameObject.layer = 6;
        spriteRenderer.sprite = defultSp;
        GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //碰到炸弹
        if (collision.CompareTag(Tags.BombEffect))
        {
            //更改Prop的Tag和layer防止与Enemy冲突
            tag = "Untagged";
            gameObject.layer = 0;

            GetComponent<Collider2D>().isTrigger = true;
            //随机道具
            int index = Random.Range(0, propType_Sprites.Length);
            //更换贴图
            spriteRenderer.sprite = propType_Sprites[index].sp;
            propType = propType_Sprites[index].type;
            StartCoroutine(PropAni());
        }

        //碰到人物，获取相应增益
        if (collision.CompareTag(Tags.Player))
        {
            //获取PlayerConTroll脚本   类名
            PlayerCtrl playerCtrl = collision.gameObject.GetComponent<PlayerCtrl>(); 
           
            switch (propType)
            {
                case PropType.Hp:
                    playerCtrl.HP++;
                    break;
                case PropType.Speed:
                    playerCtrl.AddSpeed();
                    break;
                case PropType.Bomb:
                    playerCtrl.bombCount++;
                    break;
                case PropType.Range:
                    playerCtrl.range++;
                    break;
                case PropType.Time:
                    GameController.Instance.time += 30;
                    break;
                default:
                    break;
            }
            ResetProp();
            ObjectPool.Instance.Add(ObjectType.Prop, gameObject);
        }
    }

    IEnumerator PropAni()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
