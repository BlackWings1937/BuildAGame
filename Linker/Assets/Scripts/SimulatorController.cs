using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SimulatorController : MonoBehaviour {

    private Process myProcessXbl_ = null;

	// Use this for initialization
	void Start () {
        string str = System.Environment.CurrentDirectory;
        UnityEngine.Debug.Log(str);
        myProcessXbl_ = Process.Start(str+"/newMainScene2/xiaobanlong.exe");
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 200, 150, 50), "Stop Cale"))
        {
            myProcessXbl_.Kill();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
