using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIMessageBox : UIWindowNGUI
{
    private UISprite img_Alert;
    private UILabel lbl_Msg;
    Transform [] m_btn = new Transform[3];
    UIMouseClick [] m_clickEvent = new UIMouseClick [3];

    UIMouseClick m_okClick;
    UIMouseClick m_customClick;

    public override void OnCreate()
    {
        base.OnCreate();
        img_Alert = GetChildComponent<UISprite>("img_Alert");
        lbl_Msg = GetChildComponent<UILabel>("lbl_Msg");
        m_btn[0] = mUIObject.transform.FindChild("bt_OK");
        m_btn[1] = mUIObject.transform.FindChild("bt_Cancel");
        m_btn[2] = mUIObject.transform.FindChild("bt_Custom");
    }

    public void On_CancelClick(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
    }

    public override void OnHideEffectPlayOver()
    {
        base.OnHideEffectPlayOver();
        foreach (Transform btn in m_btn)
        {
            btn.gameObject.SetActive(false);
        }

        foreach(UIMouseClick click in m_clickEvent)
        {
            if (click != null)
            {
                GameObject.Destroy(click);
            }
        }
    }

    public void ShowMessage(bool iserror, string msg)
    {
        if (iserror)
        {
            img_Alert.spriteName = "error";
        }
        else
        {
            img_Alert.spriteName = "alert";
        }
        lbl_Msg.text = msg;
        m_btn[0].gameObject.SetActive(true);
        m_clickEvent[0] = UIToolkits.AddChildComponentMouseClick(m_btn[0].gameObject, On_CancelClick);

        ShowWindow();
    }

    public void ShowCustomMessageBox(bool iserror, string msg, int buttonCount)
    {
        if (iserror)
        {
            img_Alert.spriteName = "error";
        }
        else
        {
            img_Alert.spriteName = "alert";
        }
        lbl_Msg.text = msg;
        for (int i = 0; i < buttonCount; ++i )
        {
            m_btn[i].gameObject.SetActive(true);  //
        }
        ShowWindow();
    }
	
	public override void OnShow()
	{
		base.OnShow();
		mUIObject.GetComponent<BoxCollider>().size = new Vector3(Screen.width, Screen.height, 1);
	}

    public void SetCustomButton(int index, string name, System.EventHandler<UIMouseClick.ClickArgs> action)
    {
        if (m_clickEvent != null)       //
        {
            GameObject.Destroy(m_clickEvent[index]);
        }
        m_clickEvent[index] =  UIToolkits.AddChildComponentMouseClick(m_btn[index].gameObject, action);

        if (name != string.Empty)       //按钮图片
        {
            UISprite sprite = UIToolkits.FindComponentInAllChild<UISprite>(m_btn[index]);
            sprite.spriteName = name;
        }
    }
}

