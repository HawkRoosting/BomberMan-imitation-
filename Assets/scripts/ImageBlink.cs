using UnityEngine;
using UnityEngine.UI;

public class ImageBlink : MonoBehaviour
{
    public float blinkInterval = 0.5f; // ��˸�ļ��ʱ��

    private Image image;
    private float timer;
    private bool isVisible;

    private void Start()
    {
        image = GetComponent<Image>();
        timer = 0f;
        isVisible = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // �ж��Ƿ�ﵽ��˸���ʱ��
        if (timer >= blinkInterval)
        {
            timer = 0f;
            isVisible = !isVisible;

            // ����ͼ��Ŀɼ���
            image.enabled = isVisible;
        }
    }
}
