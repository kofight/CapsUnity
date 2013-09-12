using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;


/// <summary>
/// UIGridController的数据项,在派生类中初始化UI时请override InitUI方法.
/// </summary>
public class UIGridItem
{
    public int Id { get; set; }
    public UIGridItem()
    {

    }

    /// <summary>
    /// 用携带的数据.
    /// </summary>
    public object UserState { get; set; }

    /// <summary>
    /// 设置transform,在设置transform之前不可以访问UIGridItem模板中的任何控件.
    /// </summary>
    /// <param name="transform"></param>
    public void SetTransform(Transform transform)
    {
        Transform_Prop = transform;
    }

    public Transform Transform_Prop { get; private set; }
    public GameObject GameObject_Prop { get { return Transform_Prop.gameObject; } }

    private object m_controller;
    public void SetController(object controller)
    {
        m_controller = controller;
    }

    public UIGridController<TItem> GetController<TItem>() where TItem : UIGridItem, new()
    {
        UIGridController<TItem> controller = m_controller as UIGridController<TItem>;
        return controller;
    }

    public override bool Equals(object obj)
    {
        UIGridItem equlobj = obj as UIGridItem;
        if (equlobj == null)
            return false;
        return equlobj.Id == Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public void AddClickEvent(Transform transform, System.EventHandler<UIMouseClick.ClickArgs> action)
    {
        if (action == null)
            return;
        UIToolkits.AddChildComponentMouseClick(transform.gameObject, action);
    }

    /// <summary>
    /// 初始化UI.
    /// </summary>
    public virtual void InitUI()
    {

    }

    /// <summary>
    /// 清理资源的逻辑.
    /// </summary>
    public virtual void Dispose()
    {

    }

    /// <summary>
    /// 更换代表列表项状态的贴图.
    /// </summary>
    /// <param name="name"></param>
    public virtual void ChangeSprite(string name)
    {

    }
}