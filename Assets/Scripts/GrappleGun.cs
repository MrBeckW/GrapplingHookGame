using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The grapple gun the player uses to navigate.
/// </summary>
public class GrappleGun : MonoBehaviour
{
    /// <summary>
    /// Whether or not the grapple gun is hooked onto something.
    /// </summary>
    private bool _hooked = false;

    /// <summary>
    /// Whether or not the grapple gun is hooked onto something.
    /// </summary>
    public bool Hooked { get => _hooked; }

    /// <summary>
    /// The current 1D direction the player is reeling in.
    /// </summary>
    private float _reelDir = 0f;

    [Header("Grapple Values")]
    /// <summary>
    /// The minimum distance the player can reel in towards the hook.
    /// </summary>
    [SerializeField]
    private float _minDist = 0.7f;

    /// <summary>
    /// The maximum distance the player can reel in away from the hook.
    /// </summary>
    [SerializeField]
    private float _maxDist = 32f;
    public float MaxDist { get => _maxDist; }

    /// <summary>
    /// The weight of the reeling which controls how fast the reeling direction changes.
    /// </summary>
    [SerializeField]
    private float _reelWeight = 2f;

    /// <summary>
    /// The speed at which the grapple's distance changes.
    /// </summary>
    [SerializeField]
    private float _reelSpeed = 4f;

    /// <summary>
    /// The force that is applied for swinging.
    /// </summary>
    [SerializeField]
    private float _swingForce = 7f;

    /// <summary>
    /// The line used to represent the grapple's cable.
    /// </summary>
    private LineRenderer _line;

    /// <summary>
    /// The force to apply to the hook when shooting the grapple gun.
    /// </summary>
    [SerializeField]
    private float _hookForce = 4f;

    [Header("Camera Values")]
    /// <summary>
    /// The camera to get the mouse position from.
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// The weight of the camera's zoom as it lerps.
    /// </summary>
    [SerializeField]
    private float _camZoomWeight = 1f;

    /// <summary>
    /// The maximum amount of zoom that the camera to go out to.
    /// </summary>
    [SerializeField]
    private float _camMaxZoom = 32f;

    /// <summary>
    /// The margin which determines how much farther out the camera will zoom from the hook's position.
    /// </summary>
    [SerializeField]
    private float _camZoomMargin = 1.2f;

    /// <summary>
    /// The camera's initial zoom.
    /// </summary>
    private float _initCamZoom;

    /// <summary>
    /// The zoom to lerp the camera to.
    /// </summary>
    private float _targetCamZoom;

    [Header("Bodies and Joints")]
    /// <summary>
    /// The hook the grapple gun uses to grapple towards and away from.
    /// </summary>
    [SerializeField]
    private GrappleHook _hook;

    /// <summary>
    /// The player's main body to move around.
    /// </summary>
    [SerializeField]
    private Rigidbody2D _player;

    /// <summary>
    /// The join used for the grapple physics.
    /// </summary>
    [SerializeField]
    private DistanceJoint2D _grappleJoint;

    /// <summary>
    /// The player's input actions.
    /// </summary>
    private PlayerInput plyrInput;

    private void Start()
    {
        // Get the line to use as the grapple's cable.
        _line = GetComponent<LineRenderer>();
        _initCamZoom = _camera.orthographicSize;
        _targetCamZoom = _initCamZoom;

        // Initialize and enable the player input, and then bind the needed input functions.
        plyrInput = new();
        plyrInput.Enable();
        plyrInput.Player.Grapple.performed += OnGrapple;
        plyrInput.Player.StopGrappling.performed += OnStopGrappling;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the line's positions to that of the gun's and hook's current position.
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, _hook.transform.position);

        // Make sure the gun is hooked before doing the reeling and swinging calculations.
        if (_hooked)
        {
            /*
             * Update the anchor point for the grapple joint.
             * Since the anchor point is relative to the connected body,
             * we will have to set it to the hook's local position if there is one.
             */
            if (_grappleJoint.connectedBody)
                _grappleJoint.connectedAnchor = _hook.transform.localPosition;
            else
                _grappleJoint.connectedAnchor = _hook.transform.position;

            Reel();
            Swing();

            // If the grapple joint ever breaks and is disabled, then release the gun.
            if (!_grappleJoint.enabled)
                StopGrappling();
        }

        float hookDist = 0f;

        // If the hook is currently active, then zoom out the camera to see it.
        if (_hook.Released)
            hookDist = Vector2.Distance(transform.position, _hook.transform.position);

