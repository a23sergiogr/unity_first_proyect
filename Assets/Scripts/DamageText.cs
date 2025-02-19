using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color originalColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalColor = textMesh.color;
        Destroy(gameObject, fadeDuration); // Destruye el objeto después de que termine la animación
    }

    void Update()
    {
        // Mueve el texto hacia arriba
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Calcula el nuevo color con transparencia
        float alpha = Mathf.Lerp(originalColor.a, 0, Time.deltaTime / fadeDuration);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    public void SetDamageText(float damage)
    {
        textMesh.text = damage.ToString();
    }
}
