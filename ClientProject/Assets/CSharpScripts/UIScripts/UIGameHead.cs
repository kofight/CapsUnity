using UnityEngine;
using System.Collections;

public class UIGameHead : UIWindowNGUI
{
    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }
	
	public void Reset()
	{

	}
	
    public override void OnUpdate()
    {
        base.OnUpdate();
        UIDrawer.Singleton.DefaultAnchor = UIWindowManager.Anchor.Top;

        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            UIDrawer.Singleton.DrawSprite("Fruit1Icon", -40, 46, "Kiwifruit_Icon");
            UIDrawer.Singleton.DrawNumber("Fruit1Count", 0, 50, GlobalVars.CurGameLogic.PlayingStageData.Nut1Count, "", 24, 1);
            UIDrawer.Singleton.DrawSprite("Fruit1CountSplash", 40, 50, "backslash");
            UIDrawer.Singleton.DrawNumber("Fruit1Total", 60, 50, GlobalVars.CurStageData.Nut1Count, "", 24, 1);

            UIDrawer.Singleton.DrawSprite("Fruit2Icon", 140, 46, "Cherry_Icon");
            UIDrawer.Singleton.DrawNumber("Fruit2Count", 180, 50, GlobalVars.CurGameLogic.PlayingStageData.Nut2Count, "", 24, 1);
            UIDrawer.Singleton.DrawSprite("Fruit2CountSplash", 220, 50, "backslash");
            UIDrawer.Singleton.DrawNumber("Fruit2Total", 240, 50, GlobalVars.CurStageData.Nut2Count, "", 24, 1);
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            UIDrawer.Singleton.DrawSprite("JellyCountIcon", -40, 46, "IceBlock_Icon");
            UIDrawer.Singleton.DrawNumber("Jelly1", -10, 50, GlobalVars.CurGameLogic.PlayingStageData.GetSingleJellyCount(), "", 24, 2);
            UIDrawer.Singleton.DrawSprite("JellyCountSplash", 40, 50, "backslash");
            UIDrawer.Singleton.DrawNumber("Jelly2", 60, 50, GlobalVars.CurStageData.GetSingleJellyCount(), "", 24, 2);


            UIDrawer.Singleton.DrawSprite("DoubleJellyCountIcon", 140, 46, "DoubleIceBlock_Icon");
            UIDrawer.Singleton.DrawNumber("DoubleJelly1", 170, 50, GlobalVars.CurGameLogic.PlayingStageData.GetDoubleJellyCount(), "", 24, 2);
            UIDrawer.Singleton.DrawSprite("DoubleJellyCountSplash", 220, 50, "backslash");
            UIDrawer.Singleton.DrawNumber("DoubleJelly2", 240, 50, GlobalVars.CurStageData.GetDoubleJellyCount(), "", 24, 2);
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            UIDrawer.Singleton.DrawSprite("TargetText", 10, 50, "TargetTextImg");
            UIDrawer.Singleton.DrawNumber("TargetScore", 234, 50, GlobalVars.CurStageData.StarScore[2], "", 24, 7);
        }
    }

    private void OnEditStageClicked(object sender, UIMouseClick.ClickArgs e)
    {
        if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().ShowWindow();        //显示编辑窗口
        GlobalVars.EditStageMode = true;        //编辑器里自动进入关卡编辑模式
    }
}
