using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MessageBox : MonoBehaviour {
    public static bool isMsgBoxAlreadyOpen=false;
    public GameObject MessageBoxTemplate_ref, Root;
    GameObject CancelBtn, MessageBoxTemplate;
    public GlobalVariables.MessageButtonEventModule messageButtoneventModule;
    public string Title, Message;
    public MessageBox(GlobalVariables.MessageButtonEventModule messageButtoneventModule, GameObject Root, string Title, string Message)
    {
        this.messageButtoneventModule = messageButtoneventModule;
        this.Root = Root;
        this.Title = Title;
        this.Message = Message;
    }
    public void DisplayMessageBox()
    {
        if (isMsgBoxAlreadyOpen) { return; }
        isMsgBoxAlreadyOpen = true;
        //Duplicate template
        MessageBoxTemplate = GameObject.Instantiate(MessageBoxTemplate_ref);
        //OverLay Scaling
        MessageBoxTemplate.transform.GetChild(0).localScale = new Vector3(30, 30, 0);
        //Set Title
        MessageBoxTemplate.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = Title;
        //Set Message
        MessageBoxTemplate.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = Message;
        //Ok Btn
        MessageBoxTemplate.transform.GetChild(3).transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => onOkBtn_Click());
        //Cancel Btn
        MessageBoxTemplate.transform.GetChild(3).transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => onCancelBtn_Click());
        //Set No Margin in MessageBox
        RectTransform panelRectTransform = MessageBoxTemplate.GetComponent<RectTransform>();
        panelRectTransform.position = Root.transform.position;
        //panelRectTransform.rect.Set(0, 0, Screen.width, Screen.height);
        //MessageBoxTemplate.GetComponent<RectTransform>(). = Root.GetComponent<RectTransform>();
        //Add Duplicate Data to Parent Root
        MessageBoxTemplate.transform.SetParent(Root.transform);
    }
    void onOkBtn_Click()
    {
        switch (messageButtoneventModule)
        {
            case GlobalVariables.MessageButtonEventModule.Debug:
                {
                    
                    onCancelBtn_Click();
                }break;
            default: break;
        }
    }
    void onCancelBtn_Click()
    {
        isMsgBoxAlreadyOpen = false;
        GameObject.Destroy(MessageBoxTemplate);
    }
}
