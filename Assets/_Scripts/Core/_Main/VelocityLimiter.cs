using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// VelocityLimiter Description
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VelocityLimiter : MonoBehaviour
{
    #region Attributes
    //[FoldoutGroup("GamePlay"), Tooltip("The velocity at which drag should begin being applied."), SerializeField]
    //private float dragStartVelocity;

    //[FoldoutGroup("GamePlay"), Tooltip("The velocity at which drag should equal maxDrag."), SerializeField]
    //private float dragMaxVelocity;

    // The maximum allowed velocity. The velocity will be clamped to keep
    // it from exceeding this value. (Note: this value should be greater than
    // or equal to dragMaxVelocity.)
    [FoldoutGroup("GamePlay"), Tooltip("The maximum allowed velocity"), SerializeField]    
    private float maxVelocity;

    // The maximum drag to apply. This is the value that will
    // be applied if the velocity is equal or greater
    // than dragMaxVelocity. Between the start and max velocities,
    // the drag applied will go from 0 to maxDrag, increasing
    // the closer the velocity gets to dragMaxVelocity.
    //[FoldoutGroup("GamePlay"), Tooltip("The maximum drag to apply"), SerializeField]
    //private float maxDrag = 1.0f;

    // The original drag of the object, which we use if the velocity
    // is below dragStartVelocity.
    //private float originalDrag;
    // Cache the rigidbody to avoid GetComponent calls behind the scenes.
    private Rigidbody rb;
    // Cached values used in FixedUpdate
    //private float sqrDragStartVelocity;
    //private float sqrDragVelocityRange;
    //private float sqrMaxVelocity;
    #endregion

    #region Initialization

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //originalDrag = rb.drag;
        //Initialize(dragStartVelocity, dragMaxVelocity, maxVelocity, maxDrag);
    }

    /*
    // Sets the threshold values and calculates cached variables used in FixedUpdate.
    // Outside callers who wish to modify the thresholds should use this function. Otherwise,
    // the cached values will not be recalculated.
    private void Initialize(float dragStartVelocity, float dragMaxVelocity, float maxVelocity, float maxDrag)
    {
        this.dragStartVelocity = dragStartVelocity;
        this.dragMaxVelocity = dragMaxVelocity;
        this.maxVelocity = maxVelocity;
        this.maxDrag = maxDrag;

        // Calculate cached values
        sqrDragStartVelocity = dragStartVelocity * dragStartVelocity;
        sqrDragVelocityRange = (dragMaxVelocity * dragMaxVelocity) - sqrDragStartVelocity;
        sqrMaxVelocity = maxVelocity * maxVelocity;
    }
    */
    #endregion

    #region Core
    /*
    private void LimitVelocity()
    {
        Vector3 v = rb.velocity;
        // We use sqrMagnitude instead of magnitude for performance reasons.
        float vSqr = v.sqrMagnitude;

        if (vSqr > sqrDragStartVelocity)
        {
            rb.drag = Mathf.Lerp(originalDrag, maxDrag, Mathf.Clamp01((vSqr - sqrDragStartVelocity) / sqrDragVelocityRange));

            // Clamp the velocity, if necessary
            if (vSqr > sqrMaxVelocity)
            {
                // Vector3.normalized returns this vector with a magnitude
                // of 1. This ensures that we're not messing with the
                // direction of the vector, only its magnitude.
                rb.velocity = v.normalized * maxVelocity;
            }
        }
        else
        {
            rb.drag = originalDrag;
        }
    }
    */

    private void otherLimit()
    {
        float speed = Vector3.Magnitude(rb.velocity);  // test current object speed
        if (speed > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }
    #endregion

    #region Unity ending functions

    private void FixedUpdate()
    {
        //LimitVelocity();
        otherLimit();
    }

	#endregion
}
