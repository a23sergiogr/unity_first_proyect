using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    [SerializeField] private Weapon equippedWeapon;
    [SerializeField] private Transform firePoint;

    private float speed = 5f;

    private float dashDistance = 4f;
    private float dashCooldown = 3f;
    private float nextDashTime = 0f;
    private bool isDashing = false;
    private float dashDuration = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

            equippedWeapon.Fire(shootDirection);
        }

        if (Input.GetMouseButtonDown(1) && equippedWeapon != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector2 shootDirection = (mousePosition - transform.position).normalized;

            equippedWeapon.Fire2(shootDirection);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime) 
        { 
            transform.LookAt(transform.position);
            StartCoroutine(Dash());
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; 

        Vector2 direction = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        firePoint.position = transform.position + (Vector3)direction * 1f;
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        equippedWeapon = newWeapon;
    }
}
