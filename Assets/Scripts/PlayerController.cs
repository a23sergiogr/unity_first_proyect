using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    [SerializeField] private Weapon[] equippedWeapon;
    private int equippedWeaponIndex = 0;
    private Transform firePoint;
    [SerializeField] private GameObject animation;


    [SerializeField] private List<Item> equippedItems = new List<Item>();



private float _speed = 5f;
    public float speed
    {
        get { return _speed; }
        set
        {
            // Si el valor es mayor a 30, ajusta a 30
            _speed = Mathf.Min(30f, value); }
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

    /*Healt and Mana Bars*/
    private float maxNumberOfHearts = 20;
    private float currentMaxNumberOfHearts = 5;
    private float currentNumberOfHearts;
    private float invulnerabilityTime = 0.75f;
    private bool isInvulnerable = false;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject emptyHeartPrefab;
    [SerializeField] private Transform healthBarParent;
    private float heartSpacing = 150f;

    public void InitializeHealth()
    {
        currentNumberOfHearts = currentMaxNumberOfHearts;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            return;
        }

        currentNumberOfHearts -= damage;
        if (currentNumberOfHearts <= 0) 
        { 
            return; 
        }

        StartCoroutine(Invulnerability());
        UpdateHealthBar();
    }

    IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    public void Heal(float healAmount)
    {

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        foreach (Transform child in healthBarParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentMaxNumberOfHearts; i++)
        {
            GameObject heart = Instantiate(i < currentNumberOfHearts ? heartPrefab : emptyHeartPrefab, healthBarParent);
            heart.transform.localPosition = new Vector3(i * heartSpacing - 1100, 580, 0);
        }

        Debug.Log($"Health Updated: {currentNumberOfHearts}/{currentMaxNumberOfHearts}");
    }

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
        firePoint = equippedWeapon[equippedWeaponIndex].GetFirePoint();

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
