using UnityEngine;

public class MainCharacterMove : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;

    private float movementSpeed;

    Animator anim;
    Rigidbody playerRigibody;
    Transform cameraTransform;

    Vector3 movement;

    int floorMask;
    float camRayLength = 100f;

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigibody = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Move(horiz, vert);
        Turning();
        Animating(horiz, vert);
    }

    private void Move(float horiz, float vert)
    {
        // Calcula dirección relativa a la cámara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        movement = (forward * vert + right * horiz).normalized;

        // Determina velocidad según input
        if (movement.magnitude > 0 && Input.GetKey(KeyCode.LeftShift))
            movementSpeed = runSpeed;
        else
            movementSpeed = walkSpeed;

        Vector3 moveOffset = movement * movementSpeed * Time.deltaTime;

        playerRigibody.MovePosition(transform.position + moveOffset);
    }

    private void Turning()
    {
        // El personaje rota hacia la dirección del movimiento, si se está moviendo
        if (movement != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(movement);
            playerRigibody.MoveRotation(newRotation);
        }
    }

    void Animating(float horiz, float vert)
    {
        float magnitude = new Vector2(horiz, vert).magnitude;

        if (magnitude <= 0)
        {
            anim.SetFloat("movements", 0, 0.1f, Time.deltaTime); // Idle
        }
        else if (magnitude > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetFloat("movements", 1, 0.1f, Time.deltaTime); // Correr
        }
        else if (magnitude > 0)
        {
            anim.SetFloat("movements", 0.5f, 0.1f, Time.deltaTime); // Caminar
        }
    }

    void Start()
    {
        Cursor.visible = false;
    }
}
