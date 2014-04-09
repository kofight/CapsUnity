using UnityEngine;
using System.Collections;

public class UIFailedFTUE : UIWindow 
{
	public UIWindow.WindowEffectFinished RestartFunc;
	public UIWindow.WindowEffectFinished NeedHelpFunc;
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("NeedHelpBtn", delegate()
		{
			if(NeedHelpFunc != null)NeedHelpFunc();
		});
        AddChildComponentMouseClick("TryAgainBtn", delegate()
		{
			if(RestartFunc != null)RestartFunc();
		});
    }
}
