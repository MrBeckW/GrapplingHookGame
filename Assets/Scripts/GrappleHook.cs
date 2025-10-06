using UnityEngine;

/// <summary>
/// The hook of the grapple gun that latches onto some geometry for the player to reel themselves towards.
/// </summary>
public class GrappleHook : MonoBehaviour
{
    /// <summary>
    /// Whether or not the hook has been released/shot.
    /// </summary>
    private bool _released = false;

    /// <summary>
    /// Whether or not the hook has been released/shot.
    /// </summary>
    public bool Released { get => _released; }

    /// <summary>
    /// The rigid body of the hook.
    /// </summary>
    private Rigidbody2D _rb;

    /// <summary>
    /// The sprite of the hook.
    /// </summary>
    private SpriteRenderer _sprite;

    [SerializeField]
    private Color _unhookedColor;

    [SerializeField]
    private Color _hookedColor;

    /// <summary>
    /// The grapple gun that shoots the hook out.
    /// </summary>
    [SerializeField]
    private GrappleGun _grappleGun;

    /// <summary>
    /// The tile manager of the current scene.
    /// </summary>
    [SerializeField]
    private TileManager _tileManager;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        /* 
         * If the distance between the hook and grapple gun is greater than the gun's max distance,
         * then stop the hook.
         */
        if (_released && !_grappleGun.Hooked &&
            Vector2.Distance(transform.position, _grappleGun.transform.position) >= _grappleGun.MaxDist)
            _grappleGun.StopGrappling();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the collider and rigid body of the touched object.
        ContactPoint2D contact = collision.contacts[0];
        Collider2D col = collision.GetContact(0).collider;
        Rigidbody2D hookedRB = col.GetComponent<Rigidbody2D>();
        
        if (_tileManager.gameObject == col.gameObject)
        {
            Vector3 closest = col.ClosestPoint(contact.point);
            Vector3 dir = Vector3.Normalize(closest - transform.position);
            TileData tileData = _tileManager.GetTileData(closest + dir * 0.1f);
            
            if (!tileData.CanHook)
                return;
        }

        /*
         * Stop simulating the hook's rigid body.
         * Set the color of the sprite to the hooked color.
         * Set the parent to the touched transform.
         * Tell the grapple gun that the hook has had a collision.
         */
        _rb.simulated = false;
        _sprite.color = _hookedColor;
        transform.parent = col.transform;
        _grappleGun.Hook(Vector2.Distance(_grappleGun.transform.position, transform.position), hookedRB);
    }

    /// <summary>
    /// Shoots the hook out for it to grapple onto something.
    /// </summary>
    /// <param name="pos">
    /// The position to shoot the hook at.
    /// </param>
    /// <param name="vel">
    /// The velocity to set the hook to.
    /// </param>
    /// <param name="force">
    /// The force to apply to the hook.
    /// </param>
    public void Shoot(Vector2 pos, Vector2 vel, Vector2 force)
    {
        // Set the position.
        transform.parent = null;
        transform.position = pos;

        // Enable the rigid body, set its velocity, and apply a force to it.
        _rb.simulated = true;
        _rb.linearVelocity = vel;
        _rb.AddForce(force);

        // Enable the sprite, and set its color to the unhooked color.
        _sprite.enabled = true;
        _sprite.color = _unhookedColor;

        _released = true;
    }

    /// <summary>
    /// Retracts the hook back to a state that it can't be seen in.
    /// </summary>
    public void Retract()
    {
        // Stop simulating the rigid body, and disable the sprite.
        _rb.simulated = false;
        _sprite.enabled = false;
        _released = false;
    }
}
