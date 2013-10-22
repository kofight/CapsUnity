using UnityEngine;
using System.Collections;

public class UILogin : UIWindowNGUI 
{
    UICheckbox m_developerMode;
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("PlayBtn", OnPlayBtnClick);
        AddChildComponentMouseClick("LoginBtn", OnLoginBtnClick);
        m_developerMode = UIToolkits.FindComponent<UICheckbox>(mUIObject.transform, "DeveloperCheck");
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnPlayBtnClick(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
		GlobalVars.DeveloperMode = m_developerMode.isChecked;
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
        LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
    }

    private void OnLoginBtnClick(object sender, UIMouseClick.ClickArgs e)
    {
        //HideWindow();                                                   //隐藏窗口
        //LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图

    }
}
