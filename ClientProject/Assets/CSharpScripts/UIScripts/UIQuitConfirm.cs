using UnityEngine;
using System.Collections;

public class UIQuitConfirm : UIWindow 
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

    private void OnQuitClicked()
    {
        Application.Quit();
    }

    private void OnCancelClicked()
    {
        HideWindow();
    }
}
