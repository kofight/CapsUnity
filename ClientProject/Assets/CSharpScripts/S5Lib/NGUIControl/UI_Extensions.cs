using UnityEngine;
using System.Collections;




public static class UI_Extensions
{
    public static bool Enable(this UIImageButton button)
    {
        BoxCollider boxCollider = button.GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            return boxCollider.enabled;
        }

        Debug.LogWarning(string.Format("UIImageButton {0} does not contain BoxCollider", button.name));
        return false;
    }



    public static void Text(this UIImageButton button, string text)
    {
        //button.target.spriteName = button.normalSprite;
        UILabel label = button.GetComponentInChildren<UILabel>();
        if (null == label)
        {
            return;
        }

        label.text = text;
    }


    public static void Enable(this UIWidget widget, bool enable)
    {
        if (null == widget)
        {
            return;
        }

        Color color = widget.color;
        color.a = enable ? 1.0f : 0.5f;
        widget.color = color;
    }


    public static void Gray(this UIWidget widget, bool enable)
    {
        if (null == widget)
        {
            return;
        }

        Color color = widget.color;
        color.r = enable ? 1f : 158f / 255f;
        color.g = enable ? 1f : 158f / 255f;
        color.b = enable ? 1f : 158f / 255f;
        color.a = enable ? 1f : 225f / 255f;
        widget.color = color;
    }

    #region Mouse Touch Click

    public static bool TouchClick(this Transform obj, System.EventHandler<UITouchClick.ClickArgs> action)
    {
        return TouchClick(obj, action, null);
    }

    public static bool TouchClick(this Transform obj, System.EventHandler<UITouchClick.ClickArgs> action, object userState)
    {
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogWarning(obj.name + " No boxCollider");
            return false;
        }

        UITouchClick click = obj.gameObject.GetComponent<UITouchClick>();
        if (click == null)
        {
            click = obj.gameObject.AddComponent<UITouchClick>();
        }

        if (null == click)
        {
            return false;
        }

        click.Click -= action;
        click.Click += action;
        return true;
    }


    public static bool TouchClick(this MonoBehaviour obj, System.EventHandler<UITouchClick.ClickArgs> action)
    {
        return TouchClick(obj, action, null);
    }


    public static bool TouchClick(this MonoBehaviour obj, System.EventHandler<UITouchClick.ClickArgs> action, object userState)
    {
        if (null == obj)
        {
            return false;
        }
        return TouchClick(obj.transform, action, userState);
    }


    #endregion

    public static int Count<TSource>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, bool> predicate)
    {
        int n = 0;
        foreach (TSource i in source)
        {
            if (predicate(i))
            {
                n++;
            }
        }
        return n;
    }


    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        if (null != gameObject)
        {
            UIToolkits.SetLayerRecursively(gameObject.transform, layer);
        }
    }


    public static void SetLayerRecursively(this Transform transform, int layer)
    {
        UIToolkits.SetLayerRecursively(transform, layer);
    }




    #region Transform extensions


    public static T FindChild<T>(this Transform transform, string name) where T : Component
    {
        Transform child = transform.FindChild(name);
        if (child != null)
        {
            return child.GetComponent<T>();
        }
        return null;
    }

    #endregion



    #region Transform position scale


    public static void LocalPositionX(this MonoBehaviour mono, float x)
    {
        LocalPositionX(mono.transform, x);
    }
    public static void LocalPositionX(this GameObject obj, float x)
    {
        LocalPositionX(obj.transform, x);
    }
    public static void LocalPositionX(this Transform transform, float x)
    {
        Vector3 pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }



    public static void LocalPositionY(this MonoBehaviour mono, float y)
    {
        LocalPositionY(mono.transform, y);
    }
    public static void LocalPositionY(this GameObject obj, float y)
    {
        LocalPositionY(obj.transform, y);
    }
    public static void LocalPositionY(this Transform transform, float y)
    {
        Vector3 pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;
    }



    public static void LocalPositionZ(this MonoBehaviour mono, float z)
    {
        LocalPositionZ(mono.transform, z);
    }
    public static void LocalPositionZ(this GameObject obj, float z)
    {
        LocalPositionZ(obj.transform, z);
    }
    public static void LocalPositionZ(this Transform transform, float z)
    {
        Vector3 pos = transform.localPosition;
        pos.z = z;
        transform.localPosition = pos;
    }


    public static void LocalScaleX(this Transform transform, float x)
    {
        Vector3 pos = transform.localScale;
        pos.x = x;
        transform.localScale = pos;
    }
    public static void LocalScaleY(this Transform transform, float y)
    {
        Vector3 pos = transform.localScale;
        pos.y = y;
        transform.localScale = pos;
    }
    public static void LocalScaleZ(this Transform transform, float z)
    {
        Vector3 pos = transform.localScale;
        pos.z = z;
        transform.localScale = pos;
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha">0-1.0f</param>
    public static void Alpha(this Transform transform, float alpha)
    {
        UIWidget[] widgets = transform.GetComponentsInChildren<UIWidget>();
        if (null == widgets)
        {
            return;
        }

        System.Array.ForEach(widgets, (w) => { if (null != w) { w.alpha = alpha; } });
    }

    public static void Alpha(this Component component, float alpha)
    {
        if (null == component)
        {
            return;
        }
        Alpha(component.transform, alpha);
    }

    public static void Alpha(this UIImageButton button, float alpha)
    {
        button.transform.Alpha(alpha);
    }



    #region UISlider

    public static bool OnChange(this UISlider slider, System.EventHandler<UISliderChange.ChangeEventArgs> action)
    {
        return OnChange(slider, action, null);
    }
    public static bool OnChange(this UISlider slider, System.EventHandler<UISliderChange.ChangeEventArgs> action, object userState)
    {
        BoxCollider boxCollider = slider.GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogWarning(slider.name + " No boxCollider");
            return false;
        }

        UISliderChange click = slider.gameObject.GetComponent<UISliderChange>();
        if (null == click)
        {
            click = slider.gameObject.AddComponent<UISliderChange>();
        }

        if (null == click)
        {
            return false;
        }

        slider.eventReceiver = click.gameObject;
        click.UserState = userState;

        click.OnChange -= action;
        click.OnChange += action;
        return true;
    }



    public static void MaxSteps(this UISlider slider, int n)
    {
        if (n > 0)
        {
            slider.numberOfSteps = n + 1;
        }
        else
        {
            slider.numberOfSteps = 1;
        }
    }


    public static int MaxSteps(this UISlider slider)
    {
        return slider.numberOfSteps - 1;
    }


    public static int Value(this UISlider slider)
    {
        return Mathf.RoundToInt(slider.MaxSteps() * slider.sliderValue);
    }


    public static void Value(this UISlider slider, int value)
    {
        if (value < 0)
        {
            value = 0;
        }

        if (value > slider.MaxSteps())
        {
            value = slider.MaxSteps();
        }

        if (slider.MaxSteps() > 0)
        {
            slider.sliderValue = (float)value / slider.MaxSteps();
        }
        else
        {
            slider.sliderValue = 0;
        }
    }


    public static float SubStep(this  UISlider slider)
    {
        slider.Value(slider.Value() - 1);
        return slider.sliderValue;
    }

    public static float AddStep(this  UISlider slider)
    {
        slider.Value(slider.Value() + 1);
        return slider.sliderValue;
    }
    #endregion





    #region UILabel





    public static float Width(this UIWidget w)
    {
        return w.relativeSize.x * w.transform.localScale.x;
    }

    public static float Height(this UIWidget w)
    {
        return w.relativeSize.y * w.transform.localScale.y;
    }


    public static float Left(this UIWidget w)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.BottomLeft:
                return w.transform.localPosition.x;
            case UIWidget.Pivot.TopRight:
            case UIWidget.Pivot.Right:
            case UIWidget.Pivot.BottomRight:
                return w.transform.localPosition.x - w.Width();
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Bottom:
            default:
                return w.transform.localPosition.x - w.Width() / 2;
        }
    }


    public static void Left(this UIWidget w, float pos)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.BottomLeft:
                w.transform.LocalPositionX(pos);
                break;
            case UIWidget.Pivot.TopRight:
            case UIWidget.Pivot.Right:
            case UIWidget.Pivot.BottomRight:
                w.transform.LocalPositionX(pos + w.Width());
                break;
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Bottom:
            default:
                w.transform.LocalPositionX(pos + w.Width() / 2);
                break;
        }
    }



    public static float Right(this UIWidget w)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.BottomLeft:
                return w.transform.localPosition.x + w.Width();
            case UIWidget.Pivot.TopRight:
            case UIWidget.Pivot.Right:
            case UIWidget.Pivot.BottomRight:
                return w.transform.localPosition.x;
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Bottom:
            default:
                return w.transform.localPosition.x + w.Width() / 2;
        }
    }

    public static void Right(this UIWidget w, float pos)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.BottomLeft:
                w.transform.LocalPositionY(pos - w.Width());
                break;
            case UIWidget.Pivot.TopRight:
            case UIWidget.Pivot.Right:
            case UIWidget.Pivot.BottomRight:
                w.transform.LocalPositionX(pos);
                break;
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Bottom:
            default:
                w.transform.LocalPositionX(pos - w.Width() / 2);
                break;
        }
    }


    public static float Top(this UIWidget w)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.TopRight:
                return w.transform.localPosition.y - w.Height();
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Right:
                return w.transform.localPosition.y - w.Height() / 2;
            case UIWidget.Pivot.BottomLeft:
            case UIWidget.Pivot.Bottom:
            case UIWidget.Pivot.BottomRight:
            default:
                return w.transform.localPosition.y;
        }
    }

    public static void Top(this UIWidget w, float pos)
    {
        switch (w.pivot)
        {
            case UIWidget.Pivot.TopLeft:
            case UIWidget.Pivot.Top:
            case UIWidget.Pivot.TopRight:
                w.transform.LocalPositionY(pos);
                break;
            case UIWidget.Pivot.Left:
            case UIWidget.Pivot.Center:
            case UIWidget.Pivot.Right:
                w.transform.LocalPositionY(pos - w.Height() / 2);
                break;
            case UIWidget.Pivot.BottomLeft:
            case UIWidget.Pivot.Bottom:
            case UIWidget.Pivot.BottomRight:
            default:
                w.transform.LocalPositionY(pos - w.Height());
                break;
        }
    }



    public static void Text(this UILabel label, string value)
    {
        if (null != label)
        {
            label.text = value;
        }
    }
    #endregion


    public static bool SetTopWindow(this UIWindowNGUI panel)
    {
        BoxCollider boxCollider = panel.mUIObject.GetComponent<BoxCollider>();

        if (null == boxCollider)
        {
            return false;
        }

        boxCollider.size = new Vector3(Screen.width, Screen.height, 1);
        return true;
    }



    #region UITexture

    public static bool SetTexture(this UITexture ui, Texture texture)
    {
        if (null == ui)
        {
            return false;
        }

        if (null == texture)
        {
            ui.material = null;
            return false;
        }

        if (ui.material == null)
        {
            ui.material = new Material(Shader.Find("Unlit/Transparent Colored"));
        }
        ui.material.SetTexture("_MainTex", texture);
        return true;
    }


    #endregion

}
