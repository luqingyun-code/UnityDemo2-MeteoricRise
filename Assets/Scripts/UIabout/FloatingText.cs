using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f; // 上升速度
    public float duration = 1f;   // 显示时间
    public Vector3 offset = new Vector3(0, 2, 0); // 从角色头顶偏移

    private TextMeshProUGUI textMesh;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        // 渐隐
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = Mathf.Lerp(1f, 0.3f, timer / duration); // 随时间从不透明到完全透明
            textMesh.color = c;
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 startPos, string text, Color color)
    {
        transform.position = startPos + offset;
        SetText(text, color);
        timer = 0;
    }
}
