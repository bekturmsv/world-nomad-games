using UnityEngine;

public class FpsPlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraPivot;

    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -20f;
    private float pitch;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (controller == null)
        {
            Debug.LogError("На FPS_Player нет Character Controller.");
            enabled = false;
            return;
        }

        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        Move();
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mouseX, 0f);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 horizontalVelocity = transform.TransformDirection(input) * moveSpeed;

        if (controller.isGrounded && verticalVelocity.y < 0f) verticalVelocity.y = -2f;

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 motion = horizontalVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime;
        controller.Move(motion);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}