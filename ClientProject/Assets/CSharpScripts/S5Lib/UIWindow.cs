using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 168,414,219

/// <summary>
/// Use to define a UIEffect, need implement in derived class
/// Drag it to UI prefab to use it
/// </summary>
/// 

public enum EffectState
{
    Delay,
    Showing,
    Idle,
    Hiding,
}

public abstract class UIEffectPlayer : MonoBehaviour
{
    public virtual bool IsPlaying() { return false; }           //是否正在播放特效
    public virtual void CreateEffect() { }                      //创建特效
    public bool IsDelaying() { return m_state == EffectState.Showing && m_delayStartTime > 0; }
    public float StateTimeOut = 1;                              //切到某状态后状态失效时间(用来处理idle动画无法正确结束的问题)
    protected float m_curStateStartTime = 0;
    UIWindow.WindowEffectFinished m_finishedFunc;

    public void ShowEffect()                        //显示时的特效
    {
        if (Delay == 0)
        {
            m_state = EffectState.Showing;
            m_curStateStartTime = Timer.GetFixedTime();
			DoShowEffect();
        }
        else
        {
            m_state = EffectState.Delay;
            m_curStateStartTime = Timer.GetFixedTime();
            gameObject.SetActive(false);
            m_delayStartTime = Time.realtimeSinceStartup;
        }
    }

    public virtual void HideEffect(UIWindow.WindowEffectFinished finishedFunc = null)        //隐藏时的特效
    {
        m_state = EffectState.Hiding;
        m_curStateStartTime = Timer.GetFixedTime();
        DoHideEffect();
        m_finishedFunc = finishedFunc;
    }            

    protected virtual void DoShowEffect() { }
    protected virtual void DoHideEffect() { }
    protected virtual void DoIdleEffect() { }       

    public virtual void Update()            //更新
    {
        if (m_state == EffectState.Delay)
        {
            if (Time.realtimeSinceStartup - m_delayStartTime > Delay)                         //若Delay时间已到
            {
                m_state = EffectState.Showing;                                                //切显示特效状态
                m_curStateStartTime = Timer.GetFixedTime();
                m_delayStartTime = 0;
                gameObject.SetActive(true);                                                   //把自己显示出来
                DoShowEffect();                                                               //播放显示特效
            }
        }

        if (m_state == EffectState.Showing && !IsPlaying())     //若显示特效状态已经结束
        {
            m_state = EffectState.Idle;                         //进入Idle状态
            m_curStateStartTime = Timer.GetFixedTime();
            DoIdleEffect();                                     //播放Idle状态特效
        }

        if (m_finishedFunc != null && m_state == EffectState.Hiding && !IsPlaying())      //播放结束响应
        {
            UIWindow.WindowEffectFinished func = m_finishedFunc;
            m_finishedFunc = null;
            func();
        }
    }

	public float Delay = 0.0f;
    
    public bool PlayWhileShowWindow = true;
    public bool PlayWhileHideWindow = true;

    protected float m_delayStartTime = 0.0f;                              //开始计算Delay的时间
    protected EffectState m_state;
}

public enum UIWindowStateEnum
{
    Hide,
    PlayingShowEffect,
    Show,
    PlayingHideEffect,
}

/// <summary>
/// All UI's base class, provide some common behavior of Window
/// </summary>
public class UIWindow
{
    public string prefabName;
    public GameObject mUIObject;
    public UIWindowManager.Anchor mAnchor;
    public int mUIFlag = 0;
    public bool CheckWindowFlag(int flag) { return (mUIFlag & flag) > 0; }
    public bool Visible
    {
        get { return uiWindowState != UIWindowStateEnum.Hide; }
        set
        {
            if (value)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
        }
    }

    protected List<UIEffectPlayer> mEffectPlayerList = new List<UIEffectPlayer>();
    public UIWindowStateEnum uiWindowState = UIWindowStateEnum.Hide;
    public delegate void WindowEffectFinished();
    private WindowEffectFinished m_hideFinishEffect;
    private WindowEffectFinished m_showFinishEffect;
    List<Collider> m_colliders = new List<Collider>();

    public void HideWindowWithoutInvoke()
    {
        DoHideWindow();
    }

    public void HideWindow(WindowEffectFinished finishEffectFunc)
    {
        m_hideFinishEffect = finishEffectFunc;
        HideWindow();
    }

    public void ShowWindow(WindowEffectFinished finishEffectFunc)
    {
        m_showFinishEffect = finishEffectFunc;
        ShowWindow();
    }

    public virtual void HideWindow()
    {
        if (uiWindowState == UIWindowStateEnum.Hide || uiWindowState == UIWindowStateEnum.PlayingHideEffect)
        {
            return;
        }
		
		if(uiWindowState == UIWindowStateEnum.PlayingShowEffect)
		{
			OnShowEffectPlayOver();
		}

		EnableColliders(false);

        bool bHideEffect = false;
        foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            if (player.PlayWhileHideWindow)
            {
                player.HideEffect();
				bHideEffect = true;
            }
        }
        if (bHideEffect)        //If there is hide effect, don't hide window here, hide window when hide effect play over
        {
            uiWindowState = UIWindowStateEnum.PlayingHideEffect;
        }
        else
        {
            uiWindowState = UIWindowStateEnum.Hide;
            DoHideWindow();
        }
        OnHide();

