using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsExt
{
    /// <summary>
    /// effectue une force constante en direction voulu
    /// </summary>
    public static void ApplyConstForce(Rigidbody rb, Vector3 dir, float force)
    {
        Debug.DrawRay(rb.transform.position, dir.normalized, Color.cyan, 5f);
        //rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        //Debug.Log("ici ajout petite force");
        rb.velocity += dir * Physics.gravity.y * (force - 1) * Time.fixedDeltaTime;
    }
}
