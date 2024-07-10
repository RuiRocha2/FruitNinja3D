using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private AudioClip fuseSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Explode();
            GetComponent<AudioSource>().PlayOneShot(fuseSound);
        }
    }

}
