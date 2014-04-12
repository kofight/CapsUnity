using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIToolkits
{
    static public void SetUILayer(GameObject obj, int layer)        //Todo 干啥用的？
    {
        SetRecursively(obj, (int)layer);
    }

    private static void SetRecursively(GameObject obj, int layer)   //递归设置Layer
    {
        obj.layer = layer;
        if (obj.transform.childCount == 0) return;
        for (var i = 0; i < obj.transform.childCount; i++)
        {
            SetRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    static public Transform FindChildCheckActive(Transform panelTransform, string name)
    {
        foreach (Transform transform in panelTransform)
        {
            if (!transform.gameObject.activeSelf)
            {
                continue;
            }

            if (transform.name == name)
            {
                return transform;
            }

            if (transform.childCount > 0)
            {
                Transform childTransform = FindChild(transform, name);
                if (childTransform != null)
                {
                    return childTransform;
                }
            }
        }
        return null;
    }

    static public Transform FindChild(Transform panelTransform, string name)
    {
        foreach (Transform transform in panelTransform)
        {
            if (transform.name == name)
            {
                return transform;
            }

            if (transform.childCount > 0)
            {
                Transform childTransform = FindChild(transform, name);
                if (childTransform != null)
                {
                    return childTransform;
                }
            }
        }
        return null;
    }

    //用名字和组件类型查找组件
    public static T FindComponent<T>(Transform panelTransform, string name) where T : Component
    {
        Transform trans = FindChild(panelTransform, name);
        if (trans == null)
        {
            return null;
        }
        return trans.GetComponent<T>();
    }

    //用组件类型查找组件
    static public T FindComponent<T>(Transform transParent) where T : Component
    {
        foreach (Transform transform in transParent)
        {
            T comp = transform.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            if (transform.childCount > 0)
            {
                T childComp = FindComponent<T>(transform);
                if (childComp != null)
                {
                    return childComp;
                }
            }
        }
        return null;
    }

    //查找Transform下面所有某类型控件，结果放到传入的List里面
    static public void FindComponents<T>(Transform panelTransform, List<T> list) where T : Component
    {
        T tt= panelTransform.GetComponent<T>();
        if (tt != null)
        {
            list.Add(tt);
        }
        foreach (Transform transform in panelTransform)
        {
            FindComponents(transform, list);			//查找子节点
        }
    }

    static public void FindComponents<T, T2>(Transform panelTransform, List<T2> list) where T : T2 where T2 : Component
    {
        T2 tt = panelTransform.GetComponent<T2>();
        if (tt != null)
        {
            list.Add(tt);
        }
        foreach (Transform transform in panelTransform)
        {
            FindComponents(transform, list);			//查找子节点
        }
    }

    public static bool AddChildComponentMouseClick(GameObject gameObject, EventDelegate.Callback callBack)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return false;
        }

        UIButton button = gameObject.GetComponent<UIButton>();
        if (button == null)
        {
            return false;
        }
        EventDelegate.Set(button.onClick, callBack);
        return true;
    }

    public static bool AddChildComponentMouseClick(Transform paneltransform, string name, EventDelegate.Callback action)
    {
        Transform tansform = UIToolkits.FindChild(paneltransform, name);
        if (null == tansform)
            return false;
         return AddChildComponentMouseClick(tansform.gameObject, action);
    }

    static public Vector2 GetUIObjectScreenPos(Transform uiObjTrans)
    {
        if (uiObjTrans == null || UICamera.currentCamera == null)
        {
            return new Vector2(0, 0);
        }
        Vector3 viewpos = UICamera.currentCamera.WorldToViewportPoint(uiObjTrans.position);
        Vector3 screenpos = UICamera.currentCamera.ViewportToScreenPoint(viewpos);

        //float multiplier = UIWindowManager.Singleton.mUIRoot.manualHeight / Screen.height;
        float multiplier = 1.0f;
        screenpos = new Vector3(screenpos.x * multiplier, Screen.height - screenpos.y * multiplier, 0);
        return new Vector2(screenpos.x, screenpos.y);
    }

    static public Rect GetUIObjectScreenRect(Transform uiObjTrans)
    {
        Rect rect = new Rect();
        if (uiObjTrans == null)
        {
            return rect;
        }
        var box = uiObjTrans.GetComponent<BoxCollider>();
        Vector2 pos = UIToolkits.GetUIObjectScreenPos(uiObjTrans);
        var tweenScale = uiObjTrans.GetComponent<ShowSimpleUIEffect>();
        if (box != null)
        {
            float scaleX = 1.0f;
            float scaleY = 1.0f;
            //exclude scale effect
            if (tweenScale == null)
            {
                scaleX = uiObjTrans.localScale.x;
                scaleY = uiObjTrans.localScale.y;
            }

            rect.width = box.size.x * scaleX;
            rect.height = box.size.y * scaleY;

            float offsetX = box.center.x * scaleX;
            float offsetY = box.center.y * scaleY;

            rect.x = pos.x + offsetX - rect.width / 2;
            rect.y = pos.y - offsetY - rect.height / 2;
        }
        else
        {
            rect.x = pos.x;
            rect.y = pos.y - uiObjTrans.localPosition.y;			//Convert Y
            GetWidgetComponentRect(uiObjTrans, ref rect);
        }
        return rect;
    }

    private static bool GetWidgetComponentRect(Transform obj, ref Rect rect)
    {
        UIWidget widget = obj.GetComponent<UIWidget>();

        if (null == widget)
        {
            do
            {
                {
                    UIToggle box = obj.GetComponent<UIToggle>();
                    if (null != box)
                    {
                        widget = box.activeSprite;
                        break;
                    }
                }

                {
                    UIButtonColor button = obj.GetComponent<UIButtonColor>();
                    if (null != button)
                    {
                        widget = button.tweenTarget.GetComponent<UIWidget>();
                        break;
                    }
                }

                {
                    UIImageButton button = obj.GetComponent<UIImageButton>();
                    if (null != button)
                    {
                        widget = button.target;
                        break;
                    }
                }

                {
                    UISlider slider = obj.GetComponent<UISlider>();
                    if (null != slider)
                    {
                        Debug.LogError("don't support UISlider");
                        return false;
                    }
                }

            } while (false);

            if (null == widget)
            {
                return false;
            }

            rect.x += widget.transform.localPosition.x;
            rect.y += widget.transform.localPosition.y;
        }

        rect.width = widget.transform.localScale.x;
        rect.height = widget.transform.localScale.y;

        switch (widget.pivot)
        {
            case UIWidget.Pivot.TopLeft:

                break;
            case UIWidget.Pivot.Top:
                rect.x -= rect.width / 2;
                break;
            case UIWidget.Pivot.TopRight:
                rect.x -= rect.width;
                break;
            case UIWidget.Pivot.Left:
                rect.y += rect.height / 2;
                break;
            case UIWidget.Pivot.Center:
                rect.x -= rect.width / 2;
                rect.y += rect.height / 2;
                break;
            case UIWidget.Pivot.Right:
                rect.x -= rect.width;
                rect.y += rect.height / 2;
                break;
            case UIWidget.Pivot.BottomLeft:
                rect.y += rect.height;
                break;
            case UIWidget.Pivot.Bottom:
                rect.x -= rect.width / 2;
                rect.y += rect.height;
                break;
            case UIWidget.Pivot.BottomRight:
                rect.x -= rect.width;
                rect.y += rect.height;
                break;
            default:
                break;
        }
        return true;
    }

    ///模仿NGUI写个PlaySound，主要是为了播放音乐
    static AudioSource mCurMusic;

    static public void PlayMusic(AudioClip clip)
    {
        if (mCurMusic == null)
        {
            mCurMusic = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        }

        if (clip != null)
        {
            mCurMusic.clip = clip;
            mCurMusic.Play();
        }
    }

    static public void StopMusic()
    {
        if (mCurMusic == null)
        {
            mCurMusic = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        }
        mCurMusic.Stop();
    }

    static public bool IsPlayingMusic()
    {
        if (mCurMusic == null)
        {
            return false;
        }
        return mCurMusic.isPlaying;
    }
}