        if (!bHideEffect)
        {
            OnHideEffectPlayOver();
        }
    }

    void EnableColliders(bool val)      //启用或隐藏碰撞体  
    {
        foreach (Collider collider in m_colliders)
        {
            collider.enabled = val;
        }
    }

    public virtual void ShowWindow()
    {
        if (uiWindowState == UIWindowStateEnum.Show || uiWindowState == UIWindowStateEnum.PlayingShowEffect)
        {
            return;
        }
		
		if(uiWindowState == UIWindowStateEnum.Hide)
		{
			OnHideEffectPlayOver();
		}

        DoShowWindow();
        
        bool bShowEffect = false;
        foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            if (player.PlayWhileShowWindow)
            {
                player.ShowEffect();
				bShowEffect = true;
            }
        }
        
        if (bShowEffect)
        {
            uiWindowState = UIWindowStateEnum.PlayingShowEffect;
        }
        else
        {
            uiWindowState = UIWindowStateEnum.Show;
        }
        
        OnShow();
        UpdateUIData();
        UpdateLanguage();

        if (!bShowEffect)
        {
            OnShowEffectPlayOver();
        }
    }

    public virtual void ReleaseWindow() 
    {
        mEffectPlayerList.Clear();
        GameObject.Destroy(mUIObject);
        mUIObject = null;
    }

    public virtual void OnHideEffectPlayOver() 
    {
		uiWindowState = UIWindowStateEnum.Hide;
		EnableColliders(true);
        if (m_hideFinishEffect != null)
        {
			WindowEffectFinished func = m_hideFinishEffect;         //这种写法是为了防止循环调用,若在Func里面再调用到OnHideEffectPlayOver时，不重复执行func
			m_hideFinishEffect = null;
            func();
        }
    }

    public virtual void OnShowEffectPlayOver() 
    {
		uiWindowState = UIWindowStateEnum.Show; 
        if (m_showFinishEffect != null)
        {
            WindowEffectFinished func = m_showFinishEffect;         //这种写法是为了防止循环调用,若在Func里面再调用到OnShowEffectPlayOver时，不重复执行func
            m_showFinishEffect = null;
			func();
        }
    }

    public virtual void OnDestory() { }
    public virtual void OnCreate() 
    {
    }

    public void AfterCreate()
    {
        UIToolkits.FindComponents<UIEffectPlayer>(mUIObject.transform, mEffectPlayerList);
        foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            player.CreateEffect();
        }
        UIToolkits.FindComponents<Collider>(mUIObject.transform, m_colliders);
    }
    public virtual void OnHide() { }
    public virtual void OnShow() { }
    public virtual void UpdateUIData() { }
    public virtual void UpdateLanguage() { }
    public virtual void OnGUI() { }
    public virtual void OnUpdate() 
    {
        if (uiWindowState == UIWindowStateEnum.PlayingHideEffect)
        {
            bool stillPlaying = false;
            foreach (UIEffectPlayer player in mEffectPlayerList)
            {
                if (player.IsDelaying() || (player.gameObject.activeInHierarchy && player.IsPlaying()))
                {
                    stillPlaying = true;
                    break;
                }
            }
            if (!stillPlaying)
            {
                DoHideWindow();
                OnHideEffectPlayOver();
            }
        }

        if (uiWindowState == UIWindowStateEnum.PlayingShowEffect)
        {
            bool stillPlaying = false;
            foreach (UIEffectPlayer player in mEffectPlayerList)
            {
                if (player.IsDelaying() || (player.gameObject.activeInHierarchy && player.IsPlaying()))
                {
                    stillPlaying = true;
                }
            }
            if (!stillPlaying)
            {
                OnShowEffectPlayOver();
            }
        }
		
		foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            player.Update();
        }
    }
    public bool AddChildComponentMouseClick( string name , EventDelegate.Callback callBack )
     {
        return UIToolkits.AddChildComponentMouseClick(mUIObject.transform, name, callBack);
    }

    public T GetChildComponent<T>(string name) where T : Component
    {
        return UIToolkits.FindComponent<T>(mUIObject.transform, name);
    }
    public Rect GetChildRect(string name)
    {
        if (mUIObject == null)
        {
            return new Rect();
        }
        Transform trans = UIToolkits.FindChildCheckActive(mUIObject.transform, name);
        if (trans == null)
        {
            return new Rect();
        }
        return UIToolkits.GetUIObjectScreenRect(trans);
    }
	/// <summary>
	/// Position in screen  
	/// </summary>
    public Rect GetRect() { return UIToolkits.GetUIObjectScreenRect(mUIObject.transform); }

    public void SetAnchor(GameObject UIAnchor)
    {
        float saveZ = mUIObject.transform.localPosition.z;
        mUIObject.transform.parent = UIAnchor.transform;

        mUIObject.transform.localPosition = new Vector3(0, 0, saveZ);
        mUIObject.transform.localScale = Vector3.one;
    }

    protected void DoHideWindow()
    {
        mUIObject.SetActive(false);
    }

    protected void DoShowWindow()
    {
        mUIObject.SetActive(true);
        EnableColliders(true);
    }
}
