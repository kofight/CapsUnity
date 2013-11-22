using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Linq.Expressions;

public class UIGridController<TItem> where TItem : UIGridItem, new()
{


    private int m_id = 0;

    private List<TItem> m_GridItems;
    private object _itemlock = new object();
    private UIGrid m_Grid;

    public event Action<TItem> OnSelected;

    /// <summary>
    /// 列表项复制模板.
    /// </summary>
    private Transform m_templeteTransform;

    private int m_currentIndex;
    private int m_emptySize;

    /// <summary>
    /// 每页条数.
    /// </summary>
    public int PageSize_Prop { get; set; }

    /// <summary>
    /// 一共几页.
    /// </summary>
    public int TotalPageNum_Prop { get; private set; }

    /// <summary>
    /// 针对UIGrid的控制器.
    /// </summary>
    /// <param name="window">Grid控件所属窗体.</param>
    /// <param name="gridName">控件名称.</param>
    /// <param name="ItemName">控件模板项的名称.</param>
    public UIGridController(UIWindow window, string gridName, string ItemName, string defaultBg = "")
    {
        m_Grid = window.GetChildComponent<UIGrid>(gridName);
        if (m_Grid == null)
            throw new Exception("Grid " + gridName + " not found!");
        m_templeteTransform = m_Grid.transform.FindChild(ItemName);
        //将制作UI时的模板隐藏掉.
        m_templeteTransform.gameObject.SetActive(false);

    }

    /// <summary>
    /// 设置数据源,此方法不可频繁调用.
    /// </summary>
    /// <param name="source">数据源.</param>
    /// <param name="pageSize">每页条数 0--不分页,n---每页n条.</param>
    public void SetDataSource(List<TItem> source, int pageSize = 0)
    {
        if (source == null || source.Count < 1) return;

        m_GridItems = source;
        PageSize_Prop = pageSize;
        lock (_itemlock)
        {
            for (int i = 0; i < m_GridItems.Count; i++)
            {
                m_GridItems[i].Id = m_id;
                m_GridItems[i].SetController(this);
                GameObject cloneitem = GameObject.Instantiate(m_templeteTransform.transform.gameObject) as GameObject;
                cloneitem.name = m_templeteTransform.gameObject.name + "_" + m_GridItems[i].Id;
                cloneitem.transform.parent = m_templeteTransform.transform.parent;
                cloneitem.transform.localScale = m_templeteTransform.transform.localScale;
                cloneitem.transform.localPosition = m_templeteTransform.transform.localPosition;
                if (pageSize > 0 && i >= pageSize)
                    cloneitem.gameObject.SetActive(false);
                else
                    cloneitem.gameObject.SetActive(true);
                m_GridItems[i].SetTransform(cloneitem.transform);
                m_GridItems[i].InitUI();
                Interlocked.Increment(ref m_id);
            }
        }

        Reposition();
    }

    /// <summary>
    /// 添加一项.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(TItem item)
    {
        lock (_itemlock)
        {
            if (m_GridItems == null)
                m_GridItems = new List<TItem>();
            item.Id = m_id;
            GameObject cloneitem = GameObject.Instantiate(m_templeteTransform.transform.gameObject) as GameObject;
            cloneitem.name = m_templeteTransform.gameObject.name + "_" + item.Id;
            cloneitem.transform.parent = m_templeteTransform.transform.parent;
            cloneitem.transform.localScale = m_templeteTransform.transform.localScale;
            cloneitem.transform.localPosition = m_templeteTransform.transform.localPosition;
            cloneitem.gameObject.SetActive(true);
            item.SetTransform(cloneitem.transform);
            item.SetController(this);
            item.InitUI();
            m_GridItems.Add(item);
            Interlocked.Increment(ref m_id);
        }
        Reposition();
    }

    /// <summary>
    /// 重绘列表.
    /// </summary>
    public void Reposition()
    {
        m_Grid.Reposition();
    }

    /// <summary>
    /// 移除一个列表项.
    /// </summary>
    /// <param name="gridItem"></param>
    public void Remove(UIGridItem gridItem)
    {
        if (gridItem == null)
            return;
        if (gridItem.Equals(SelectedItem))
            SelectedItem = null;
        if (gridItem.Equals(LastSelectedItem))
            LastSelectedItem = null;
        Transform ga = m_Grid.transform.FindChild(gridItem.Transform_Prop.name);
        UnityEngine.Object.Destroy(ga.gameObject);
        m_GridItems.RemoveAll(p => p.Equals(gridItem));
        gridItem.Dispose();
        Reposition();
    }


    public void RemoveAll()
    {
        if (m_GridItems == null)
            return;
        foreach (var item in m_GridItems)
        {
            Transform ga = m_Grid.transform.FindChild(item.Transform_Prop.name);
            UnityEngine.Object.Destroy(ga.gameObject);
            item.Dispose();
        }
        m_GridItems.Clear();
        Reposition();
    }


    public List<TItem> GetItems(Expression<Func<TItem, bool>> condition)
    {
        return m_GridItems.Where(condition.Compile()).ToList();
    }

    /// <summary>
    /// 激活某项.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="enable"></param>
    public void Active(TItem item, bool enable)
    {
        item.GameObject_Prop.SetActive(enable);
    }


    private TItem _tItem;

    /// <summary>
    /// 当前选中的项.
    /// </summary>
    public TItem SelectedItem { get { return _tItem; } set { _tItem = value; OnSelected(value); } }


    /// <summary>
    /// 上次选中的项.
    /// </summary>
    public TItem LastSelectedItem { get; set; }

}

