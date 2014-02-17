//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;

namespace Uniject.Impl {
    public class UnityLogger : ILogger{
        #region ILogger implementation
        public void LogWarning (string message, params object[] formatArgs) {
            UnityEngine.Debug.LogWarning(string.Format(message, formatArgs));
        }
        #endregion

        public string prefix { get; set; }

    	#region ILogger implementation
    	public void Log(string message) {
            UnityEngine.Debug.Log(formatMessage(message)); // Removed to avoid filling the Unity log. Uncomment to get complete tracing information.
    	}

        public void Log(string message, object[] args) {
            Log(string.Format(message, args));
        }

        public void LogError(string message, params object[] formatArgs) {
            UnityEngine.Debug.LogError(formatMessage(string.Format(message, formatArgs)));
        }

        private string formatMessage (string message) {
            if (prefix == null) {
                return message;
            }
            return string.Format("{0}: {1}", prefix, message);
        }
    	#endregion
    }
}
