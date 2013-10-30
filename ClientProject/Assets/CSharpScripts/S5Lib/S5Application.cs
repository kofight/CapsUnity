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
        Height = 1136;
        Width = Screen.width * 1136 / Screen.height;
    }

    protected virtual void ExceptionProcessor(Exception e)
    {
        Debug.LogError("[Application Update] " + e.Message + " " + e.StackTrace);
    }

    public void Init()
    {
        FingerGestures.OnTap += OnTap;
        FingerGestures.OnDoubleTap += OnDoubleTap;
        FingerGestures.OnDragMove += OnDragMove;
        FingerGestures.OnPinchMove += OnPinchMove;
        FingerGestures.OnTwoFingerDragMove += OnTwoFingerDragMove;
        FingerGestures.OnTwoFingerDragEnd += OnTwoFingerDragMoveEnd;
        FingerGestures.OnDragBegin += OnDrawBegin;
        FingerGestures.OnDragEnd += OnDrawEnd;
        FingerGestures.OnFingerDown += OnPressDown;
        FingerGestures.OnFingerUp += OnPressUp;
        FingerGestures.OnRotationMove += OnRotate;
        FingerGestures.OnRotationEnd += OnRotateEnd;
        FingerGestures.OnLongPress += OnLongPress;
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
            OnBackKey();
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

    #region ControllHandler
    private void OnBackKey()
    {
        mCurGameState.OnBackKey();
    }

    private void OnTap(Vector2 fingerPos)
    {
        
		fingerPos.y = Screen.height - fingerPos.y;

        fingerPos.y = fingerPos.y * Height / Screen.height;
        fingerPos.x = fingerPos.x * Height / Screen.height;

        mCurGameState.OnTap(fingerPos);
    }

    private void OnDoubleTap(Vector2 fingerPos)
    {
        fingerPos.y = Screen.height - fingerPos.y;

        fingerPos.y = fingerPos.y * Height / Screen.height;
        fingerPos.x = fingerPos.x * Height / Screen.height;

        mCurGameState.OnDoubleTap(fingerPos);
    }

    private void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
		fingerPos.y = Screen.height - fingerPos.y;

        fingerPos.y = fingerPos.y * Height / Screen.height;
        fingerPos.x = fingerPos.x * Height / Screen.height;

        delta.x = delta.x * Height / Screen.height;
        delta.y = delta.y * Height / Screen.height;

        mCurGameState.OnDragMove(fingerPos, delta);
    }

    private void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
    {
        mCurGameState.OnPinchMove(fingerPos1, fingerPos2, delta);
    }

    private void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
    {
        mCurGameState.OnTwoFingerDragMove(fingerPos, delta);
    }

    private void OnTwoFingerDragMoveEnd(Vector2 fingerPos)
    {
        mCurGameState.OnTwoFingerDragMoveEnd(fingerPos);
    }

    private void OnLongPress(Vector2 fingerPos)
    {
        mCurGameState.OnLongPress(fingerPos);
    }

    private void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
    {
        mCurGameState.OnRotateEnd(fingerPos1, fingerPos2, totalRotationAngle);
    }

    private void OnDrawBegin(Vector2 fingerPos, Vector2 startPos)
    {
        mCurGameState.OnDrawBegin(fingerPos, startPos);
    }
    private void OnDrawEnd(Vector2 fingerPos)
    {
        mCurGameState.OnDrawEnd(fingerPos);
    }
    private void OnPressDown(int fingerIndex, Vector2 fingerPos)
    {
		fingerPos.y = Screen.height - fingerPos.y;
        fingerPos.y = fingerPos.y * Height / Screen.height;
        fingerPos.x = fingerPos.x * Height / Screen.height;
        mCurGameState.OnPressDown(fingerIndex, fingerPos);
    }
    private void OnPressUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
    {
        mCurGameState.OnPressUp(fingerIndex, fingerPos, timeHeldDown);
    }
    private void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
    {
        mCurGameState.OnRotate(fingerPos1, fingerPos2, rotationAngleDelta);
    }
    #endregion
}
