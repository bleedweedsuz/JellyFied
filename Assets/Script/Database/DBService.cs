using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System;
public class DBService : MonoBehaviour {
    public string DatabaseName = "Jellified.sqlite";
    SQLiteConnection connection;
    public DBService()
    {
        #if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/Database/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            //CreateDBAndInsertNecessaryData();
            //Debug.Log("Final PATH: " + dbPath);
    }
    public void DisConnect()
    {
        if (connection != null)
        {
            connection.Close();
            connection = null;
        }
    }
    public SQLiteConnection getConnection()
    {
        return connection;
    }
    public void CreateDBAndInsertNecessaryData()
    {
        connection.CreateTable<LevelScore>();//Create Table
        try
        {
            if (connection.Table<LevelScore>().Count() <= 0)
            {
                //Insert All The Necessary Data Into Table
                connection.InsertAll(new[]{
                    new LevelScore(){
                        LevelName = "Begginer Level",LastScore = 0,Lock = false
                    },
                     new LevelScore(){
                        LevelName = "Candy Mania",LastScore = 0,Lock = true
                    },
                     new LevelScore(){
                        LevelName = "Dark Valley",LastScore = 0,Lock = true
                    },
                     new LevelScore(){
                        LevelName = "Chocolate War",LastScore = 0,Lock = true
                    },
                     new LevelScore(){
                        LevelName = "Soda Pop",LastScore = 0,Lock = true
                    },
                     new LevelScore(){
                        LevelName = "Jelly Land",LastScore = 0,Lock = true
                    }
                });
                //Debug.Log("OK --- First Initilization");
            }
            else
            {
                //Debug.Log("OK -- EXISTS OLD DATA");
            }
        }
        catch (Exception ex)
        {
            print(ex.ToString());
        }
    }
}
