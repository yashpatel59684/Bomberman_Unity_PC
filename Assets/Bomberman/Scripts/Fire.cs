using UnityEngine;

public sealed class Fire : MonoBehaviour
{
    Fire() { }
    [Range(0, 5)]
    [SerializeField] float destroyAfterStart = .5f;
    void Start()
    {
        Destroy(gameObject, destroyAfterStart);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall")) return;
        GetComponent<Collider2D>().enabled = false;
        if (collision.CompareTag("Enemy"))
        {
            GameManager.Instance.RemoveEnemy(collision.transform);
        }
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance?.PlayerDie();
        }
        //Debug.Log($"{collision.gameObject.name}");
        Destroy(collision.gameObject);
    }
}
