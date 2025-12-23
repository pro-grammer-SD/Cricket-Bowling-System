using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBounced = false;

    // State
    private bool isSwingMode;
    private float accuracy;
    private float direction; // -1, 0, 1
    private float currentSwingForce;
    private float currentSpinAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Step 1: Completely stop the ball and reset rotation so it is ready to be thrown.
    /// </summary>
    public void ResetPhysics()
    {
        hasBounced = false;

        // KILL all physics from previous throw
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset rotation so 'transform.right' (Swing Direction) is consistent
        rb.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);

        // Cancel any pending disable timers from previous runs
        CancelInvoke(nameof(DeactivateBall));
    }

    /// <summary>
    /// Step 2: Set the data for the new throw.
    /// </summary>
    public void Initialize(bool swing, float acc, float dir, float swingForce, float spinAngle)
    {
        isSwingMode = swing;
        accuracy = acc;
        direction = dir;
        currentSwingForce = swingForce;
        currentSpinAngle = spinAngle;

        // Auto-hide after 8 seconds
        Invoke(nameof(DeactivateBall), 8f);
    }

    void FixedUpdate()
    {
        // Apply Swing Force
        if (isSwingMode && !hasBounced && direction != 0)
        {
            
            Vector3 sideForce = transform.right * direction * accuracy * currentSwingForce;
            rb.AddForce(sideForce, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasBounced && collision.gameObject.CompareTag("Ground"))
        {
            hasBounced = true;
            if (!isSwingMode && direction != 0)
            {
                ApplySpin();
            }
        }
    }

    void ApplySpin()
    {
        Vector3 currentVel = rb.linearVelocity;
        float turn = direction * accuracy * currentSpinAngle;
        Quaternion turnRot = Quaternion.Euler(0, turn, 0);
        rb.linearVelocity = turnRot * currentVel;
    }

    void DeactivateBall()
    {
        gameObject.SetActive(false);
    }
}