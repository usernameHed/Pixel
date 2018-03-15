using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExt
{
    /// <summary>
    /// rotate smoothly selon 2 axe
    /// </summary>
	public static Quaternion DirObject(this Quaternion rotation, float horizMove, float vertiMove, float turnRate)
	{
		float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, vertiMove * turnRate * Time.deltaTime);

		Quaternion _targetRotation = Quaternion.identity;

		_targetRotation = Quaternion.Euler(0f, -heading * Mathf.Rad2Deg, 0f);
		rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
		return (rotation);
	}
    /// <summary>
    /// rotate un quaternion selon un vectir directeur
    /// use: transform.rotation.LookAtDir((transform.position - target.transform.position) * -1);
    /// </summary>
    public static Quaternion LookAtDir(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir * -1);
        return (rotation);
    }
}
