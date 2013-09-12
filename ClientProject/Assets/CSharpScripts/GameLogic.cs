using UnityEngine;
using System.Collections;

public enum TGameState
{
    EGameState_Waitting,
    EGameState_Playing,
    EGameState_EndWaitingDropingDown,			//游戏已经满足结束条件，等待下落结束
    EGameState_PlayingEndAni,
    EGameState_End,
};

enum TDirection
{
    EDir_Up,
    EDir_UpRight,
    EDir_DownRight,
    EDir_Down,
    EDir_LeftDown,
    EDir_LeftUp,
};

struct Paticle
{
    int xSpeed;
    int ySpeed;
    int startX;
    int startY;
    long startTime;
    float gravity;			//重力值
    float lifeTime;			//生命期
    bool changeAlphaByLifeTime;		//是否改变Alpha
    TBlockColor color;
    UISprite pBlockSprite;
};

public class GameLogic {

	
}
