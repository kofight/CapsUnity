using UnityEngine;
using System.Collections;

public class UIMainMenu : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("QuitBtn", OnQuitClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnQuitClicked()
    {
        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() != null && UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().Visible)
            {
                UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().HideWindow();
            }
            UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();   
        }
        else if (UIWindowManager.Singleton.GetUIWindow<UIMap>().Visible)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
        }
    }
}
