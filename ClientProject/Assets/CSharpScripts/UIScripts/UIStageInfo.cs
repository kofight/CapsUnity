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
        for (int i = 0; i < 3; ++i )
        {
            UISprite star = GetChildComponent<UISprite>("Star" + (i + 1));
            if (GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1] > i)
            {
                star.spriteName = "Star_Large";
            }
            else
            {
				star.spriteName = "Grey_Star_Large";
            }
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (uiWindowState == UIWindowStateEnum.Show)
        {
            UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.Center;
            UIDrawer.Singleton.DrawNumber("ScoreTarget", 0, -92, GlobalVars.CurStageData.StarScore[2], "", 22);
            UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.TopLeft;
        }
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
