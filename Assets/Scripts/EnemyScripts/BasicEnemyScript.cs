using System.Collections;
using TMPro;
using UnityEngine;

public class BasicEnemyScript : AbstractEnemy
{

    private float lifePoints = 1.5f;
    private int attackDamage = 1;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] GameObject damageTextPrefab;

    private readonly float moveSpeed = 50f;
    private readonly float fadeDuration = 1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Busca al jugador en la escena
    }

    void Update()
    {
        animation.transform.rotation = Quaternion.identity;
    }

    public override void attack()
    {
        attackHitbox.SetActive(true);
    }

    public override void reciveDmg(float damage)
    {
        lifePoints = lifePoints - damage;
        //ShowDamageText(damage);
        if (lifePoints < 0)
        {
            Destroy(gameObject);
        }
    }

    private void ShowDamageText(float damage)
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        if (canvas == null)
        {
            Debug.LogError("No Canvas found with tag 'UI'. Make sure your Canvas is tagged correctly.");
            return;
        }

        GameObject damageTextInstance = Instantiate(damageTextPrefab, canvas.transform);

        TMPro.TMP_Text damageText = damageTextInstance.GetComponent<TMPro.TMP_Text>();
        if (damageText != null)
        {
            damageText.text = damage.ToString();
            StartCoroutine(damageFollow(damageTextInstance, damageText));
        }
        else
        {
            Debug.LogError("No TMP_Text component found on damageTextPrefab");
        }

        Destroy(damageTextInstance, 1.5f);
    }

    private IEnumerator damageFollow(GameObject damageTextInstance, TMPro.TMP_Text damageText)
    {
        while (damageTextInstance != null) 
        {
            if (damageTextInstance.GetComponent<RectTransform>() != null)
            {
                damageTextInstance.transform.position = transform.position;


                Vector2 finalPosition = transform.position;
                finalPosition += Vector2.up * moveSpeed * Time.deltaTime;

                Color originalColor = damageText.color;

                float alpha = Mathf.Lerp(originalColor.a, 0, Time.deltaTime / fadeDuration);
                damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            yield return null; 
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
