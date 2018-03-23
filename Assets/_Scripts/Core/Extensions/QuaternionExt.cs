using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExt
{
    public enum TurnType
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// rotate smoothly selon 2 axe
    /// </summary>
	public static Quaternion DirObject(this Quaternion rotation, float horizMove, float vertiMove, float turnRate, TurnType typeRotation = TurnType.Z)
	{
		float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, -vertiMove * turnRate * Time.deltaTime);

		Quaternion _targetRotation = Quaternion.identity;

        float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
        float y = (typeRotation == TurnType.Y) ? heading * 1 * Mathf.Rad2Deg : 0;
        float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

        _targetRotation = Quaternion.Euler(x, y, z);
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
    

    /// <summary>
    /// get mirror of a vector, according to a normal
    /// </summary>
    /// <param name="point">Vector 1</param>
    /// <param name="normal">normal</param>
    /// <returns>vector mirror to 1 (reference= normal)</returns>
    public static Vector3 ReflectionOverPlane(Vector3 point, Vector3 normal)
    {
        return point - 2 * normal * Vector3.Dot(point, normal) / Vector3.Dot(normal, normal);
    }

    /// <summary>
    /// prend un quaternion en parametre, et retourn une direction selon un repère
    /// </summary>
    /// <param name="quat">rotation d'un transform</param>
    /// <param name="up">Vector3.up</param>
    /// <returns>direction du quaternion</returns>
    public static Vector3 QuaternionToDir(Quaternion quat, Vector3 up)
    {
        return ((quat * up).normalized);
    }

    /// <summary>
    /// Dot product de 2 vecteur, retourne négatif si l'angle > 90°, 0 si angle = 90, positif si moin de 90
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <returns>retourne négatif si l'angle > 90°</returns>
    public static float DotProduct(Vector3 a, Vector3 b)
    {
        return (Vector3.Dot(a, b));
    }

    /// <summary>
    /// retourne le vecteur de droite au vecteur A, selon l'axe Z
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 CrossProduct(Vector3 a, Vector3 z)
    {
        return (Vector3.Cross(a, z));
    }

    public static Vector3 GetMiddleOf2Vector(Vector3 a, Vector3 b)
    {
        return ((a + b).normalized);
    }
    public static Vector3 GetMiddleOfXVector(Vector3[] arrayVect)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < arrayVect.Length; i++)
        {
            sum += arrayVect[i];
        }
        return ((sum).normalized);
    }
    /// <summary>
    /// get la bisection de 2 vecteur
    /// </summary>
    public static Vector3 GetbisectionOf2Vector(Vector3 a, Vector3 b)
    {
        return ((a + b) * 0.5f);
    }

    /// <summary>
    /// prend un vecteur2 et retourne l'angle x, y en degré
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        //float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (angle + 360) % 360;                       // angle in [0,360]
        return (angle360);

        //return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    /// <summary>
    /// prend un angle A, B, en 360 format, et test si les 2 angles sont inférieur à différence (180, 190, 20 -> true, 180, 210, 20 -> false)
    /// </summary>
    /// <param name="angleReference">angle A</param>
    /// <param name="angleToTest">angle B</param>
    /// <param name="differenceAngle">différence d'angle accepté</param>
    /// <returns></returns>
    public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle, out float diff)
    {
        if (angleReference < 0 || angleReference > 360 ||
            angleToTest < 0 || angleToTest > 360)
        {
            Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
        }

        diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

        //diff = Mathf.Abs(angleReference - angleToTest);        

        if (diff <= differenceAngle)
            return (true);
        return (false);
    }

    /// <summary>
    /// retourne un vecteur2 par rapport à un angle
    /// </summary>
    /// <param name="angle"></param>
    public static Vector3 GetVectorFromAngle(float angle)
    {
        return (new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0));
    }

    /// <summary>
    /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <param name="n">reference</param>
    /// <returns>Retourne un angle en degré</returns>
    public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
        return (angle360);
    }
}
