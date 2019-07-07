using UnityEngine;
using System.Collections;
using System;

public class InitDB : MonoBehaviour {
    void Awake()
    {
        InitializeDatabase();
    }
    void InitializeDatabase()
    {
        DBService db = null;
        try
        {
            db = new DBService();
            db.CreateDBAndInsertNecessaryData();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
            db.DisConnect();
        }
    }
}
