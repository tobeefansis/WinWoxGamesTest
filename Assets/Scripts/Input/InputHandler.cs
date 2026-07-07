using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool JumpPressed { get; private set; }

    [SerializeField] private float mouseSensitivity = 2f;

    private void Awake()
    {
        ServiceLocator.Instance.Register(this);
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(horizontal, vertical).normalized;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        LookInput = new Vector2(mouseX, mouseY);

        IsSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        JumpPressed = Input.GetKeyDown(KeyCode.Space);
    }
}
