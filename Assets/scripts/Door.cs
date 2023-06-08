using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //��ȡͨ������ͼ
    public Sprite doorSprite;//defultSpΪ�����ʼ���ã�����Ӷ������ȡ����������һ�ؿ���������Ҫ����
    private Sprite defultSp;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //�ݴ�Ĭ��ͼ
        defultSp = spriteRenderer.sprite;
    }


    public void ResetDoor()
    {
        tag = "Wall";
        gameObject.layer = 6;
        spriteRenderer.sprite = defultSp;
        GetComponent<Collider2D>().isTrigger = false;
    }

    //��ը��ը���ű���ǽ�ڳ���ͨ����ʵ��˼·������⵽BombEffectʱ����ͼ���и���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            tag = "Untagged";
            gameObject.layer = 0;
            //������ͼ
            spriteRenderer.sprite = doorSprite;
            //����֮��ͨ�����ǿ�ͨ����������Ҫ��Door��istrigger���Թ�ѡ
            GetComponent<Collider2D>().isTrigger = true;

            //��ʱ�������ﴩ����ʱ�����ŵ���ͼ���棺��spriteRenderer��order in Layer�㼶����
        }
        //ͨ������������ȫ������player��door������ײ
        if (collision.CompareTag(Tags.Player))
        {
            GameController.Instance.LoadNextLevel();
        }
    }
}
