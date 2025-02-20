using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private readonly float moveSpeed = 50f;
    private readonly float fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color originalColor;
    private RectTransform rectTransform;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        if (textMesh == null)
        {
            Debug.LogError("No se encontró TextMeshProUGUI en el objeto de texto de daño.");
            return;
        }

        originalColor = textMesh.color;

    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        float alpha = Mathf.Lerp(originalColor.a, 0, Time.deltaTime / fadeDuration);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    public void SetDamageText(float damage)
    {
        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
        }
    }
}
