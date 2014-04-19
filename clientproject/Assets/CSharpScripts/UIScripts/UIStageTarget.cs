using UnityEngine;
using System.Collections;

public class UIStageTarget : UIWindow 
{
	Transform m_collectBoard;
	Transform m_jellyBoard;
	Transform m_scoreBoard;
	Transform m_nutBoard;

    Transform jelly2Board;

    Transform m_stepLimitBoard;         //步数限制
    Transform m_timeLimitBoard;         //时间限制

    Transform m_gameFailedBoard;
    Transform m_resortBoard;
    Transform m_autoResortBoard;

    public enum TargetMode
    {
        StageTarget,
        AutoResort,
        GameFailed,
    }

    public TargetMode Mode;

	NumberDrawer nut1Label;
    NumberDrawer nut2Label;
	UISprite nut1Icon;
	UISprite nut2Icon;
    UISprite nutSplash;

    UISprite m_background;

    UILabel[] collectLabel = new UILabel[3];
	UISprite [] collectIcon = new UISprite [3];

    public override void OnCreate()
    {
        base.OnCreate();

		m_collectBoard = mUIObject.transform.FindChild("CollectBoard");
		m_jellyBoard = mUIObject.transform.FindChild("JellyBoard");
        m_scoreBoard = mUIObject.transform.FindChild("ScoreBoard");
		m_nutBoard = mUIObject.transform.FindChild("NutBoard");

        jelly2Board = m_jellyBoard.FindChild("JellyDoubleBoard");

        m_gameFailedBoard = mUIObject.transform.FindChild("FailedBoard");
        m_resortBoard = mUIObject.transform.FindChild("ResortBoard");
        m_autoResortBoard = mUIObject.transform.FindChild("AutoResortBoard");

        m_stepLimitBoard = mUIObject.transform.FindChild("StepLimitBoard");
        m_timeLimitBoard = mUIObject.transform.FindChild("TimeLimitBoard");

        nut1Label = GetChildComponent<NumberDrawer>("NutCount1");
        nut2Label = GetChildComponent<NumberDrawer>("NutCount2");
		nut1Icon = GetChildComponent<UISprite>("NutIcon1");
		nut2Icon = GetChildComponent<UISprite>("NutIcon2");
        nutSplash = GetChildComponent<UISprite>("NutSplash");

        m_background = GetChildComponent<UISprite>("Background");

        for (int i = 0; i < 3; ++i )
        {
            collectLabel[i] = GetChildComponent<UILabel>("CollectCount" + (i + 1));
            collectIcon[i] = GetChildComponent<UISprite>("Icon" + (i + 1));
        }
    }
    public override void OnShow()
    {
        base.OnShow();


        m_collectBoard.gameObject.SetActive(false);
        m_jellyBoard.gameObject.SetActive(false);
        m_scoreBoard.gameObject.SetActive(false);
        m_nutBoard.gameObject.SetActive(false);
        m_gameFailedBoard.gameObject.SetActive(false);
        m_resortBoard.gameObject.SetActive(false);
        m_autoResortBoard.gameObject.SetActive(false);
        m_stepLimitBoard.gameObject.SetActive(false);
        m_timeLimitBoard.gameObject.SetActive(false);
		
		Transform curBoard;
        if (Mode == TargetMode.StageTarget)
        {
            m_background.spriteName = "TargetBar";
            StageData stage = GlobalVars.CurStageData;
            if (stage.Target == GameTarget.GetScore)
            {
                m_scoreBoard.gameObject.SetActive(true);
                curBoard = m_scoreBoard;
                NumberDrawer scoreLabel = UIToolkits.FindComponent<NumberDrawer>(curBoard, "Score");
                scoreLabel.SetNumberRapid(stage.StarScore[0]);
            }
            else if (stage.Target == GameTarget.ClearJelly)
            {
                m_jellyBoard.gameObject.SetActive(true);
                curBoard = m_jellyBoard;

                NumberDrawer jellyLabel = GetChildComponent<NumberDrawer>("JellyCount");
                jellyLabel.SetNumberRapid(stage.GetSingleJellyCount());

                if (stage.GetDoubleJellyCount() > 0)        //若有双层冰块
                {
                    m_jellyBoard.LocalPositionX(-30.0f);
                    NumberDrawer jelly2Label = GetChildComponent<NumberDrawer>("Jelly2Count");
                    jelly2Label.SetNumberRapid(stage.GetDoubleJellyCount());

                    jelly2Board.gameObject.SetActive(true);
                }
                else
                {
                    m_jellyBoard.LocalPositionX(86.0f);
                    jelly2Board.gameObject.SetActive(false);
                }
            }
            else if (stage.Target == GameTarget.BringFruitDown)
            {
                m_nutBoard.gameObject.SetActive(true);
                curBoard = m_nutBoard;

                if (stage.Nut1Count > 0)
                {
                    nut1Label.SetNumberRapid(stage.Nut1Count);

                    nut1Label.gameObject.SetActive(true);
                    nut1Icon.gameObject.SetActive(true);
                    nut1Label.LocalPositionX(-14);
                }
                else
                {
                    nut1Label.gameObject.SetActive(false);
                    nut1Icon.gameObject.SetActive(false);
                }

                if (stage.Nut2Count > 0)
                {
                    nut2Label.SetNumberRapid(stage.Nut2Count);

                    nut2Label.gameObject.SetActive(true);
                    nut2Icon.gameObject.SetActive(true);
                    nut2Label.LocalPositionX(-14);
                }
                else
                {
                    nut2Label.gameObject.SetActive(false);
                    nut2Icon.gameObject.SetActive(false);
                }

                if (stage.Nut1Count > 0 && stage.Nut2Count > 0)
                {
                    nutSplash.gameObject.SetActive(true);

                    nut1Label.LocalPositionX(-101);
                    nut2Label.LocalPositionX(71);
                }
                else
                {
                    nutSplash.gameObject.SetActive(false);
                }
            }
            else 		//Collect
            {
                m_collectBoard.gameObject.SetActive(true);
                curBoard = m_collectBoard;
                int collectCount = 0;

                for (int i = 0; i < 3; ++i)
                {
                    if (stage.CollectCount[i] > 0)
                    {
                        ++collectCount;
                        collectLabel[i].gameObject.SetActive(true);
                        collectIcon[i].gameObject.SetActive(true);

                        collectLabel[i].text = stage.CollectCount[i].ToString();

                        switch (GlobalVars.CurStageData.CollectSpecial[i])
                        {
                            case TSpecialBlock.ESpecial_Normal:
                                {
                                    collectIcon[i].spriteName = "Item" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                                }
                                break;
                            case TSpecialBlock.ESpecial_NormalPlus6:
                                {
                                    collectIcon[i].spriteName = "TimeAdded" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                                }
                                break;
                            case TSpecialBlock.ESpecial_EatLineDir0:
                                collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_3";
                                break;
                            case TSpecialBlock.ESpecial_EatLineDir1:
                                collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_1";
                                break;
                            case TSpecialBlock.ESpecial_EatLineDir2:
                                collectIcon[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_2";
                                break;
                            case TSpecialBlock.ESpecial_Bomb:
                                collectIcon[i].spriteName = "Bomb" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                                break;
                            case TSpecialBlock.ESpecial_EatAColor:
                                collectIcon[i].spriteName = "Rainbow";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        collectLabel[i].gameObject.SetActive(false);
                        collectIcon[i].gameObject.SetActive(false);
                    }
                }

                if (collectCount == 3)
                {
                    m_collectBoard.LocalPositionX(0);
                }
                else if (collectCount == 2)
                {
                    m_collectBoard.LocalPositionX(67);
                }
                else
                {
                    m_collectBoard.LocalPositionX(120);
                }
            }

            if (stage.StepLimit > 0)
            {
                m_stepLimitBoard.gameObject.SetActive(true);
                NumberDrawer stepLabel = UIToolkits.FindComponent<NumberDrawer>(m_stepLimitBoard, "StepNum");
                stepLabel.SetNumberRapid(stage.StepLimit);
            }
            else
            {
                m_timeLimitBoard.gameObject.SetActive(true);
                NumberDrawer timeLabel = UIToolkits.FindComponent<NumberDrawer>(m_timeLimitBoard, "TimeNum");
                timeLabel.SetNumberRapid(stage.TimeLimit);
            }
        }
        else
        {
            if (Mode == TargetMode.GameFailed)
            {
                m_background.spriteName = "FailedBar";
                m_gameFailedBoard.gameObject.SetActive(true);
            }
            else if (Mode == TargetMode.AutoResort)
            {
                m_background.spriteName = "ResortBar";
                m_autoResortBoard.gameObject.SetActive(true);
            }
        }
    }
}
