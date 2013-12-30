using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//An Manager To Manage All UI And Window
public class UIWindowManager
{
    #region Singleton
    public static UIWindowManager Singleton { get; private set; }
    public UIWindowManager()
    {
        if (Singleton == null)
        {
            Singleton = this;
            OnCreate();
        }
        else
        {
            throw new System.Exception();			//if singleton exist, throw a Exception
        }
    }
    #endregion
    public GameObject[] AnchorObject = new GameObject[9];
    public enum Anchor
    {
        Center = 0,
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
    }
    private Dictionary<string, UIWindow> mWindowMap = new Dictionary<string, UIWindow>();
    public delegate bool UIClickProcesser(GameObject clickObj);
    public UIClickProcesser ClickProcesser = null;
    public delegate bool UIDoubleClickProcesser(GameObject clickObj);
    public UIDoubleClickProcesser DoubleClickProcesser;
    public GameObject ClickObject = null;
    public UIRoot mUIRoot;
    public Dictionary<string, UIWindow> mWindowToRemoveList = new Dictionary<string, UIWindow>();
    public bool LockUIInputProp { set; get; }

    public void OnCreate()
    {
        Transform uiRootTransform = GameObject.Find("Camera").transform;
        for (Anchor i = Anchor.Center; i <= Anchor.BottomRight; i++)
        {
            AnchorObject[(int)i] = uiRootTransform.FindChild(i + "Anchor").gameObject;
        }
        mUIRoot = uiRootTransform.gameObject.GetComponent<UIRoot>();
    }

