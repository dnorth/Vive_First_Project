using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObj;
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

    void FixedUpdate()
    {
        if(Controller.GetHairTriggerDown())
        {
            if(collidingObj)
            {
                GrabObject();
            }
        }

        if(Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    private void GrabObject()
    {
        objectInHand = collidingObj;
        collidingObj = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
	
	public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if(!collidingObj)
        {
            return;
        }

        collidingObj = null;
    }

    private void SetCollidingObject(Collider col)
    {
        if(collidingObj || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObj = col.gameObject;
    }
}
