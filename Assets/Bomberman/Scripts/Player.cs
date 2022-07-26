using UnityEngine;
using UnityEngine.InputSystem;
public sealed class Player : MonoBehaviour
{
    Vector3 newPos;
    [Range(0, 10)]
    [SerializeField] float runSpeed = 2.5f;
    Player() { }
    public void OnBombSet(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            GameManager.Instance.PlaceBomb(transform);
        }
    }
    public void OnMove(InputAction.CallbackContext obj)
    {
        var data = obj.ReadValue<Vector2>();
        newPos = new Vector3((float)data.x, (float)data.y, 0);
    }
    void FixedUpdate()
    {
        newPos.Normalize();
        transform.Translate(runSpeed * Time.deltaTime * newPos);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
