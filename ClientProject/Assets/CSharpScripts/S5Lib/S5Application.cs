using UnityEngine;
using System;
using System.Collections;


//The Base Applicaion of Smartron5, to implement some basic functions.
public class S5Application
{
    int m_changeStateNextFrame = -1;                //存要切换的状态,然后下一帧切换
    bool mHasFirstUpdate = false;
    public State mCurGameState { get; private set; }
    private State mOldGameState;

    public int Height { get; set; }
    public int Width { get; set; }

    public MonoBehaviour mCoroutineStarter { private get; set; }				  //Any Exist MonoBehaviour could be a starter, usually is AppLaugher
    public S5Application()
    {
        GameObject obj = GameObject.Find("UI Root");
        UIRoot root = obj.GetComponent<UIRoot>();

        if ((float)Screen.width / Screen.height > 0.667)           //如果长宽比低于2/3， 按长度计算大小(iphone4, ipad等)
        {
            float factor = Screen.height / 960.0f;          //
            root.manualHeight = 960;
            root.maximumHeight = 960;
            root.minimumHeight = 960;

            Width = (int)(Screen.width / factor);
        }
        else                                                //按宽度计算
        {
            float factor = Screen.width / 640.0f;

            root.manualHeight = (int)(Screen.height / factor);
            root.maximumHeight = root.manualHeight;
            root.minimumHeight = root.manualHeight;

            Width = 640;
        }
        Height = root.manualHeight;

        Debug.Log("Width = " + Width + "Height = " + Height);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    protected virtual void ExceptionProcessor(Exception e)
    {
        Debug.LogError("[Application Update] " + e.Message + " " + e.StackTrace);
    }

    public void Init()
    {
        Application.runInBackground = true;
        new UIWindowManager();          //Create UIManager Singleton
        new TextTable();
        new UIDrawer();
        DoInit();
    }

    bool backKeyPress = false;

    public void Update()
    {
        try
        {
            if (mOldGameState != null)
            {
                mOldGameState.DeInitState();         //Release the resources for the gamestate
                mOldGameState = null;

                Resources.UnloadUnusedAssets();
                GC.Collect(2);
            }

            if (!mHasFirstUpdate)
            {
                mHasFirstUpdate = true;
                InitWhenFirstFrame();
            }

            if (mCurGameState != null)
            {
                mCurGameState.Update();
            }

            DoUpdate();

            UIWindowManager.Singleton.Update();
            UIDrawer.Singleton.Update();
        }
        catch (Exception e)
        {
            ExceptionProcessor(e);
        }

        if (!backKeyPress && Input.GetKeyDown(KeyCode.Escape))
        {
            backKeyPress = true;
        }
        if (backKeyPress && Input.GetKeyUp(KeyCode.Escape))
        {
            mCurGameState.OnBackKey();
            backKeyPress = false;
        }
    }

    public void OnGUI()
    {
        if (mCurGameState != null) mCurGameState.OnGUI();
        if (Event.current.type == EventType.Repaint)
        {
            UIWindowManager.Singleton.OnGUI();
        }
        DoOnGUI();
    }

    public void ChangeState(int changeToState)
    {
        m_changeStateNextFrame = changeToState;
    }

    public void StartCoroutine(IEnumerator coroutine)
    {
        if (mCoroutineStarter == null)
        {
            Debug.LogError("Can't start coroutine because mCoroutineStarter undefine");
            return;
        }
        mCoroutineStarter.StartCoroutine(coroutine);
    }

    public void StopCoroutine(IEnumerator coroutine)
    {
        mCoroutineStarter.StartCoroutine(coroutine);
    }

    protected virtual State CreateState(int statEnum) { return null; }

    protected virtual void DoUpdate()
    {
        if (m_changeStateNextFrame > -1)
        {
            if (mCurGameState != null)
            {
                mOldGameState = mCurGameState;         //Save the state
            }
            mCurGameState = CreateState(m_changeStateNextFrame);
            mCurGameState.InitState();
            m_changeStateNextFrame = -1;
        }
    }

    protected virtual void DoInit()
    {

    }

    protected virtual void InitWhenFirstFrame()			//Do The Last Init Stuffs
    {

    }

    protected virtual void DoOnGUI()
    {

    }

    public virtual void OnChangeLevelFinish(int level)
    {

    }

    public virtual void OnApplicationQuit()
    {

    }

    public virtual void OnApplicationPause(bool bPause)
    {

    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadLevelEnumerator(levelName));
    }

    public IEnumerator LoadLevelEnumerator(string levelName)
    {
        AsyncOperation async = Application.LoadLevelAsync(levelName);
        yield return async;

        Debug.Log("Loading complete");
        mCurGameState.OnLoadLevel();
    }
}
