using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    [SerializeField] private Weapon[] equippedWeapon;
    private int equippedWeaponIndex = 0;
    private Transform firePoint;

    private float speed = 5f;

    private float dashDistance = 4f;
    private float dashCooldown = 3f;
    private float nextDashTime = 0f;
    private bool isDashing = false;
    private float dashDuration = 0.2f;

    void Start()
    {
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

    private IEnumerator Dash()
    {
        isDashing = true;

        nextDashTime = Time.time + dashCooldown;

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
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
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
