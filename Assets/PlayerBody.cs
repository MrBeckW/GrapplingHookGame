using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    private float _gravity = 9.8f;

    [SerializeField]
    private LayerMask _groundLayer;

    [SerializeField]
    private Vector2 _groundCheckSize;

    [SerializeField]
    private Vector2 _groundCheckOffset;

    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Physics2D.BoxCast(transform.position + (Vector3)_groundCheckOffset, _groundCheckSize, 0f, Vector2.zero, 0f, _groundLayer))
            _rb.linearVelocity += Vector2.down * _gravity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)_groundCheckOffset, _groundCheckSize);
    }
}
