using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class NodeController : MonoBehaviour {
    public GameObject RootNode, ChildNode;
    int ID;
    bool isOpen = false;
    void Start()
    {
        ID = Random.Range(0, 400);
        MaintainRatioOfNodeButton();
    }
    void Update()
    {
        MaintainRatioOfNodeButton();
    }
    public void Node_onClick()
    {
        DisableOtherNodeInHierarchy();
        if (isOpen)
        {
            ChildNode.SetActive(false);
            isOpen = false;
        }
        else
        {
            ChildNode.SetActive(true);
            isOpen = true;
        }
        //----------------->Animation States
        AnimationMovement();
    }
    public void setState(bool state)
    {
        isOpen = state;
    }
    public int getID()
    {
        return this.ID;
    }
    void DisableOtherNodeInHierarchy()
    {
        //First find the parent of the node
        GameObject MainRootNode = RootNode.transform.parent.gameObject;
        int count = 0;
        for (int node = 0; node < MainRootNode.transform.childCount;node++ )
        {
            if (count % 2 == 1)
            {
                MainRootNode.transform.GetChild(node).gameObject.SetActive(false);
            }
            else
            {
                if (MainRootNode.transform.GetChild(node).gameObject.GetComponent<NodeController>().getID() != this.getID())
                {
                    MainRootNode.transform.GetChild(node).gameObject.GetComponent<NodeController>().setState(false);
                }
            }
            count++;
        }
    }
    void MaintainRatioOfNodeButton()
    {
        RootNode.GetComponent<LayoutElement>().minWidth= RootNode.GetComponent<RectTransform>().rect.height;
        if (isOpen == true)
        {
            for (int i = 0; i < ChildNode.transform.childCount; i++)
            {
                ChildNode.transform.GetChild(i).gameObject.GetComponent<LayoutElement>().minWidth = ChildNode.GetComponent<RectTransform>().rect.height;
            }
        }
    }
    void AnimationMovement()
    {
        //0->Ideal,1->Animation State Opening,2->Animation State Closing
        Animator animator = this.transform.GetChild(1).GetComponent<Animator>();
        if (animator != null)
        {
            try
            {
                if (animator.GetInteger("currentState") == 2 || animator.GetInteger("currentState") == 0)
                {
                    animator.SetInteger("currentState", 1);
                }
                else if (animator.GetInteger("currentState") == 1 || animator.GetInteger("currentState") == 0)
                {
                    animator.SetInteger("currentState", 2);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
}
