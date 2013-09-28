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
        Application.Quit();
    }

    private void OnCancelClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
    }
}
