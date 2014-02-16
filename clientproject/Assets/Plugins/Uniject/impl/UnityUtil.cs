//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unibill.Impl;

public class UnityUtil : Uniject.IUtil {

    public T[] getAnyComponentsOfType<T>() where T : class {
        GameObject[] objects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
        List<T> result = new List<T>();
        foreach (GameObject o in objects) {
            foreach (MonoBehaviour mono in o.GetComponents<MonoBehaviour>()) {
                if (mono is T) {
                    result.Add(mono as T);
                }
            }
        }

        return result.ToArray();
    }

    public DateTime currentTime { get { return DateTime.Now; } }

    public string persistentDataPath {
        get { return Application.persistentDataPath; }
    }

    public string loadedLevelName() {
        return Application.loadedLevelName;
    }

    public RuntimePlatform Platform {
        get { return Application.platform; }
    }

    public bool IsEditor {
        get { return Application.isEditor; }
    }

    private static List<RuntimePlatform> PCControlledPlatforms = new List<RuntimePlatform>() {
	    RuntimePlatform.FlashPlayer,
	    RuntimePlatform.LinuxPlayer,
	    RuntimePlatform.NaCl,
        RuntimePlatform.OSXDashboardPlayer,
        RuntimePlatform.OSXEditor,
        RuntimePlatform.OSXPlayer,
        RuntimePlatform.OSXWebPlayer,
        RuntimePlatform.WindowsEditor,
        RuntimePlatform.WindowsPlayer,
        RuntimePlatform.WindowsWebPlayer,
	};

    public static T findInstanceOfType<T>() where T : MonoBehaviour {
        return (T) GameObject.FindObjectOfType(typeof(T));
    }

    public static T loadResourceInstanceOfType<T>() where T : MonoBehaviour {
        return ((GameObject) GameObject.Instantiate(Resources.Load(typeof(T).ToString()))).GetComponent<T>();
    }

    public static bool pcPlatform() {
        return PCControlledPlatforms.Contains(Application.platform);
    }

    public static void DebugLog(string message, params System.Object[] args) {
        try {
            UnityEngine.Debug.Log(string.Format(
                "com.ballatergames.debug - {0}",
                string.Format(message, args))
            );
        } catch (ArgumentNullException a) {
            UnityEngine.Debug.Log(a);
        } catch (FormatException f) {
            UnityEngine.Debug.Log(f);
        }
    }
	
	/*
	 * Returns: xMin, xMax, yMin, yMax, zMin, zMax
	 * */
	public static float[] getFrustumBoundaries(Camera camera) {
		
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return new float[] {
			(-planes[0].normal * planes[0].distance).x,
			(-planes[1].normal * planes[1].distance).x,
			(-planes[5].normal * planes[5].distance).y,
			(-planes[4].normal * planes[4].distance).y,
			(-planes[2].normal * planes[2].distance).z,
			(-planes[3].normal * planes[3].distance).z,
		};
	}
}
