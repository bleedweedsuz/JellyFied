using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class MenuController : MonoBehaviour {
    
    /*List Index for Menu is
     * 
     * 0 => MainMenuPanel
     * 1 => GameLevelPanel
     */
    public Sprite Vol_ON, Vol_OFF;
    public Image volBackImage;
    public AudioSource mainBackAudioSource;

    public List<GameObject> gameObjectList = new List<GameObject>();
    public static int listIndex = 0;
	void Start () {
        setScreen();
        PlayBackAudioSource();
        if (GlobalVariables.ISMainBackAudio)
        {
            volBackImage.sprite = Vol_ON;
        }
        else
        {
            volBackImage.sprite = Vol_OFF;
        }
	}
    public void setScreen()
    {
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (i == listIndex)
            {
                RectTransform mainMenuRectTransform = gameObjectList[i].GetComponent<RectTransform>();
                mainMenuRectTransform.anchoredPosition = Vector2.zero;//set to the main Position
            }
            else
            {
                RectTransform mainMenuRectTransform = gameObjectList[i].GetComponent<RectTransform>();
                mainMenuRectTransform.anchoredPosition = new Vector2(5000,5000);//All the way to up...
            }
        }
    }
    //-------------->
    //For Menu Controller
    public void PlayBtn_OnClick()
    {
        listIndex = 1;
        setScreen();
    }
    public void setScreen_Onclick(int index)
    {
        listIndex = index;
        setScreen();
    }
    //For ScrollBoard
    public void DisableAllNodeInHierarchy(GameObject RootNode)
    {
        //First find the parent of the node
        GameObject MainRootNode = RootNode;
        int count = 0;
        for (int node = 0; node < MainRootNode.transform.childCount; node++)
        {
            if (count % 2 == 1)
            {
                MainRootNode.transform.GetChild(node).gameObject.SetActive(false);
            }
            else
            {
                MainRootNode.transform.GetChild(node).gameObject.GetComponent<NodeController>().setState(false);
            }
            count++;
        }
    }
    public void PlayBackAudioSource()
    {
        if (mainBackAudioSource != null && GlobalVariables.ISMainBackAudio == true)
        {
            mainBackAudioSource.mute = false;
            mainBackAudioSource.Play();
        }
    }
    public void ToggleAudioSource()
    {
        if (GlobalVariables.ISMainBackAudio)
        {
            StopBackAudioSource();
            volBackImage.sprite = Vol_OFF;
        }
        else
        {
            GlobalVariables.ISMainBackAudio = true;
            PlayBackAudioSource();
            volBackImage.sprite = Vol_ON;
        }
    }
    public void StopBackAudioSource()
    {
        if (mainBackAudioSource != null)
        {
            GlobalVariables.ISMainBackAudio = false;
            mainBackAudioSource.Stop();
        }
    }

    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }


}
