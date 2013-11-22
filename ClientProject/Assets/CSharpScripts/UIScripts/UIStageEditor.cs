using UnityEngine;
using System.Collections;

public class UIStageEditor : UIWindow
{
    bool editingPortal = false;         //编辑传送门的标记
    UILabel m_portalEditLabel;
    UILabel m_seedLabel;

    public override void OnCreate()
    {
        base.OnCreate();

        m_portalEditLabel = GetChildComponent<UILabel>("EditPortalTip");
        m_seedLabel = GetChildComponent<UILabel>("SeedLabel");

        AddChildComponentMouseClick("SaveBtn", OnSaveClicked);

        AddChildComponentMouseClick("RevertBtn", OnLoadClicked);

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);

        AddChildComponentMouseClick("RestartBtn", delegate()
            {
                GlobalVars.CurGameLogic.ClearGame();
				GlobalVars.CurGameLogic.Init();
                GlobalVars.CurGameLogic.StartGame();
            });

        AddChildComponentMouseClick("ResortBtn", delegate()
        {
            GlobalVars.CurGameLogic.AutoResort();
        });

        AddChildComponentMouseClick("HelpBtn", delegate()
        {
            GlobalVars.CurGameLogic.ClearHelpPoint();
            GlobalVars.CurGameLogic.Help();
            GlobalVars.CurGameLogic.ShowHelpAnim();
        });

        AddChildComponentMouseClick("ReSeedBtn", delegate()
        {
            GlobalVars.CurGameLogic.ClearGame();
			GlobalVars.CurGameLogic.Init();
			GlobalVars.CurGameLogic.PlayingStageData.Seed = (int)(Time.realtimeSinceStartup * 1000 % 1000);
            GlobalVars.CurGameLogic.StartGame();
            m_seedLabel.text = GlobalVars.CurGameLogic.PlayingStageData.Seed.ToString();
        });

        for (int i = 0; i < GameLogic.TotalColorCount; ++i)
        {
			int color = i + 1;
            AddChildComponentMouseClick("ChangeColor" + color, delegate()
            {
                GlobalVars.EditState = TEditState.ChangeColor;
                GlobalVars.EditingColor = (TBlockColor)(color);
            });
        }

