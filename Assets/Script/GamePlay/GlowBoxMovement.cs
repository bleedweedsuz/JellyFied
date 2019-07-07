using UnityEngine;
using System.Collections;

public class GlowBoxMovement : MonoBehaviour {

    public GameObject parentObject;
    public float lerpTimeVal = 0.08f;
    public float offsetY = 0.2f;
    bool isAnimationFloatingOn = false;
    Vector3 oldVector3,newVector3;
    
    void Update () {
        try
        {
            AnimationFollower();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }
	}
    void AnimationFollower()
    {
        if (!isAnimationFloatingOn)
        {
            this.transform.position = Vector3.Lerp(transform.position, parentObject.transform.position, lerpTimeVal);
            this.transform.position += new Vector3(0, offsetY, 0);
            oldVector3 = parentObject.transform.position;
            if (oldVector3.Equals(newVector3) == true && ((int)oldVector3.x).Equals((int)transform.position.x) == true && ((int)oldVector3.z).Equals((int)transform.position.z) == true)
            {
                //Player Stop
                isAnimationFloatingOn = true;
            }
            newVector3 = oldVector3;
        }
        else
        {
            oldVector3 = parentObject.transform.position;
            if (oldVector3.Equals(newVector3) == false)
            {
                //Player Move
                isAnimationFloatingOn = false;
            }
            newVector3 = oldVector3;
        }
    }
}
