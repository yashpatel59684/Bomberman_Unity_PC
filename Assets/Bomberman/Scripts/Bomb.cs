using UnityEngine;

public sealed class Bomb : MonoBehaviour
{
    Bomb(){ }
    [Range(2,10)]
    [SerializeField] float countdown = 3f;
    void Update()
    {
        if (countdown <= 0f)
        {
            GameManager.Instance.BlasstBomb();
        }
        else if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
