using UnityEngine;
using UnityEngine.UI;

public class ImageBlink : MonoBehaviour
{
    public float blinkInterval = 0.5f; // 闪烁的间隔时间

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

        // 判断是否达到闪烁间隔时间
        if (timer >= blinkInterval)
        {
            timer = 0f;
            isVisible = !isVisible;

            // 设置图像的可见性
            image.enabled = isVisible;
        }
    }
}
