using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)] public float baseBombChance = 0.05f;
    private float bombChance;

    public float baseMinSpawnDelay = 0.25f;
    public float baseMaxSpawnDelay = 1f;
    private float minSpawnDelay;
    private float maxSpawnDelay;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;
    
    public GameManager score;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        bombChance = baseBombChance;
        minSpawnDelay = baseMinSpawnDelay;
        maxSpawnDelay = baseMaxSpawnDelay;
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            UpdateDifficulty();

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    private void UpdateDifficulty()
{
    // Acessa o score atual através da propriedade pública Score
    int score = GameManager.Instance.Score; // Ajuste conforme a implementação do seu GameManager

    // Ajusta a chance de bomba e os intervalos de spawn com base no score
    bombChance = baseBombChance + (score / 20) * 0.01f;
    bombChance = Mathf.Clamp(bombChance, 0f, 1f); // Garante que a chance não exceda 100%

    // Diminui os intervalos de spawn conforme o score aumenta
    float difficultyMultiplier = Mathf.Clamp(1 - (score / 1000f), 0.5f, 1f);
    minSpawnDelay = baseMinSpawnDelay * difficultyMultiplier;
    maxSpawnDelay = baseMaxSpawnDelay * difficultyMultiplier;
}
}
