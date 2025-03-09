using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    [SerializeField] private Weapon[] equippedWeapon;
    private int equippedWeaponIndex = 0;
    [SerializeField] private new GameObject animation;
    [SerializeField] private List<Item> equippedItems = new List<Item>();
    private float _speed = 4f;
    public float speed
    {
        get { return _speed; }
        set
        {
            // Si el valor es mayor a 30, ajusta a 30
            _speed = Mathf.Min(30f, value); 
        }
    }

    /*Dash*/
    private float dashDistance = 4f;
    private float _dashCooldown = 2f;
    public float dashCooldown
    {
        get { return _dashCooldown; }
        set
        {
            // Si el valor es menor a 1, ajusta a 1
            _dashCooldown = Mathf.Max(0.5f, value);
        }
    }
    private float nextDashTime = 0f;
    private bool isDashing = false;
    private float dashDuration = 0.12f;


    /* Health and Mana Bars */
    private int maxNumberOfHearts = 20;
    private int currentMaxNumberOfHearts = 5;
    private int currentNumberOfHearts;
    private float invulnerabilityTime = 0.75f;
    private bool isInvulnerable = false;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject emptyHeartPrefab;
    [SerializeField] private Transform healthBarParent;
    //private float heartSpacing = 150f; 
    private float heartSpacing = 75f; 

    public void InitializeHealth()
    {
        currentNumberOfHearts = currentMaxNumberOfHearts;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage) // Cambiado a int
    {
        if (isInvulnerable)
        {
            return;
        }

        currentNumberOfHearts -= damage;
        if (currentNumberOfHearts <= 0)
        {
            currentNumberOfHearts = 0; // Asegura que no sea negativo
            UpdateHealthBar();
            GameManager.Instance.PlayerDeath();
            Destroy(gameObject);
            return;
        }

        StartCoroutine(Invulnerability());
        UpdateHealthBar();
    }

    IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        GetComponent<CircleCollider2D>().enabled = false;
        SpriteRenderer sr = GameObject.FindGameObjectWithTag("sprite").GetComponent<SpriteRenderer>();
        sr.enabled = false;
        yield return new WaitForSeconds(invulnerabilityTime/4);
        sr.enabled = true;
        yield return new WaitForSeconds(invulnerabilityTime/4);
        sr.enabled = false;
        yield return new WaitForSeconds(invulnerabilityTime/4);
        sr.enabled = true;
        yield return new WaitForSeconds(invulnerabilityTime/4);
        sr.enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        isInvulnerable = false;
    }

    public void Heal(int healAmount)
    {
        currentNumberOfHearts += healAmount;
        if (currentNumberOfHearts > currentMaxNumberOfHearts)
        {
            currentNumberOfHearts = currentMaxNumberOfHearts;
        }
        UpdateHealthBar();
    }

    public void HealthUp()
    {
        if (currentMaxNumberOfHearts < maxNumberOfHearts)
        {
            currentMaxNumberOfHearts++;
            currentNumberOfHearts++;
            UpdateHealthBar();
        }
        else
        {
            Debug.Log("No se puede aumentar m�s la salud m�xima.");
        }
    }

    private void UpdateHealthBar()
    {
        foreach (Transform child in healthBarParent)
        {
            Destroy(child.gameObject);
        }

        Vector3 topLeft = GetTopLeftCorner(healthBarParent.GetComponent<RectTransform>());

        for (int i = 0; i < currentMaxNumberOfHearts; i++)
        {
            GameObject heart = Instantiate(i < currentNumberOfHearts ? heartPrefab : emptyHeartPrefab, healthBarParent);
            Vector3 heartPosition = topLeft + new Vector3(i * heartSpacing + heartSpacing, -heartSpacing, 0);
            heart.transform.localPosition = heartPosition;
        }

        Debug.Log($"Health Updated: {currentNumberOfHearts}/{currentMaxNumberOfHearts}");
    }

    private Vector3 GetTopLeftCorner(RectTransform canvasRect)
    {
        Vector3[] corners = new Vector3[4];
        canvasRect.GetLocalCorners(corners);
        return corners[1]; // Esquina superior izquierda
    }

    //Pause
    public PauseManager pauseManager;

    void Start()
    {
        InitializeHealth();

        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < equippedWeapon.Length; i++)
        {
            if (equippedWeapon[i] != null)
            {
                equippedWeapon[i].gameObject.SetActive(i == equippedWeaponIndex);
            }
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (!pauseManager.isPaused) 
        {
            RotateFirePointTowardsMouse();

            if (Input.GetMouseButtonDown(0) && equippedWeapon != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                Vector2 shootDirection = (mousePosition - transform.position).normalized;

                equippedWeapon[equippedWeaponIndex].Fire(shootDirection);
            }

            if (Input.GetMouseButtonDown(1) && equippedWeapon != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                Vector2 shootDirection = (mousePosition - transform.position).normalized;

                equippedWeapon[equippedWeaponIndex].Fire2(shootDirection);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
            {
                transform.LookAt(transform.position);
                StartCoroutine(Dash());
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeWeapon(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeWeapon(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseManager.TogglePause();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item") 
        {
            Debug.Log("TrigerEnterred");
            Item item = collision.GetComponent<Item>();

            if (item != null)
            {
                item.ApplyEffect(this);
                equippedItems.Add(item);
                Destroy(collision.gameObject);
            }
        }
    }


    private IEnumerator Dash()
    {
        isDashing = true;

        nextDashTime = Time.time + _dashCooldown;

        Vector2 dashDirection = movement * dashDistance;
        Vector2 dashTarget = rb.position + dashDirection;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, dashTarget, elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.MovePosition(rb.position + movement * _speed * Time.deltaTime);
        }
    }

    private void RotateFirePointTowardsMouse()
    {
        Transform firePoint = equippedWeapon[equippedWeaponIndex].GetFirePoint();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; 

        Vector2 direction = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        equippedWeapon[equippedWeaponIndex].gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        equippedWeapon[equippedWeaponIndex].gameObject.transform.position = transform.position + (Vector3)direction * 1f;


        animation.transform.localScale = new Vector3(mousePosition.x < transform.position.x ? -1 : 1, 1, 1);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        equippedWeapon[equippedWeaponIndex] = newWeapon;
    }

    public void ChangeWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapon.Length)
        {
            return;
        }

        for (int i = 0; i < equippedWeapon.Length; i++)
        {
            if (equippedWeapon[i] != null)
            {
                equippedWeapon[i].gameObject.SetActive(i == index);
            }
        }

        equippedWeaponIndex = index;
    }
}
