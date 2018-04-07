using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipQjRXUXk4UmR3MjA%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipQjRXUXk4UmR3MjA
see:
https://drive.google.com/file/d/0Byzet-SVq6ipVkNiX00xS2RBV28/view
*/

public class TimeWithEffectOnCamera : MonoBehaviour
{
    public CameraTarget camTarget;
    [Range(0, 10f)]             public float timeOpti = 0.1f;
    [Range(0.01f, 300f)]             public float timeWithEffect = 1.0f;
    public bool isOk = true;
    public bool restart = false;
    public bool sendInfoToCamArray = true;
    public bool alwaysOnCamera = false;

    /// <summary>
    /// 
    /// </summary>

    private float timeToGo;
    private bool isOkIsFalse = false;
    private float timeStartWithNoEffect;
    //private CameraController cameraController;

    private void Awake()
    {
        //cameraController = GameManager.Instance.CameraObject.GetComponent<CameraController>();
        //else
        //  this.enabled = false;
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }

    void addThisToCam()
    {
        /*if (!cameraController && Camera.main)
            cameraController = Camera.main.transform.gameObject.GetComponent<CameraController>();
        if (!cameraController)
            return;*/
        if (sendInfoToCamArray && !isOk)
        {
            camTarget.enabled = true;
            //cameraController.AddTarget(camTarget);                //add this to Cam !!
        }
        else if (sendInfoToCamArray && isOk)
        {
            camTarget.enabled = false;
            //cameraController.RemoveTarget(camTarget);                //remove this to Cam !!
        }
    }

    /// <summary>
    /// stop imédiatement le timer et set à vrai
    /// </summary>
    public void stopNow()
    {
        timeStartWithNoEffect = Time.fixedTime;
        restart = false;
        isOk = true;
        isOkIsFalse = false;
        addThisToCam();             //remove to target;
    }

    private void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            if (restart)
            {
                isOkIsFalse = false;
                isOk = false;
                restart = false;
            }
            if (!isOk && !isOkIsFalse)                                              //le début
            {
                addThisToCam();             //add to target
                isOkIsFalse = true;
                timeStartWithNoEffect = Time.fixedTime + timeWithEffect;
            }
            if (!isOk && isOkIsFalse && Time.fixedTime >= timeStartWithNoEffect && !alwaysOnCamera)    //la fin
            {
                isOk = true;
                //Debug.Log("ok amazing !");
                isOkIsFalse = false;
                addThisToCam();             //remove to target;
            }
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
