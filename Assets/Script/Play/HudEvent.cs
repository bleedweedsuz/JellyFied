using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
public class HudEvent : MonoBehaviour {
	public static bool isGamepause;
	public static bool isBackground = true;
	public static bool isSFX = true;
	AudioSource BackGroundMusic;
	public static int totalCollectionObject;
	public static int CollectorCount = 0;
	public GameObject CollectorObjectCollection;
	public List<Sprite> GUIImageList = new List<Sprite>();
	/* Image List
	 * 0=>Pause GUI
	 * 1=>Game Over GUI
	 */
	public List<Sprite> ScoreOnList = new List<Sprite>();
	void Start(){
		CountCollectorObject ();
		isGamepause=false;
		BackGroundMusic = gameObject.GetComponent<AudioSource> ();
	}
	GameObject TopHudObj;
	public void Hud_SetTopHudGameObject(GameObject TopHudObj){
		this.TopHudObj = TopHudObj;
	}
	public void Hud_PauseToggle(GameObject toggleGameObject){
		//Toggle Pause
		isGamepause = isGamepause==true?false:true;	
		//----Toggle GameObject
		if (isGamepause) {
			//Show ToggleGameObject
			toggleGameObject.SetActive(true);
			TopHudObj.gameObject.SetActive(false);
			toggleGameObject.transform.GetChild(0).gameObject.SetActive(true);
			toggleGameObject.transform.GetChild(1).gameObject.SetActive(false);
			//-------------->
			toggleGameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GUIImageList[0];
			toggleGameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
		}
		else {
			//Hide ToggleGameObject
			toggleGameObject.SetActive(false);
			TopHudObj.gameObject.SetActive(true);
		}
	}
	public void Hud_TopHudToggle(GameObject toggleGameObject){
		//Toggle Pause
		isGamepause = isGamepause==true?false:true;	
		//----Toggle GameObject
		if (isGamepause) {
			//Show ToggleGameObject
			toggleGameObject.SetActive(true);
			TopHudObj.gameObject.SetActive(false);
			toggleGameObject.transform.GetChild(0).gameObject.SetActive(true);
			toggleGameObject.transform.GetChild(1).gameObject.SetActive(false);
			//-------------------->
			toggleGameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GUIImageList[1];
			toggleGameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
		}
		else {
			//Hide ToggleGameObject
			toggleGameObject.SetActive(false);
			TopHudObj.gameObject.SetActive(true);
		}
	}
	public void Hud_FinishedToggle(GameObject toggleGameObject){
		//Toggle Pause
		isGamepause = isGamepause == true ? false : true;	
		//----Toggle GameObject
		if (isGamepause) {
           
			//Show ToggleGameObject
			toggleGameObject.SetActive (true);
			TopHudObj.gameObject.SetActive (false);
			toggleGameObject.transform.GetChild (0).gameObject.SetActive (false);
			toggleGameObject.transform.GetChild (1).gameObject.SetActive (true);
			//---LOGIC MAKER GAME FINISHED
			Text Scorelabel = toggleGameObject.transform.GetChild (1)
			    	.transform.GetChild(0).transform.GetChild(1)
					.transform.GetChild(1).gameObject.GetComponent<Text>();
			Scorelabel.text = LogData();
			int countStar = ((3 * CollectorCount) / totalCollectionObject) ;
			//--------------------------->
			Image Score1 = toggleGameObject.transform.GetChild (1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
			Image Score2 = toggleGameObject.transform.GetChild (1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Image>();
			Image Score3 = toggleGameObject.transform.GetChild (1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<Image>();
			//--------------------------->
			if(countStar == 1){
				Score1.sprite = ScoreOnList[0];
			}
			else if(countStar == 2){
				Score1.sprite = ScoreOnList[0];
				Score2.sprite = ScoreOnList[1];
			}
			else if(countStar == 3){
				Score1.sprite = ScoreOnList[0];
				Score2.sprite = ScoreOnList[1];
				Score3.sprite = ScoreOnList[2];
			}
            DBService db = new DBService();
            try
            {
                //Update this session data
                db.getConnection().Execute("update `LevelScore` set `LastScore` = ? where `LevelId`= ? ",countStar,GlobalVariables.currentLevel);
                if (countStar >= 2)
                {
                    db.getConnection().Execute("update `LevelScore` set `Lock` = ? where `LevelId`= ? ", false, GlobalVariables.currentLevel + 1);
                }
            }
            catch (Exception ex)
            {
                print(ex.ToString());
            }
            finally
            {
                if (db != null)
                {
                    db.DisConnect();
                }
            }

		}
	}
	public void Hud_Home(){
		AsyncOperation asyncOperation;
		try
		{
			MenuController.listIndex = 0;
			asyncOperation = Application.LoadLevelAsync("Menu");
			asyncOperation.allowSceneActivation=true;
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}
	public void ReStartApp(){
		AsyncOperation asyncOperation;
		try
		{
			string levelName = Application.loadedLevelName;
			asyncOperation = Application.LoadLevelAsync(levelName);
			asyncOperation.allowSceneActivation=true;
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}
	public void Hud_Level(){
		AsyncOperation asyncOperation;
		try
		{
			MenuController.listIndex = 1;
			asyncOperation = Application.LoadLevelAsync("Menu");
			asyncOperation.allowSceneActivation=true;
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}
	void CountCollectorObject(){
		int count = CollectorObjectCollection.transform.childCount;
		totalCollectionObject = count;
	}
	public static void IncrementCollectorData(){
		CollectorCount ++;
	}
	public static string LogData(){
		//For Collector Val
		string cVal = "";
		if (CollectorCount < 10) {
			cVal = "0" + CollectorCount;
		}
		else {
			cVal = CollectorCount.ToString();
		}
		//For TotalCollector Val
		string tVal = "";
		if (totalCollectionObject < 10) {
			tVal = "0" + totalCollectionObject;
		}
		else {
			tVal = totalCollectionObject.ToString();
		}
		//---LOG SET DATA
		return cVal + "/" + tVal;
	}
	public static void ResetCollectorData(){
		CollectorCount = 0;
	}
}
