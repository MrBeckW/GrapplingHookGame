using UnityEngine;

/// <summary>
/// The hook of the grapple gun that latches onto some geometry for the player to reel themselves towards.
/// </summary>
public class GrappleHook : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField]
    private GrappleGun _grappleGun;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        /* 
         * If the distance between the hook and grapple gun is greater than the gun's max distance,
         * then stop the hook.
         */
        if (Vector2.Distance(transform.position, _grappleGun.transform.position) >= _grappleGun.MaxDist)
            _grappleGun.StopGrappling();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the collider and rigid body of the touched object.
        Collider2D col = collision.GetContact(0).collider;
        Rigidbody2D hookedRB = col.GetComponent<Rigidbody2D>();
        
        /*
         * Stop simulating the hook's rigid body.
         * Set the parent to the touched transform.
         * Tell the grapple gun that the hook has had a collision.
         */
        _rb.simulated = false;
        transform.parent = col.transform;
        _grappleGun.Hook(Vector2.Distance(_grappleGun.transform.position, transform.position), hookedRB);
    }
}
