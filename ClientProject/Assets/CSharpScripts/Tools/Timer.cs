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

    public static long millisecondNow()
    {
        return (long)(s_currentTime * 1000);
    }

    public static float GetRealTimeSinceStartUp()
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

    public static void Update()
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
    }

	public static float s_currentTime = 0;
	public Timer()
    {
        m_startTime = -1;
    }

    public long GetTime()
    {
        return millisecondNow() - m_startTime;
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
	public void Adjust(long val)
    {
        m_startTime -= val;
    }

	long m_startTime;
};
