//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
#if UNITY_IPHONE
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Unibill.Editor;


public static class PostbuildProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild (BuildTarget target, string path) {
        if (BuildTarget.iPhone == target) {
            PBXModifier mod = new PBXModifier();
            string pbxproj = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
            if (!File.Exists(pbxproj)) {
                return;
            }
            var lines = mod.applyTo(pbxproj);

            File.WriteAllLines(pbxproj, lines);
        }
    }
}
#endif
