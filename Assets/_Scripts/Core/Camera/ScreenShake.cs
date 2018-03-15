using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {

    [SerializeField] private  float screenBump = 0.25f;
    [SerializeField] private float shakeDuration = 5f;

    private float shakeTimer;
    private Vector3 camPosition;
    private float xPos;
    private float yPos;

	// Use this for initialization
	void Start ()
    {
        camPosition = transform.localPosition;
        shakeTimer = 0;
        xPos = 0;
        yPos = 0;
	}
   
    public void Shake()
    {
        lock(this)
        {
            if (shakeTimer <= shakeDuration)
            {
                xPos = Random.Range(-1, 2) * screenBump;
                yPos = Random.Range(-1, 2) * screenBump;

                transform.localPosition = new Vector3(xPos, yPos, -13);

                shakeTimer++;
                StartCoroutine(ShakeWaiting());
            }
            else
            {
                transform.localPosition = camPosition;
                shakeTimer = 0;
            }
        }
    }
   
    IEnumerator ShakeWaiting()
    {
        yield return new WaitForSeconds(0.05f);
        Shake();
        yield return null;
    }
}

