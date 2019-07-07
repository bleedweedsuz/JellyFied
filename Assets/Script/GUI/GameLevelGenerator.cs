using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SQLite4Unity3d;
using System;

public class GameLevelGenerator : MonoBehaviour {

    DBService ds;
    public List<Sprite> levelSprite = new List<Sprite>();
    public GameObject gameObjectIni;
    public GameObject backBtnObject;
    public GameObject scrollViewPanel;
    public List<CreateLevel> levelList = new List<CreateLevel>();
    public int totalLevel=0;
    int currentLevel = 0;
    void Start () {
        this.currentLevel = GlobalVariables.currentLevel;
        LevelGenerator();
	}
    void LevelGenerator()
    {
        levelList.Clear();
        foreach (Transform child in scrollViewPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //
        GameObject backBtn = GameObject.Instantiate(backBtnObject);
        backBtn.transform.SetParent(scrollViewPanel.transform);
        try
        {
            ds = new DBService();
            IEnumerable<LevelScore> lList = ds.getConnection().Table<LevelScore>();
            foreach (LevelScore lScore in lList)
            {
                totalLevel++;
                bool isLock = lScore.Lock;
                int score = lScore.LastScore;
                CreateLevel level = new CreateLevel(isLock, totalLevel, score, gameObjectIni, levelSprite);
                level.getGeneratedLevel().transform.SetParent(scrollViewPanel.transform);
                //Adding to the list
                levelList.Add(level);
            }
        }
        catch (Exception ex)
        {
            print(ex.ToString());
        }
        finally
        {
            ds.DisConnect();
        }
    }
    void Update()
    {
        try
        {
            //Update on gameList Gameobject
            for (int i = 0; i < levelList.Count; i++)
            {
                if (levelList[i].isAsyncOperationBegin)
                {
                    MenuController mController = this.GetComponent<MenuController>();
                    mController.setScreen_Onclick(3);
                    if (levelList[i].asyncOperation.isDone)
                    {
						//-->
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    //----------->
    public class CreateLevel
    {
        public bool isAsyncOperationBegin = false;
        public AsyncOperation asyncOperation;
        List<Sprite> levelSprite;
        bool isLock;
        int levelName;
        int score;
        Sprite backImage;
        GameObject generatedGameObj;
        GameObject refrenceObject;
        public CreateLevel(bool isLock,int levelName,int score,GameObject gameObjectIni,List<Sprite> levelSprite)
        {
            this.isLock = isLock;
            this.levelName = levelName;
            this.score = score;
            this.refrenceObject = gameObjectIni;
            this.levelSprite = levelSprite;
        }
        public GameObject getGeneratedLevel()
        {
            generatedGameObj = GameObject.Instantiate(refrenceObject);//Duplicating the object
            //OnClick Event
            Button levelButton = generatedGameObj.GetComponent<Button>();
            levelButton.onClick.AddListener(() => onClick(this.levelName));
            //Set Text
            generatedGameObj.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text=levelName.ToString();
            //Set Score Image Visible
            int count=0;
            for (int i = 1; i <= 3; i++)
            {
                count++;
                if (count <= score)
                {
                    generatedGameObj.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(1f,1f,1f,1f);
                }
                else
                {
                    generatedGameObj.transform.GetChild(1).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
                }
            }
            //Set Lock Level
            if (isLock)
            {
                generatedGameObj.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "";
                generatedGameObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = levelSprite[0];
            }
            else
            {
                generatedGameObj.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = Color.white;
                generatedGameObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = levelSprite[1];
            }
            
            return generatedGameObj;
        }
        void onClick(int level)
        {
           //Select The Scene According to data
            if (isLock)
            {   
                //if the level is lock then show animation 
                Debug.Log("You cann't play this level");
            }
            else
            {
                try
                {
                    GlobalVariables.currentLevel = level;
                    asyncOperation = Application.LoadLevelAsync("Level" + this.levelName);
					asyncOperation.allowSceneActivation=true;
                    isAsyncOperationBegin = true;
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }
}
