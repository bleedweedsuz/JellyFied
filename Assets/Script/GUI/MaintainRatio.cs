using UnityEngine;
using System.Collections;

public class MaintainRatio : MonoBehaviour {
    public enum RefrenceType { Acc_Width,Acc_Height}
    GameObject MyObject;
    public RefrenceType refrenceType;
    void Start () {
        this.MyObject = this.gameObject;
        switch (refrenceType)
        {
            case RefrenceType.Acc_Width:
                {
                    MyObject.GetComponent<RectTransform>().rect.Set(0, 0, MyObject.GetComponent<RectTransform>().rect.width, MyObject.GetComponent<RectTransform>().rect.width);
                }break;
            case RefrenceType.Acc_Height:
                {
                    MyObject.GetComponent<RectTransform>().rect.Set(0, 0, MyObject.GetComponent<RectTransform>().rect.height, MyObject.GetComponent<RectTransform>().rect.height);
                }break;
            default: break;
        }
	}
}
