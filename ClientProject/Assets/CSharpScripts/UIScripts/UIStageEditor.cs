using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindowNGUI 
{
    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
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
        GlobalVars.EditState = TEditState.None;
    }
}
