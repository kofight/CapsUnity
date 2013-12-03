using UnityEngine;
using System.Collections;

public class UIStageInfo : UIWindow 
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

        NumberDrawer number = GetChildComponent<NumberDrawer>("StageTarget");
        number.SetNumber(GlobalVars.CurStageData.StarScore[2]);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnCloseClicked()
    {
        HideWindow();
    }

    private void OnPlayClicked()
    {
        HideWindow(delegate()
        {
            if (CapsApplication.Singleton.CurStateEnum != StateEnum.Game)
            {
                UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
                CapsApplication.Singleton.ChangeState((int)StateEnum.Game);
            }
            else
            {
                GlobalVars.CurGameLogic.Init();
                GlobalVars.CurGameLogic.StartGame();
            }
        });
    }
}
