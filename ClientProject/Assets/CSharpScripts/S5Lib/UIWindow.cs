using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 168,414,219

/// <summary>
/// Use to define a UIEffect, need implement in derived class
/// Drag it to UI prefab to use it
/// </summary>
public abstract class UIEffectPlayer : MonoBehaviour
{
    public enum PlayingExcludeEffectType
    {
        None,
        HideEffect,
        ShowEffect,
    }
    public virtual bool IsPlaying(PlayingExcludeEffectType effectType) { return mCurPlayingExcludeEffect == effectType; }
    public virtual void OnCreate() { }
    public virtual void OnShow()
    {
        mCurPlayingExcludeEffect = PlayingExcludeEffectType.ShowEffect;
    }
    public virtual void OnHide()
    {
		if(enabled)
        	mCurPlayingExcludeEffect = PlayingExcludeEffectType.HideEffect;
    }
    public virtual void OnUpdate() { }
    public virtual void ResetWidget() { }

    public bool PlayWhileShowWindow = true;
    public bool PlayWhileHideWindow = true;
    protected PlayingExcludeEffectType mCurPlayingExcludeEffect;
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
public abstract class UIWindow
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

    public virtual void SetAnchor(GameObject UIAnchor) { }
    List<UIEffectPlayer> mEffectPlayerList = new List<UIEffectPlayer>();
    public UIWindowStateEnum uiWindowState = UIWindowStateEnum.Hide;
    public delegate void WindowEffectFinished();
    private WindowEffectFinished m_hideFinishEffect;
    private WindowEffectFinished m_showFinishEffect;

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

        bool bHideEffect = false;
        foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            if (player.PlayWhileHideWindow && player.gameObject.activeInHierarchy)
            {
                player.OnHide();
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

    public virtual void ShowWindow()
    {
        if (uiWindowState == UIWindowStateEnum.Show || uiWindowState == UIWindowStateEnum.PlayingShowEffect)
        {
            return;
        }

        DoShowWindow();
        
        bool bShowEffect = false;
        foreach (UIEffectPlayer player in mEffectPlayerList)
        {
            if (player.PlayWhileShowWindow && player.gameObject.activeInHierarchy)
            {
                player.OnShow();
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


    protected void ResetWidget()
    {
        mEffectPlayerList.ForEach( ( e ) => e.ResetWidget() );
    }

    public void ShowWidget(string name)
    {
        Transform trans = UIToolkits.FindChild(mUIObject.transform, name);
        if (trans == null)
        {
            return;
        }
        UIEffectPlayer player = trans.gameObject.GetComponent<UIEffectPlayer>();
        if (player != null)
        {
            player.OnShow();
        }
        trans.gameObject.SetActive(true);
    }

    public void HideWidget(string name)
    {
        Transform trans = UIToolkits.FindChild(mUIObject.transform, name);
        if (trans == null)
        {
            return;
        }
        UIEffectPlayer player = trans.gameObject.GetComponent<UIEffectPlayer>();
        if (player != null)
        {
            player.OnHide();
        }
        trans.gameObject.SetActive(false);
    }

    protected virtual void DoHideWindow() { }
    protected virtual void DoShowWindow() { }

    public virtual void ReleaseWindow() 
    {
        mEffectPlayerList.Clear();
        GameObject.Destroy(mUIObject);
        mUIObject = null;
    }

    public virtual void OnHideEffectPlayOver() 
    {
        if (m_hideFinishEffect != null)
        {
            m_hideFinishEffect();
            m_hideFinishEffect = null;
        }
        uiWindowState = UIWindowStateEnum.Hide;
    }

    public virtual void OnShowEffectPlayOver() 
    {
        if (m_showFinishEffect != null)
        {
            m_showFinishEffect();
            m_showFinishEffect = null;
        }
        uiWindowState = UIWindowStateEnum.Show; 
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
            player.OnCreate();
        }
		
		UIEffectPlayer[] players = mUIObject.transform.GetComponents<UIEffectPlayer>();
        foreach (UIEffectPlayer player in players)
        {
            player.OnCreate();
            mEffectPlayerList.Add(player);
        }
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
                if (player.transform.gameObject.activeInHierarchy 						//防止transform被停止,player没停止的情况
                && player.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.HideEffect))
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
            if (prefabName == "UIGameMenu")
            {
                string sss = string.Empty;
            }
            bool stillPlaying = false;
            foreach (UIEffectPlayer player in mEffectPlayerList)
            {
                if (player.gameObject.activeSelf && player.IsPlaying(UIEffectPlayer.PlayingExcludeEffectType.ShowEffect))
                {
                    stillPlaying = true;
                }
            }
            if (!stillPlaying)
            {
                OnShowEffectPlayOver();
            }
        }
    }
    public UIMouseClick AddChildComponentMouseClick( string name , System.EventHandler<UIMouseClick.ClickArgs> action )
     {
        Transform tansform = UIToolkits.FindChild(mUIObject.transform, name);
        if( null == tansform )
         {
            Debug.LogError(" tansform is null .");
             return null;
         }
        return UIToolkits.AddChildComponentMouseClick(tansform.gameObject, action);
    }

    public UIMouseHover AddChildComponentMouseHover( string name , System.EventHandler<UIMouseHover.HoverArgs> action )
     {
          Transform tansform = UIToolkits.FindChild( mUIObject.transform , name );
         if( null == tansform )
         {
             Debug.LogError( " tansform is null ." );
             return null;
         }
        UIMouseHover hover = tansform.gameObject.AddComponent<UIMouseHover>();
        return UIToolkits.AddChildComponentMouseHover(tansform.gameObject, action);
    }

    public T GetChildComponent<T>(string name) where T : Component
    {
        Transform tansform = UIToolkits.FindChild(mUIObject.transform, name);
        if (null == tansform)
        {
            return null;
        }
        return tansform.GetComponent<T>();
    }
    public virtual Rect GetChildRect(string name) { return new Rect(); }
	/// <summary>
	/// Position in screen  
	/// </summary>
    public virtual Rect GetRect() { return new Rect(); }

    public virtual void SetPosition(Vector2 pos) { }
}
