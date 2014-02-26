using UnityEngine;
using System.Collections;

public class UIQuitConfirm : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnQuitClicked);
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

    private void OnQuitClicked()
    {
        Application.Quit();
    }

    private void OnCancelClicked()
    {
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }
}