        // Lerp the camera's zoom to the currently targeted zoom value.
        _targetCamZoom = Mathf.Clamp(hookDist * _camZoomMargin, _initCamZoom, _camMaxZoom);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetCamZoom, _camZoomWeight * Time.deltaTime);
    }

    private void Reel()
    {
        // Get the reel input and update the reel direction.
        float reelInput = plyrInput.Player.Reel.ReadValue<float>();
        _reelDir = Mathf.Lerp(_reelDir, reelInput, _reelWeight * Time.deltaTime);

        // Update the grapple's distance and make sure to clamp it accordingly.
        _grappleJoint.distance -= _reelDir * _reelSpeed * Time.deltaTime;
        _grappleJoint.distance = Mathf.Clamp(_grappleJoint.distance, _minDist, _maxDist);
    }

    private void Swing()
    {
        // Get the swing input.
        float swingInput = plyrInput.Player.Swing.ReadValue<float>();

        // If the swing input is not zero, then perform the calculations and actions needed.
        if (swingInput != 0f)
        {
            // Add a force perpendicular to that of the hook's direction for swinging.
            Vector2 hookDir = Vector3.Normalize(_hook.transform.position - transform.position);
            float swingAngle = Mathf.Atan2(hookDir.y, hookDir.x) - Mathf.PI / 2f;
            Vector2 swingDir = new Vector3(Mathf.Cos(swingAngle), Mathf.Sin(swingAngle));
            _player.AddForce(swingDir * swingInput * _swingForce * Time.deltaTime);
        }
    }

    private Vector3 GetMousePos()
    {
        Vector2 mouseScrPos = Mouse.current.position.ReadValue();

        if (mouseScrPos.x > 0f && mouseScrPos.x < _camera.pixelWidth &&
            mouseScrPos.y > 0f && mouseScrPos.y < _camera.pixelHeight)
        {
            Vector3 mousePos = _camera.ScreenPointToRay(Mouse.current.position.ReadValue()).origin;
            mousePos.z = 0f;
            return mousePos;
        }
        else
            return transform.position;
    }

    /// <summary>
    /// Shoots out the hook for grappling.
    /// Called when the player performs the grapple action.
    /// </summary>
    /// <param name="context">
    /// The input context of the player.
    /// </param>
    private void OnGrapple(InputAction.CallbackContext context)
    {
        /* Get the direction of the mouse. This will be the direction to shoot the hook in.
         * Mouse position calculation from Mahsa on https://stackoverflow.com/questions/66930040/how-to-find-the-mouses-position-using-the-new-input-system.
         */
        Vector2 dir = Vector3.Normalize(GetMousePos() - transform.position);

        // Reset the hook to a state that it can be fired in.
        ResetHook();
        _line.enabled = true;
        
        // Shoot out the hook.
        _hook.Shoot(transform.position, _player.linearVelocity, dir * _hookForce);
    }

    /// <summary>
    /// Stop grappling towards or away from something.
    /// Called when the player does the stop grappling action.
    /// </summary>
    /// <param name="context">
    /// The input context of the player.
    /// </param>
    private void OnStopGrappling(InputAction.CallbackContext context) => StopGrappling();

    /// <summary>
    /// Reset some of the grapple gun so that it can be fired again.
    /// Note that the difference between this and StopGrappling is that this should also be used before the hook is fired.
    /// </summary>
    private void ResetHook()
    {
        _hooked = false;
        _hook.Retract();
        _line.enabled = false;
        _grappleJoint.enabled = false;
        _grappleJoint.connectedBody = null;
        _targetCamZoom = _initCamZoom;
    }

    /// <summary>
    /// Stop grappling towards or away from something.
    /// </summary>
    public void StopGrappling()
    {
        ResetHook();
        _grappleJoint.connectedBody = null;
    }

    /// <summary>
    /// Hooks the grapple gun, allowing the player to reel and swing.
    /// </summary>
    /// <param name="dist">
    /// The distance between the grapple gun and the hook.
    /// </param>
    /// <param name="hookedRB">
    /// The rigid body that was hooked onto. If there wasn't one,
    /// then this value will be null.
    /// </param>
    public void Hook(float dist, Rigidbody2D hookedRB)
    {
        // Allow the player to reel and swing. Make sure to reset the reel direction.
        _hooked = true;
        _reelDir = 0f;

        // Configure the grapple joint.
        _grappleJoint.enabled = true;
        _grappleJoint.distance = dist;
        _grappleJoint.connectedBody = hookedRB;
    }
}
