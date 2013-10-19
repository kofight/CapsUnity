using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindowNGUI
{
    bool editingPortal = false;         //编辑传送门的标记

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

        AddChildComponentMouseClick("ResortBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.CurGameLogic.AutoResort();
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

        //开始编辑关卡旗标事件
        AddChildComponentMouseClick("EditStageBlock", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditState = TEditState.EditStageGrid;
            GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
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

        for (int i = 1; i <= 10; ++i)
        {
            AddChildComponentMouseClick("GridFlag" + i, delegate(object sender, UIMouseClick.ClickArgs e)
            {
                GlobalVars.EditState = TEditState.EditStageGrid;
                GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
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

        AddChildComponentMouseClick("EditPortal", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 1;
        });

        AddChildComponentMouseClick("EditPortalInvisible", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 0;
        });

        AddChildComponentMouseClick("GridNoneBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = 0;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJellyBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Jelly) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJelly2Btn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = ((int)GridFlag.JellyDouble) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridNormalBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = (int)GridFlag.GenerateCap;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridStoneBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Stone) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridChocolateBtn", delegate(object sender, UIMouseClick.ClickArgs e)
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Chocolate) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });
    }

    void RefreshStageGridFlagCheckBoxes(int gridFlags)       //刷新当前编辑的GridFlag对应的CheckBox界面
    {
        for (int i = 0; i < 10; ++i )
        {
            UICheckbox checkBox = UIToolkits.FindComponent<UICheckbox>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if ((gridFlags & 1 << i) > 0)
            {
                checkBox.Set(true);
            }
            else
            {
                checkBox.Set(false);
            }
        }
    }

    int GetGridFlagsFromCheckBoxes()
    {
        int gridFlags = 0;
        for (int i = 0; i < 10; ++i)
        {
            UICheckbox checkBox = UIToolkits.FindComponent<UICheckbox>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if (checkBox.isChecked)
            {
                gridFlags |= 1 << i;
            }
        }
        return gridFlags;
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
        GlobalVars.EditStageMode = false;
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
