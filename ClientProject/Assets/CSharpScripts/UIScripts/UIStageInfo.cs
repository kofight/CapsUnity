using UnityEngine;
using System.Collections;

public class UIStageInfo : UIWindowNGUI 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("PlayBtn", OnPlayClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnCloseClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
    }

    private void OnPlayClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
		if(CapsApplication.Singleton.CurStateEnum != StateEnum.Game)
		{
			UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
        	CapsApplication.Singleton.ChangeState((int)StateEnum.Game);
		}
        else
        {
            GlobalVars.CurGameLogic.Init();
            GlobalVars.CurGameLogic.StartGame();
        }
    }
}
