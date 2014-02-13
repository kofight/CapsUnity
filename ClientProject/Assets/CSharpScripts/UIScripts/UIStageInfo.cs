using UnityEngine;
using System.Collections;

public class UIStageInfo : UIWindow 
{
    UILabel m_levelLabel;

    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("PlayBtn", OnPlayClicked);

        m_levelLabel = GetChildComponent<UILabel>("LevelLabel");
    }
    public override void OnShow()
    {
        base.OnShow();

        m_levelLabel.text = System.String.Format(Localization.instance.Get("LevelName"), GlobalVars.CurStageNum);

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

        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void OnCloseClicked()
    {
        HideWindow();

        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();        //返回地图，不需要刷新按钮
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        }
    }

    private void OnPlayClicked()
    {
        GlobalVars.UseHeart();      //使用一颗心

        if (CapsApplication.Singleton.CurStateEnum != StateEnum.Game)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            HideWindow();

            
            UIWindowManager.Singleton.GetUIWindow("UILoading").ShowWindow(
            delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Game);
            }
            );

            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
        }
        else
        {
            HideWindow(delegate()
            {
				UIWindowManager.Singleton.GetUIWindow<UIDialog>().TriggerDialog(GlobalVars.CurStageNum, DialogTriggerPos.StageStart, delegate()
                {
                    GameLogic.Singleton.Init();
                	GameLogic.Singleton.StartGame();
					UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
					UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
					UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();
                });

            });
        }
    }
}
