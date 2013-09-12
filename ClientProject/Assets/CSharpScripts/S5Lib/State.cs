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

    public virtual void OnTap(Vector2 fingerPos) { }
    public virtual void OnDoubleTap(Vector2 fingerPos) { }
    public virtual void OnDragMove(Vector2 fingerPos, Vector2 delta) { }
    public virtual void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) { }
    public virtual void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta) { }
    public virtual void OnTwoFingerDragMoveEnd(Vector2 fingerPos) { }
	public virtual void OnDrawBegin( Vector2 fingerPos, Vector2 startPos ){ }
	public virtual void OnDrawEnd( Vector2 fingerPos ){ }
	public virtual void OnPressUp( int fingerIndex, Vector2 fingerPos, float timeHeldDown ){}
	public virtual void OnPressDown( int fingerIndex, Vector2 fingerPos ){}
    public virtual void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) { }
    public virtual void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle) { }
    public virtual void OnLongPress(Vector2 fingerPos) { }
    public virtual void OnBackKey() { }
}
