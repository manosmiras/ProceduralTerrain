using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float Speed = 20f;
    private const float Distance = 10000;

    // Update is called once per frame
    void LateUpdate()
    {
        var origin = transform.position;
        origin.y = 1000f;
        var direction = Vector3.down;
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (Physics.Raycast(origin, direction * Distance, out var hitInfo))
        {
            Debug.DrawLine(origin, direction * Distance, Color.red);
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }
    }
}
