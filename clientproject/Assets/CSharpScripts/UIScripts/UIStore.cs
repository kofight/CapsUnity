using UnityEngine;
using System.Collections;
using System;

public class UIStore : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        //AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        //AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow();
        });
    }
    public override void OnShow()
    {
        base.OnShow();
    }
}
