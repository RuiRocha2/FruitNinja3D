using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juiceEffect;

    public int points = 1;

    public AudioClip sliceSound;
    private AudioSource audioSource;

    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        Debug.Log("Fruta cortada");

        GameManager.Instance.IncreaseScore(points);
        GameManager.Instance.FruitSliced();

        // Desabilita a fruta inteira
        fruitCollider.enabled = false;
        whole.SetActive(false);

        // Habilita a fruta cortada
        sliced.SetActive(true);
        juiceEffect.Play();

        // Rotaciona com base no ângulo de corte
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        // Adiciona uma força a cada fatia com base na direção da lâmina
        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Colisão detectada com o jogador");

            // Toca o som de corte na colisão com o jogador
            if (sliceSound != null)
            {
                Debug.Log("Tocando som de corte na colisão");
                audioSource.PlayOneShot(sliceSound);
            }

            Blade blade = other.GetComponent<Blade>();
            Slice(blade.Direction, blade.transform.position, blade.sliceForce);
        }
    }
}
