
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveInput : MonoBehaviour
{
    private static readonly int RUN = Animator.StringToHash("RUN");
    private PlayerInputActions playerInputActions;
    private Animator animator;
    [SerializeField] private VirtualJoystick joystick; 
    
    [SerializeField] private float speed = 5f;
    
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        animator.SetTrigger(RUN);
    }

    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;

        playerInputActions.Disable();
    }
    
    private void Update()
    {
        // PC/패드 입력
        Vector2 inputFromDevice = MoveInput;

        // 모바일 조이스틱 입력 (없으면 Vector2.zero)
        if (joystick != null && joystick.Input != Vector2.zero)
            inputFromDevice = joystick.Input;

        // 이동 처리
        Vector3 dir = new Vector3(inputFromDevice.x, 0, inputFromDevice.y);
        transform.position += dir * (speed * Time.deltaTime);
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        Debug.Log($"MoveInput: {MoveInput}");
    }
}



