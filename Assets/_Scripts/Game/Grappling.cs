using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float secondeBeforeDetach = 1f;
    //[FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    //private FrequencyCoolDown coolDown = new FrequencyCoolDown();
    //[FoldoutGroup("GamePlay"), Tooltip("list des layer à ignorer"), SerializeField]
    //private List<GameData.Layers> listLayerToIgnore;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private SpringJoint springJoint;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private LineRenderer line;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool attached = false;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private Transform target;

    private void Awake()
    {
        line.SetPosition(0, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target)
            return;
            
        if (other.CompareTag(GameData.Prefabs.Player.ToString())/* && coolDown.IsReady()*/)
        {
            Attach(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!target)
            return;

        if (other.CompareTag(GameData.Prefabs.Player.ToString()))
        {
            attached = false;
            Invoke("Detach", secondeBeforeDetach);
            //coolDown.StartCoolDown();
        }
    }

    /// <summary>
    /// attache
    /// </summary>
    /// <param name="other"></param>
    private void Attach(Collider other)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, other.transform.position, out hit))
        {
            if (hit.transform.CompareTag(GameData.Prefabs.Player.ToString()))
            {
                //SoundManager.GetSingleton.playSound(GameData.Sounds.SpiksOn.ToString() + transform.GetInstanceID().ToString());
                CancelInvoke("Detach");
                springJoint.connectedBody = other.GetComponent<Rigidbody>();
                attached = true;
                line.enabled = attached;
                target = other.transform;
            }
        }
    }

    /// <summary>
    /// détacher
    /// </summary>
    private void Detach()
    {
        if (!attached)
        {
            //SoundManager.GetSingleton.playSound(GameData.Sounds.SpiksOff.ToString() + transform.GetInstanceID().ToString());
            springJoint.connectedBody = null;
            line.enabled = attached;
            target = null;
        }
    }

    private void Update()
    {
        if (line.enabled && target != null)
        {
            line.SetPosition(1, target.position);
        }
    }
}
