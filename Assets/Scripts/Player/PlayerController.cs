using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float verticalClampAngle = 80f;

    private CharacterController _controller;
    private InputHandler _input;
    private float _verticalVelocity;
    private float _cameraPitch;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        ServiceLocator.Instance.Register(this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _input = ServiceLocator.Instance.Get<InputHandler>();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float speed = _input.IsSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * _input.MoveInput.x + transform.forward * _input.MoveInput.y;
        move *= speed;

        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        _verticalVelocity += gravity * Time.deltaTime;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        transform.Rotate(Vector3.up * _input.LookInput.x);

        _cameraPitch -= _input.LookInput.y;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -verticalClampAngle, verticalClampAngle);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }
}
