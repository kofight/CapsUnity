using UnityEngine;
using System.Collections;

public class UIHowToPlay : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        });
    }
}
