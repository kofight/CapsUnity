using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindowNGUI
{
    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("SaveBtn", OnSaveClicked);

        AddChildComponentMouseClick("RevertBtn", OnLoadClicked);

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);

        AddChildComponentMouseClick("RestartBtn", delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.CurGameLogic.ClearGame();
                GlobalVars.CurGameLogic.StartGame();
            });

        for (int i = 0; i < GameLogic.TotalColorCount; ++i)
        {
			int color = i + 1;
            AddChildComponentMouseClick("ChangeColor" + color, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.ChangeColor;
                GlobalVars.EditingColor = (TBlockColor)(color);
            });
        }

        AddChildComponentMouseClick("ChangeToNut1", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut1;
        });

        AddChildComponentMouseClick("ChangeToNut2", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut2;
        });

        for (int i = 0; i <= (int)TSpecialBlock.ESpecial_EatAColor; ++i)
        {
            int special = i;
            AddChildComponentMouseClick("ChangeSpecial" + i, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.ChangeSpecial;
                GlobalVars.EditingSpecial = (TSpecialBlock)special;
            });
        }

        for (int i = 0; i <= (int)TGridType.JellyDouble; ++i)
        {
            int grid = i;
            AddChildComponentMouseClick("StageGrid" + i, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.EditStageGrid;
                GlobalVars.EditingGrid = (TGridType)grid;
            });
        }

        for (int i = 0; i <= (int)TGridBlockType.Cage; ++i)
        {
            int gridBlock = i;
            AddChildComponentMouseClick("StageBlock" + i, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.EditStageBlock;
                GlobalVars.EditingGridBlock = (TGridBlockType)gridBlock;
            });
        }

        for (int i = 0; i <= (int)GameTarget.GetScore; ++i)
        {
            int targetType = i;
            AddChildComponentMouseClick("TargetMode" + i, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.CurGameLogic.PlayingStageData.Target = (GameTarget)targetType;
            });
        }

        AddChildComponentMouseClick("EatBlock", delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.Eat;
            });
    }

    public override void OnShow()
    {
        base.OnShow();
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        input.text = GlobalVars.CurStageNum.ToString();

        input = GetChildComponent<UIInput>("StepLimit");
        input.text = GlobalVars.CurGameLogic.PlayingStageData.StepLimit.ToString();

        input = GetChildComponent<UIInput>("TimeLimit");
        input.text = GlobalVars.CurGameLogic.PlayingStageData.TimeLimit.ToString();

        input = GetChildComponent<UIInput>("ColorCount");
        input.text = GlobalVars.CurGameLogic.PlayingStageData.ColorCount.ToString();

        input = GetChildComponent<UIInput>("Nut1");
        input.text = GlobalVars.CurStageData.Nut1Count.ToString();

        input = GetChildComponent<UIInput>("Nut2");
        input.text = GlobalVars.CurStageData.Nut2Count.ToString();

        for (int i = 0; i < 3; ++i )
        {
            input = GetChildComponent<UIInput>("Star" + (i +1));
            input.text = GlobalVars.CurGameLogic.PlayingStageData.StarScore[i].ToString();
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnCloseClicked(object sender, UIMouseClick.ClickArgs e)
    {
        HideWindow();
        GlobalVars.EditState = TEditState.None;
    }

    private void OnSaveClicked(object sender, UIMouseClick.ClickArgs e)
    {
        UIInput input = GetChildComponent<UIInput>("StepLimit");
        int stepLimit = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.StepLimit = stepLimit;



        input = GetChildComponent<UIInput>("TimeLimit");
        int timeLimit = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.TimeLimit = timeLimit;

        input = GetChildComponent<UIInput>("ColorCount");
        int colorCount = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.ColorCount = colorCount;

        for (int i = 0; i < 3; ++i )
        {
            input = GetChildComponent<UIInput>("Star" + (i + 1));
            int score = (int)System.Convert.ChangeType(input.text, typeof(int));
            GlobalVars.CurGameLogic.PlayingStageData.StarScore[i] = score;
        }

        input = GetChildComponent<UIInput>("Nut1");
        int nut1Count = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.Nut1Count = nut1Count;

        input = GetChildComponent<UIInput>("Nut2");
        int nut2Count = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.Nut2Count = nut2Count;

        input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.SaveStageData(levelNum);
        GlobalVars.CurStageNum = levelNum;

        GlobalVars.CurStageData.LoadStageData(levelNum);
    }

    private void OnLoadClicked(object sender, UIMouseClick.ClickArgs e)
    {
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData = StageData.CreateStageData();
        GlobalVars.CurGameLogic.PlayingStageData.LoadStageData(levelNum);
    }
}
