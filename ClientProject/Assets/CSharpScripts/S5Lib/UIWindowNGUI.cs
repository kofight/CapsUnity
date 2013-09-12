using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindowNGUI : UIWindow 
{
    protected UIPanel mNGUIPanel;


    public override void SetAnchor(GameObject UIAnchor)
    {
		float saveZ = mUIObject.transform.localPosition.z;
        mUIObject.transform.parent = UIAnchor.transform;

        mUIObject.transform.localPosition = new Vector3(0, 0, saveZ);
        mUIObject.transform.localScale = Vector3.one;
    }
    public void InitNGUIWindow(UIPanel panel, GameObject prefab)
    {
        mNGUIPanel = panel;
        mUIObject = prefab;
    }

    protected override void DoHideWindow()
    {
        mNGUIPanel.enabled = false;
        mUIObject.SetActive(false);
    }

    protected override void DoShowWindow()
    {
        mNGUIPanel.enabled = true;
        mUIObject.SetActive(true);
    }


    public override Rect GetChildRect( string name )
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
	
	public override Rect GetRect() {
		return UIToolkits.GetUIObjectScreenRect(mNGUIPanel.transform);
	}

    public override void SetPosition(Vector2 pos)
    {
        mUIObject.transform.localPosition = new Vector3(pos.x, -pos.y, mUIObject.transform.localPosition.z);
    }

    public override void ReleaseWindow()
    {
        base.ReleaseWindow();
        GameObject.Destroy(mNGUIPanel);
        mNGUIPanel = null;
    }
}
