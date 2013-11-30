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
    public virtual bool IsPlaying() { return false; }           //是否正在播放特效
    public virtual void CreateEffect() { }                      //创建特效
    public virtual void ShowEffect() { }                        //显示时的特效
    public virtual void HideEffect() { }                        //隐藏时的特效
	public virtual void Update(){}								//

    public bool PlayWhileShowWindow = true;
    public bool PlayWhileHideWindow = true;
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
            player.CreateEffect();
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
                && player.IsPlaying())
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
                if (player.gameObject.activeSelf && player.IsPlaying())
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
    }
}
