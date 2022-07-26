using UnityEngine;

public sealed class CameraHandler : MonoBehaviour
{
    [SerializeField] new Camera camera;
    CameraHandler() { }
    internal void CenterMe(float x, float y)
    {
        Transform transform = this.transform;
        transform.SetPositionAndRotation(new Vector3(x - .5f, y - .5f, transform.position.z), Quaternion.identity);
        if (x < y)
        {
            camera.orthographicSize = y;
        }
        else if (x - y > y)
        {
            camera.orthographicSize = x - y;
        }
        else
        {
            camera.orthographicSize = y;
        }
    }
}
