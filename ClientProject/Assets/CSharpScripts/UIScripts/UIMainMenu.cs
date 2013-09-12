using UnityEngine;
using System.Collections;

public class UIMainMenu : UIWindowNGUI 
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

    private void OnQuitClicked(object sender, UIMouseClick.ClickArgs e)
    {
        UIWindowManager.Singleton.GetUIWindow<UIQuitConfirm>().ShowWindow();
    }
}
