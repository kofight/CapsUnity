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

        for (int i = 0; i < GameLogic.ColorCount; ++i)
        {
			int color = i + 1;
            AddChildComponentMouseClick("ChangeColor" + color, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.ChangeColor;
                GlobalVars.EditingColor = (TBlockColor)(color);
            });
        }

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

        for (int i = 0; i <= (int)GameTarget.ClearJelly; ++i)
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
