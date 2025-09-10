using System.Collections;
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] fishPrefabs;

    [Header("Spawn Timing (seconds)")]
    [SerializeField] private float spawnDelayMin = 0.7f;
    [SerializeField] private float spawnDelayMax = 1.5f;

    [Header("Fish Speed (units/sec)")]
    [SerializeField] private float speedMin = 1.5f;
    [SerializeField] private float speedMax = 3.5f;

    [Header("Screen Padding")]
    [SerializeField] private float horizontalPadding = 1.0f;
    [SerializeField] private float verticalPadding = 0.5f;

    [Header("Difficulty Over Time (optional)")]
    [SerializeField] private float speedRampPerMinute = 0.8f;
    [SerializeField] private float minSpawnClamp = 0.35f;      
    [SerializeField] private float spawnRampPerMinute = 0.3f;   

    private Camera cam;
    private float elapsed;
    private bool running = true;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("EnemySpawner2D: No Main Camera found.");
        }
    }
    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
    }

    public void StopSpawning()
    {
        running = false;
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (running)
        {
            SpawnOne();

            float minutes = elapsed / 60f;
            float dSpeed = speedRampPerMinute * minutes;
            float dRate = spawnRampPerMinute * minutes;

            float delayMinNow = Mathf.Max(spawnDelayMin - dRate, minSpawnClamp);
            float delayMaxNow = Mathf.Max(spawnDelayMax - dRate, delayMinNow + 0.05f);

            float wait = Random.Range(delayMinNow, delayMaxNow);
            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnOne()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0 || cam == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        float leftX = camPos.x - halfWidth;
        float rightX = camPos.x + halfWidth;
        float minY = camPos.y - halfHeight + verticalPadding;
        float maxY = camPos.y + halfHeight - verticalPadding;

        bool fromLeft = Random.value < 0.5f;

        float spawnX = fromLeft ? (leftX - horizontalPadding) : (rightX + horizontalPadding);
        float y = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(spawnX, y, 0f);

        GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        GameObject fish = Instantiate(prefab, spawnPos, Quaternion.identity);

        SpriteRenderer sr = fish.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !fromLeft;
        }

        float minutes = elapsed / 60f;
        float dSpeed = speedRampPerMinute * minutes;
        float speed = Random.Range(speedMin + dSpeed, speedMax + dSpeed);

        Vector2 dir;
        float destroyX;
        if (fromLeft)
        {
            dir = Vector2.right;
            destroyX = rightX + horizontalPadding;
        }
        else
        {
            dir = Vector2.left;
            destroyX = leftX - horizontalPadding;
        }

        EnemyFish2D mover = fish.GetComponent<EnemyFish2D>();
        if (mover == null) mover = fish.AddComponent<EnemyFish2D>();
        mover.Initialize(dir, speed, destroyX, flipOnDirection: false);
    }
}