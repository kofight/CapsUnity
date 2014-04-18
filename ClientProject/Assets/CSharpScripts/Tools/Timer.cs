using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TimerEnum
{
	ERunning,
	EStop,
};

struct DelayAction
{
    public float startTime;
    public System.Action action;
}


public class Timer {

    static LinkedList<DelayAction> m_delayActoinList = new LinkedList<DelayAction>();
    static float m_lastUpdateRealTime;
    static float m_RealTime;

    public static long millisecondNow()
    {
        return (long)(s_currentTime * 1000);
    }

    public static float GetRealTime()   //每帧刷新的时间
    {
        return m_RealTime;                   
    }

    public static float GetFixedTime()              //固定间隔的时间
    {
        return s_currentTime;
    }

    public static void AddDelayFunc(float time, System.Action action)
    {
        DelayAction act = new DelayAction();
        act.startTime = s_currentTime + time;
        act.action = action;
        m_delayActoinList.AddLast(act);
    }

    public static void FixedUpdate()
    {
        foreach (DelayAction act in m_delayActoinList)
        {
            if (s_currentTime > act.startTime)
            {
                act.action();
                m_delayActoinList.Remove(act);
                break;
            }
        }

        Timer.s_currentTime += Time.fixedDeltaTime;		//更新时间
        m_lastUpdateRealTime = Time.realtimeSinceStartup;
    }

    public static void Update()
    {
        m_RealTime = s_currentTime + (Time.realtimeSinceStartup - m_lastUpdateRealTime) * Time.timeScale;
    }

	public static float s_currentTime = 0;
	public Timer()
    {
        m_startTime = -1;
    }

    public long GetTime()
    {
        return (int)(GetRealTime() * 1000) - m_startTime;
    }

    public void Play()
    {
        m_startTime = millisecondNow();
    }

    public void Stop()
    {
        m_startTime = -1;
    }

	public TimerEnum GetState()
    {
        if (m_startTime == -1)
	    {
		    return TimerEnum.EStop;
	    }
	    return TimerEnum.ERunning;
    }

	long m_startTime;
};
