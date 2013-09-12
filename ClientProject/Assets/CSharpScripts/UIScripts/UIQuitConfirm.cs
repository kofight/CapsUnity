using UnityEngine;
using System.Collections;

public class UIQuitConfirm : UIWindowNGUI 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("QuitBtn", OnQuitClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnQuitClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
        }
        else
        {
            Application.Quit();
        }
    }

    private void OnCancelClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
    }
}
