//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using UnityEngine;
using Unibill.Impl;

namespace Uniject {
    public interface IUtil {
        T[] getAnyComponentsOfType<T>() where T : class;
        string loadedLevelName();
        RuntimePlatform Platform { get; }

        bool IsEditor { get; }
        string persistentDataPath { get; }
        DateTime currentTime { get; }
    }
}
