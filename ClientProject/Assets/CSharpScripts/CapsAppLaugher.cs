using UnityEngine;
using System.Collections;

public class CapsAppLaugher : MonoBehaviour {
		
	CapsApplication mApp;
	
	// Use this for initialization
	void Awake() 
	{
        //Debug.Log("Compile Version: " + GolfConfig.Instance.version);
	}
	
	void Start()
	{
        mApp = new CapsApplication();
        mApp.mCoroutineStarter = this;
		DontDestroyOnLoad(GameObject.Find("GlobalObject"));
		mApp.Init();
	}
	
	// Update is called once per frame
	void Update() 
	{
		mApp.Update();
	}
	
	void OnGUI()
	{
        mApp.OnGUI();
	}
	
	void OnLevelWasLoaded(int level)
	{
        mApp.OnChangeLevelFinish(level);
	}

    void OnApplicationQuit()
    {
        if(mApp != null)
        mApp.OnApplicationQuit();
    }

    public void OnApplicationPause(bool pause)
    {
		if(mApp != null)
        	mApp.OnApplicationPause(pause);
    }
}
