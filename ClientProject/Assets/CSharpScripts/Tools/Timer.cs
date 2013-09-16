using UnityEngine;
using System.Collections;

public enum TimerEnum
{
	ERunning,
	EStop,
};


public class Timer {

    public static long millisecondNow()
    {
        return (long)(s_currentTime * 1000);
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
