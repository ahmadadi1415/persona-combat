using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;

    private Vector2 _movement;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _movement * _movementSpeed;
    }

    public void Move(Vector2 movement)
    {
        _movement = movement;
    }
}
