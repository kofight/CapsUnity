using UnityEngine;
using System.Collections;

public abstract class State
{
    // Update Game State
    public virtual void Update() { }

    public virtual void OnGUI() { }

    public virtual void DoInitState() { }
    public virtual void DoDeInitState() { }

    public virtual void OnLoadLevel() { }

    public void InitState()
    {
        DoInitState();
    }

    public void DeInitState()
    {
        DoDeInitState();
    }

    public virtual void OnPauseGame()
    {

    }

    public virtual void OnQuitGame()
    {

    }

    public virtual void OnResume()
    {

    }

    public virtual void OnBackKey() { }
}
