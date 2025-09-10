using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFish2D : MonoBehaviour
{
    private Vector2 direction = Vector2.right;
    private float speed = 2f;
    private float destroyX = 9999f;

    private bool flipOnDirection = false;

    private void Reset()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Initialize(Vector2 dir, float spd, float destroyXWorld, bool flipOnDirection = false)
    {
        direction = dir.normalized;
        speed = spd;
        destroyX = destroyXWorld;
        this.flipOnDirection = flipOnDirection;

        if (flipOnDirection)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (direction.x < 0f)
                    sr.flipX = true;  
                else
                    sr.flipX = false;
            }
        }
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (direction.x > 0f && transform.position.x > destroyX)
            Destroy(gameObject);
        else if (direction.x < 0f && transform.position.x < destroyX)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy fish!");

            EnemyFish2D[] allFishes = Object.FindObjectsByType<EnemyFish2D>(FindObjectsSortMode.None);
            foreach (EnemyFish2D fish in allFishes)
            {
                fish.enabled = false;
            }

            GameManager2D.Instance?.GameOver();
        }
    }
}