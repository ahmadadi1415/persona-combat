using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;

    [field: SerializeField] public bool IsMoving { get; private set; } = false;

    private Vector2 _movement;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private readonly int _horizontalAnim = Animator.StringToHash("Horizontal");
    private readonly int _verticalAnim = Animator.StringToHash("Vertical");
    private readonly int _lastVerticalAnim = Animator.StringToHash("LastVertical");
    private readonly int _lastHorizontalAnim = Animator.StringToHash("LastHorizontal");

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _movement * _movementSpeed;
    }

    public void Move(Vector2 movement)
    {
        _movement = movement;
        _animator.SetFloat(_horizontalAnim, movement.x);
        _animator.SetFloat(_verticalAnim, movement.y);

        IsMoving = movement != Vector2.zero;

        if (IsMoving)
        {
            _animator.SetFloat(_lastHorizontalAnim, movement.x);
            _animator.SetFloat(_lastVerticalAnim, movement.y);
        }
    }
}