        AddChildComponentMouseClick("ChangeToNut1", delegate()
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut1;
        });

        AddChildComponentMouseClick("ChangeToNut2", delegate()
        {
            GlobalVars.EditState = TEditState.ChangeColor;
            GlobalVars.EditingColor = TBlockColor.EColor_Nut2;
        });

        //开始编辑关卡旗标事件
        AddChildComponentMouseClick("EditStageBlock", delegate()
        {
            GlobalVars.EditState = TEditState.EditStageGrid;
            GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
        });


        for (int i = 0; i <= (int)TSpecialBlock.ESpecial_EatAColor; ++i)
        {
            int special = i;
            AddChildComponentMouseClick("ChangeSpecial" + i, delegate()
            {
                GlobalVars.EditState = TEditState.ChangeSpecial;
                GlobalVars.EditingSpecial = (TSpecialBlock)special;
            });
        }

        for (int i = 1; i <= 10; ++i)
        {
            AddChildComponentMouseClick("GridFlag" + i, delegate()
            {
                GlobalVars.EditState = TEditState.EditStageGrid;
                GlobalVars.EditingGrid = GetGridFlagsFromCheckBoxes();
            });
        }

        for (int i = 0; i <= (int)GameTarget.GetScore; ++i)
        {
            int targetType = i;
            AddChildComponentMouseClick("TargetMode" + i, delegate()
            {
                GlobalVars.CurGameLogic.PlayingStageData.Target = (GameTarget)targetType;
            });
        }

        AddChildComponentMouseClick("EatBlock", delegate()
            {
                GlobalVars.EditState = TEditState.Eat;
            });

        AddChildComponentMouseClick("EditPortal", delegate()
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 1;
            GlobalVars.EditingPortalTip = "Edit Portal: 选择Pos1";
        });

        AddChildComponentMouseClick("EditPortalInvisible", delegate()
        {
            GlobalVars.EditState = TEditState.EditPortal;
            GlobalVars.EditingPortal = new Portal();
            GlobalVars.EditingPortal.flag = 0;
            GlobalVars.EditingPortalTip = "Edit Portal: 选择Pos1";
        });

        AddChildComponentMouseClick("GridNoneBtn", delegate()
        {
            GlobalVars.EditingGrid = 0;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJellyBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Jelly) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridJelly2Btn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.JellyDouble) | ((int)GridFlag.GenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("ClearAllPortalBtn", delegate()
        {
            GlobalVars.CurGameLogic.PlayingStageData.PortalFromMap.Clear();
            GlobalVars.CurGameLogic.PlayingStageData.PortalToMap.Clear();
            for (int i = 0; i < GameLogic.BlockCountX; ++i )
            {
                for (int j = 0; j < GameLogic.BlockCountY; ++j )
                {
                    GlobalVars.CurGameLogic.PlayingStageData.ClearFlag(i, j, GridFlag.Portal);
                    GlobalVars.CurGameLogic.PlayingStageData.ClearFlag(i, j, GridFlag.PortalEnd);
                    GlobalVars.CurGameLogic.PlayingStageData.ClearFlag(i, j, GridFlag.PortalStart);
                }
            }
        });


        AddChildComponentMouseClick("GridNormalBtn", delegate()
        {
            GlobalVars.EditingGrid = (int)GridFlag.GenerateCap;
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridStoneBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Stone) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });

        AddChildComponentMouseClick("GridChocolateBtn", delegate()
        {
            GlobalVars.EditingGrid = ((int)GridFlag.Chocolate) | ((int)GridFlag.NotGenerateCap);
            RefreshStageGridFlagCheckBoxes(GlobalVars.EditingGrid);
        });
    }

    void RefreshStageGridFlagCheckBoxes(int gridFlags)       //刷新当前编辑的GridFlag对应的CheckBox界面
    {
        for (int i = 0; i < 10; ++i )
        {
            UIToggle checkBox = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if ((gridFlags & 1 << i) > 0)
            {
                checkBox.value = true;
            }
            else
            {
                checkBox.value = false;
            }
        }
    }

    int GetGridFlagsFromCheckBoxes()
    {
        int gridFlags = 0;
        for (int i = 0; i < 10; ++i)
        {
            UIToggle checkBox = UIToolkits.FindComponent<UIToggle>(mUIObject.transform, "GridFlag" + (i + 1));      //找到CheckBox
            if (checkBox.value)
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
        input.value = GlobalVars.CurStageNum.ToString();

        input = GetChildComponent<UIInput>("StepLimit");
        input.value = GlobalVars.CurGameLogic.PlayingStageData.StepLimit.ToString();

        input = GetChildComponent<UIInput>("TimeLimit");
        input.value = GlobalVars.CurGameLogic.PlayingStageData.TimeLimit.ToString();

        input = GetChildComponent<UIInput>("ColorCount");
        input.value = GlobalVars.CurGameLogic.PlayingStageData.ColorCount.ToString();

        input = GetChildComponent<UIInput>("Nut1");
        input.value = GlobalVars.CurStageData.Nut1Count.ToString();

        input = GetChildComponent<UIInput>("Nut2");
        input.value = GlobalVars.CurStageData.Nut2Count.ToString();

        input = GetChildComponent<UIInput>("NutInitCount");
        input.value = GlobalVars.CurStageData.NutInitCount.ToString();

        input = GetChildComponent<UIInput>("NutMaxCount");
        input.value = GlobalVars.CurStageData.NutMaxCount.ToString();

        input = GetChildComponent<UIInput>("NutStep");
        input.value = GlobalVars.CurStageData.NutStep.ToString();

        input = GetChildComponent<UIInput>("PlusInitCount");
        input.value = GlobalVars.CurStageData.PlusInitCount.ToString();

        input = GetChildComponent<UIInput>("PlusMaxCount");
        input.value = GlobalVars.CurStageData.PlusMaxCount.ToString();

        input = GetChildComponent<UIInput>("PlusStep");
        input.value = GlobalVars.CurStageData.PlusStep.ToString();

        input = GetChildComponent<UIInput>("PlusStartTime");
        input.value = GlobalVars.CurStageData.PlusStartTime.ToString();

        for (int i = 0; i < 3; ++i )
        {
            input = GetChildComponent<UIInput>("Star" + (i +1));
            input.value = GlobalVars.CurGameLogic.PlayingStageData.StarScore[i].ToString();
        }

        UIToggle targetCheck = GetChildComponent<UIToggle>("TargetMode" + (int)GlobalVars.CurGameLogic.PlayingStageData.Target);
        targetCheck.value = true;

        m_seedLabel.text = GlobalVars.CurGameLogic.PlayingStageData.Seed.ToString();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (GlobalVars.EditState == TEditState.EditPortal)
        {
            m_portalEditLabel.text = GlobalVars.EditingPortalTip;
        }
        else
        {
            m_portalEditLabel.text = string.Empty;
        }
    }

    private void OnCloseClicked()
    {
        HideWindow();
        GlobalVars.EditState = TEditState.None;
        GlobalVars.EditStageMode = false;
    }

    private void OnSaveClicked()
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

        input = GetChildComponent<UIInput>("NutInitCount");
        int nutInitCount = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.NutInitCount = nutInitCount;

        input = GetChildComponent<UIInput>("NutMaxCount");
        int nutMaxCount = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.NutMaxCount = nutMaxCount;

        input = GetChildComponent<UIInput>("NutStep");
        int nutStep = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.NutStep = nutStep;

        input = GetChildComponent<UIInput>("PlusInitCount");
        int plusInitCount = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.PlusInitCount = plusInitCount;

        input = GetChildComponent<UIInput>("PlusMaxCount");
        int plusMaxCount = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.PlusMaxCount = plusMaxCount;

        input = GetChildComponent<UIInput>("PlusStartTime");
        int plusStartTime = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.PlusStartTime = plusStartTime;

        input = GetChildComponent<UIInput>("PlusStep");
        int plusStep = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.PlusStep = plusStep;

        input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData.SaveStageData(levelNum);
        GlobalVars.CurStageNum = levelNum;

        GlobalVars.CurStageData.LoadStageData(levelNum);
    }

    private void OnLoadClicked()
    {
        UIInput input = GetChildComponent<UIInput>("LevelInput");
        int levelNum = (int)System.Convert.ChangeType(input.text, typeof(int));
        GlobalVars.CurGameLogic.PlayingStageData = StageData.CreateStageData();
        GlobalVars.CurGameLogic.PlayingStageData.LoadStageData(levelNum);
    }
}
