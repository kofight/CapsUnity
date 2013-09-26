using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindowNGUI
{
    public GameLogic GameLogicProp { get; set; }

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("SaveBtn", OnSaveClicked);

        AddChildComponentMouseClick("RevertBtn", OnLoadClicked);

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);

        AddChildComponentMouseClick("RestartBtn", delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GameLogicProp.ClearGame();
                GameLogicProp.StartGame();
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
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GameLogicProp.CurStageData.SaveStageData(levelNum);
        GlobalVars.CurStageNum = levelNum;
    }

    private void OnLoadClicked(object sender, UIMouseClick.ClickArgs e)
    {
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GameLogicProp.CurStageData = StageData.CreateStageData();
        GameLogicProp.CurStageData.LoadStageData(levelNum);
    }
}
