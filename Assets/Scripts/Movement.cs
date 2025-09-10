using UnityEngine;

public class FishController : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    [Header("Movement Limits")]
    [SerializeField] float minX = -8f;
    [SerializeField] float maxX = 8f;
    [SerializeField] float minY = -4f;
    [SerializeField] float maxY = 4f;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager2D.Instance == null) return;
        if (!GameManager2D.Instance.PlayerCanMove()) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical).normalized;

        if (horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;
        }
    }

    void FixedUpdate()
    {
        if (GameManager2D.Instance == null) return;
        if (!GameManager2D.Instance.PlayerCanMove()) return;

        rb.linearVelocity = movement * speed;

        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(rb.position.x, minX, maxX),
            Mathf.Clamp(rb.position.y, minY, maxY)
        );
        rb.position = clampedPosition;
    }
}