    public void Update()
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (pair.Value.Visible)
            {
                pair.Value.OnUpdate();
            }
        }

        if (mWindowToRemoveList.Count != 0)
        {
            foreach (UIWindow pUIWindow in mWindowToRemoveList.Values)
            {
                DestroyWindow(pUIWindow);
            }
            mWindowToRemoveList.Clear();
        }

        ClickObject = null;
    }

    public void OnGUI()
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (pair.Value.Visible)
            {
                pair.Value.OnGUI();
            }

        }
    }

    public void UpdateWindowUIData(string windowName)
    {
        UIWindow pWindow = GetUIWindow(windowName);
        if (pWindow == null)
        {
            //Debug.LogError("Can't find windowname " + windowName);
            return;
        }
        if (pWindow.Visible)
        {
            pWindow.UpdateUIData();
        }
    }

    public void UpdateUIData()
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (pair.Value.Visible)
            {
                pair.Value.UpdateUIData();
            }
        }
    }

    public int CloseWindowsWithFlag(int flag)
    {
        int count = 0;
        ExcuteFuncOnWindowsWithFlag(
            delegate(UIWindow pWin)
            {
                if (pWin.Visible)
                {
                    ++count;
                    pWin.HideWindow();
                }
            },
            flag);
        return count;
    }

    public void ExcuteFuncOnWindowsWithFlag(System.Action<UIWindow> action, int flag)
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if ((pair.Value.mUIFlag & flag) > 0)
            {
                action(pair.Value);
            }
        }
    }

    public GameObject FindUIControll(string name)
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (!pair.Value.Visible)
            {
                continue;
            }
            if (pair.Value.mUIObject.transform.localPosition.x < -1000) continue;
            Transform trans = UIToolkits.FindChild(pair.Value.mUIObject.transform, name);
            if (trans != null)
            {
                return trans.gameObject;
            }
        }
        return null;
    }

    public GameObject FindUIControll(string name, out UIWindow pWindow)
    {
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (pair.Value.mUIObject.transform.localPosition.x < -1000) continue;
            if (!pair.Value.Visible)
            {
                continue;
            }
            Transform trans = UIToolkits.FindChild(pair.Value.mUIObject.transform, name);
            if (trans != null)
            {
                pWindow = pair.Value;
                return trans.gameObject;
            }
        }
        pWindow = null;
        return null;
    }

    public bool FindUIControllRect(string name, out Rect rect)
    {
        Rect tempRect = new Rect();
        rect = tempRect;
        foreach (KeyValuePair<string, UIWindow> pair in mWindowMap)
        {
            if (pair.Value == null)
            {
                continue;
            }
            if (pair.Value.mUIObject.transform.localPosition.x < -1000) continue;
            rect = pair.Value.GetChildRect(name);
            if (!rect.Equals(tempRect))
            {
                return true;
            }
            //Transform trans = UIToolkits.FindChild(pair.Value.mUIObject.transform, name);
            //if (trans != null)
            //{
            //    rect = UIToolkits.GetUIObjectScreenRect(trans);
            //    return true;
            //}
        }
        return false;
    }

    #region Create And Destory And Get Window Functions
    public UIWindow GetUIWindow(string windowName)
    {
        UIWindow pWindow;
        bool bSuc = mWindowMap.TryGetValue(windowName, out pWindow);
        if (bSuc)
        {
            return pWindow;
        }
        return null;
    }

    public T GetUIWindow<T>() where T : UIWindow
    {
        UIWindow pWin = null;
        mWindowMap.TryGetValue(typeof(T).ToString(), out pWin);
        if (pWin is T)
        {
            return pWin as T;
        }
        return null;
    }

    public T CreateWindow<T>() where T : UIWindow, new()
    {
        var window = CreateWindow<T>(typeof(T).ToString(), Anchor.Center);
        return window;
    }

    public T CreateWindow<T>(Anchor UIAnchor) where T : UIWindow, new()
    {
        return CreateWindow<T>(typeof(T).ToString(), UIAnchor);
    }

    public GameObject InstancePreb(string prefabName)
    {
        GameObject prefab = ResourceManager.Singleton.GetUIPrefabByName(prefabName);
        if (null == prefab)
        {
            Debug.LogError(string.Format("Load prefab name = {0} Error ", prefabName));
            return null;
        }
        return (GameObject)GameObject.Instantiate(prefab);
    }

    public bool CreatePrefab(UIWindow pWindow)
    {
        GameObject prefab = ResourceManager.Singleton.GetUIPrefabByName(pWindow.prefabName);
        if (null == prefab)
        {
            Debug.LogError(string.Format("Load prefab name = {0} Error ", pWindow.prefabName));
            return false;
        }
        GameObject instance = (GameObject)GameObject.Instantiate(prefab);
        pWindow.mUIObject = instance;
        pWindow.SetAnchor(AnchorObject[(int)pWindow.mAnchor]);
        pWindow.HideWindowWithoutInvoke();
        pWindow.OnCreate();
        pWindow.AfterCreate();
        return true;
    }

    public T CreateWindow<T>(string prefabName, Anchor anchor) where T : UIWindow, new()
    {
        UIWindow pUIWindow = null;
        if (mWindowToRemoveList.TryGetValue(prefabName, out pUIWindow))
        {
            mWindowToRemoveList.Remove(prefabName);
            return pUIWindow as T;
        }

        pUIWindow = GetUIWindow(prefabName);

        if (pUIWindow != null)
        {
            return pUIWindow as T;
        }

        pUIWindow = new T();
        pUIWindow.prefabName = prefabName;
        pUIWindow.mAnchor = anchor;
        mWindowMap[prefabName] = pUIWindow;

        CreatePrefab(pUIWindow);
        return (T)pUIWindow;
    }

    public void DestroyWindow(UIWindow pWindow)
    {
        Debug.Log("Window Destoryed, name = " + pWindow.prefabName);
        pWindow.OnDestory();
        pWindow.ReleaseWindow();
        mWindowMap.Remove(pWindow.prefabName);
    }

    public void DestroyWindow(UIWindow pWindow, bool immediately)
    {
        if (!immediately)
        {
            mWindowToRemoveList.Add(pWindow.prefabName, pWindow);
            return;
        }
        DestroyWindow(pWindow);
    }

    public void HideAllWindow()
    {
        foreach (var key in mWindowMap.Keys)
        {
            if (mWindowMap[key].Visible)
                mWindowMap[key].HideWindow();
        }
    }

    ///// <summary>
    ///// try to hide ui if the ui exists
    ///// </summary>
    ///// <typeparam name="T">type of ui</typeparam>
    //public void TryToHideWindow<T>() where T: UIGameWindow
    //{
    //    var ui = GetUIWindow<T>();
    //    if (ui != null && ui.IsVisible)
    //    {
    //        ui.HideWindow();
    //    }
    //}
    #endregion
}
