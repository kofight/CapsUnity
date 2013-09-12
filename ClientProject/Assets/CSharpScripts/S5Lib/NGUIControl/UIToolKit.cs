using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIToolkits
{
    static public void SetUILayer(GameObject obj, int layer)
    {
        SetRecursively(obj, (int)layer);
    }

    private static void SetRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        if (obj.transform.childCount == 0) return;
        for (var i = 0; i < obj.transform.childCount; i++)
        {
            SetRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    static public void ChangePositionZ(UIWindow pWindow, int z)
    {
        if (pWindow is UIWindowNGUI)
        {
            Vector3 pos = pWindow.mUIObject.transform.localPosition;
            pos.z = z;
            pWindow.mUIObject.transform.localPosition = pos;
        }
    }

    static public void ChangePositionZ(Transform trans, int z)
    {
        Vector3 pos = trans.localPosition;
        pos.z = z;
        trans.localPosition = pos;
    }

    static public T GetChildComponent<T>(Component obj, string name) where T : Component
    {
        if (obj == null)
        {
            return null;
        }

        Transform tansform = FindChild(obj.transform, name);
        if (null == tansform)
        {
            return null;
        }

        return tansform.GetComponent<T>();
    }

    static public T FindComponentInAllChild<T>(Transform transParent) where T : Component
    {
        foreach (Transform transform in transParent)
        {
            T comp = transform.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            if (transform.GetChildCount() > 0)
            {
                T childComp = FindComponentInAllChild<T>(transform);
                if (childComp != null)
                {
                    return childComp;
                }
            }
        }
        return null;
    }

    static public T FindComponentInAllChild<T>(Transform transParent, string childName) where T : Component
    {
        Transform trans = FindChild(transParent, childName);
        if (trans == null)
        {
            return null;
        }
        return trans.GetComponent<T>();
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

            if (transform.GetChildCount() > 0)
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

            if (transform.GetChildCount() > 0)
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

    public static T FindComponent<T>(Transform panelTransform, string name) where T : Component
    {
        return FindChild(panelTransform, name).GetComponent<T>();
    }

    static public bool FindChild(Transform panelTransform, List<Transform> list, string name)
    {
        foreach (Transform transform in panelTransform)
        {
            if (transform.name == name)
            {
                list.Add(transform);
                return true;
            }

            if (transform.GetChildCount() > 0)
            {
                if (FindChild(transform, list, name))
                {
                    list.Add(transform);
                    return true;
                }
            }
        }
        return false;
    }

    static public void FindComponents<T>(Transform panelTransform, List<T> list) where T : Component
    {
        foreach (Transform transform in panelTransform)
        {
            T[] tArray = transform.GetComponents<T>();
            foreach (T t in tArray)
            {
                list.Add(t);
            }

            FindComponents(transform, list);			//查找子节点
        }
    }

    static public void SetLayerRecursively(Transform transform, int layer)
    {
        if (null == transform)
        {
            return;
        }

        foreach (Transform trans in transform)
        {
            trans.gameObject.layer = layer;
            if (trans.GetChildCount() > 0)
            {
                SetLayerRecursively(trans, layer);
            }
        }
    }

    static public void ChangePositionX(Transform transform, int x)
    {
        Vector3 pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }


    static public void ChangePositionY(Transform transform, int y)
    {
        Vector3 pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;
    }




    static public UITouchClick AddChildComponentTouchClick(GameObject gameObject, System.EventHandler<UITouchClick.ClickArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return null;
        }
        UITouchClick click = gameObject.GetComponent<UITouchClick>();
        if (null == click)
            click = gameObject.AddComponent<UITouchClick>();
        click.Click += action;
        return click;
    }

    static public UICheckboxActivate AddChildComponentActivate(GameObject checkBox, System.EventHandler<UICheckboxActivate.ActivateArgs> action)
    {
        if (null == checkBox)
        {
            Debug.LogError(" checkBox is null .");
            return null;
        }
        if (checkBox.GetComponent<UICheckbox>() == null)
        {
            Debug.LogError(" only operator checkBox.");
            return null;
        }
        var activate = checkBox.GetComponent<UICheckboxActivate>();
        if (activate == null)
        {
            activate = checkBox.AddComponent<UICheckboxActivate>();
        }

        activate.OnSelected -= action;
        activate.OnSelected += action;

        return activate;
    }


    static public UISliderChange AddChildSliderChange(UISlider slider, System.EventHandler<UISliderChange.ChangeEventArgs> action)
    {
        if (null == slider)
        {
            Debug.LogWarning(" UISlider is null .");
            return null;
        }

        UISliderChange change = slider.gameObject.GetComponent<UISliderChange>();
        if (null == change)
            change = slider.gameObject.AddComponent<UISliderChange>();

        slider.eventReceiver = change.gameObject;
        change.OnChange += action;

        return change;
    }


    static public UISliderChange AddChildSliderChange(GameObject gameObject, System.EventHandler<UISliderChange.ChangeEventArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogWarning(" gameObject is null .");
            return null;
        }

        UISlider slider = gameObject.GetComponent<UISlider>();
        if (null == slider)
        {
            Debug.LogWarning(" gameObject is null .");
            return null;
        }
        return AddChildSliderChange(slider, action);
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
        var tweenScale = uiObjTrans.GetComponent<AocShowUIEffect>();
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

    public static UIMouseClick AddChildComponentMouseClick(GameObject gameObject, System.EventHandler<UIMouseClick.ClickArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return null;
        }

        UIMouseClick click = gameObject.GetComponent<UIMouseClick>();
        if (click != null)
        {
            GameObject.DestroyImmediate(click);
        }
        click = gameObject.AddComponent<UIMouseClick>();
        click.Click += action;
        click.UserState = gameObject;
        return click;
    }

    public static UIMouseClick AddChildComponentMouseClick(Transform paneltransform, string name, System.EventHandler<UIMouseClick.ClickArgs> action)
    {
        Transform tansform = UIToolkits.FindChild(paneltransform, name);
        if (null == tansform)
            return null;
         return AddChildComponentMouseClick(tansform.gameObject, action);
    }

    public static UIMouseHover AddChildComponentMouseHover(GameObject gameObject, System.EventHandler<UIMouseHover.HoverArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return null;
        }
        UIMouseHover hover = gameObject.GetComponent<UIMouseHover>();
        if (hover != null)
        {
            GameObject.DestroyImmediate(hover);
        }
        hover = gameObject.AddComponent<UIMouseHover>();
        hover.Hover += action;
        return hover;
    }
    public static UIMousePress AddChildComponentMousePress(GameObject gameObject, System.EventHandler<UIMousePress.ClickArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return null;
        }
        UIMousePress press = gameObject.GetComponent<UIMousePress>();
        if (null == press)
            press = gameObject.AddComponent<UIMousePress>();
        press.Press += action;
        return press;
    }
    public static UITouchClick AddChildComponentMouseTouch(GameObject gameObject, System.EventHandler<UITouchClick.ClickArgs> action)
    {
        if (null == gameObject)
        {
            Debug.LogError(" gameObject is null .");
            return null;
        }
        UITouchClick click = gameObject.GetComponent<UITouchClick>();
        if (click != null)
        {
            GameObject.DestroyImmediate(click);
        }
        click = gameObject.AddComponent<UITouchClick>();
        click.Click += action;
        return click;
    }
    private static bool GetWidgetComponentRect(Transform obj, ref Rect rect)
    {
        UIWidget widget = obj.GetComponent<UIWidget>();

        if (null == widget)
        {
            do
            {
                {
                    UICheckbox box = obj.GetComponent<UICheckbox>();
                    if (null != box)
                    {
                        widget = box.checkSprite;
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

}
