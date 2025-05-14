using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float _turnSpeed;

    Vector3 _cameraOffset;
    void Start()
    {
        _cameraOffset = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
            return; // Evita ejecutar si no encuentra al jugador aún.

        _cameraOffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * _turnSpeed, Vector3.up) * _cameraOffset;
        transform.position = player.transform.position + _cameraOffset;
        transform.LookAt(player.transform.position);
    }
}

