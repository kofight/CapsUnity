using UnityEngine;
using System.Collections;




public static class UI_Extensions
{
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


    public static void Alpha(this Component component, float alpha)
    {
        if (null == component)
        {
            return;
        }
        Alpha(component.transform, alpha);
    }

    #region UILabel





    public static float Width(this UIWidget w)
    {
        return w.width;
    }

    public static float Height(this UIWidget w)
    {
        return w.height;
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
