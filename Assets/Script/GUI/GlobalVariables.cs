using UnityEngine;
using System.Collections;

public class GlobalVariables : MonoBehaviour {
    public static bool ISMainBackAudio;
    private static int count;
    public static int currentLevel = 0;
    public enum MessageButtonEventModule
    {
        Exit,Debug
    }
    public void Awake()
    {
        if (count == 0)
        {
            ISMainBackAudio = true;
            count++;
        }
    }
}
