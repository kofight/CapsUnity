// SDK Version:1.0.13.4

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class TalkingDataPlugin {
	#if UNITY_IPHONE
		/* Interface to native implementation */
		
		[DllImport ("__Internal")]
		private static extern void _tdSessionStarted(string appKey, string channelId);
		
		[DllImport ("__Internal")]
		private static extern void _tdSetExceptionReportEnabled(bool enable);
		
		[DllImport ("__Internal")]
		private static extern void _tdSetLatitude(double latitude, double longitude);
		
		[DllImport ("__Internal")] 
		private static extern void _tdSetLogEnabled(bool enable);
		
		[DllImport ("__Internal")]
		private static extern void _tdTrackEvent(string eventId);
		
		[DllImport ("__Internal")]
		private static extern void _tdTrackEventLabel(string eventId, string eventLabel);
		
		[DllImport ("__Internal")]
		private static extern void _tdTrackEventParameters(string eventId, string eventLabel, 
			string[] keys, string[] stringValues, double[] numberValues, int count);
	
		[DllImport ("__Internal")]
		private static extern void _tdTrackPageBegin(string pageName);
		
		[DllImport ("__Internal")]
		private static extern void _tdTrackPageEnd(string pageName);
		
	#endif
	
	/* Public interface for use inside C# / JS code */
	
	
	public static void SetLocation(double latitude, double longitude)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor)
			_tdSetLatitude(latitude, longitude);
		#endif
	}
	
	public static void SetLogEnabled(bool enable)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor)
			_tdSetLogEnabled(enable);
		#endif
		#if UNITY_ANDROID
		AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
		tCAgent.SetStatic("DEBUG", true);
		#endif
	}
	
	/* Public interface for use inside C# / JS code */
	public static void SessionStarted(string appKey, string channelId)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor){
				Debug.Log("ios start");
				_tdSessionStarted(appKey, channelId);
		}
		#endif
		#if UNITY_ANDROID
				AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
		    	AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
				Debug.Log("android start");
				tCAgent.CallStatic("onResume", currActivity, appKey, channelId);
		#endif
			
	}
	
	
	
	public static void SessionStoped()
	{
		// Call plugin only when running on real device
			#if UNITY_ANDROID
			Debug.Log("android stop");
		   	AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
	    	AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			tCAgent.CallStatic("onPause", currActivity);
			#endif
	}
	
	public static void SetExceptionReportEnabled(bool enable)
	{
		// Call plugin only when running on real device
			#if UNITY_IPHONE
					if (Application.platform != RuntimePlatform.OSXEditor){
						_tdSetExceptionReportEnabled(enable);
					}
			#endif
			#if UNITY_ANDROID
					AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
					tCAgent.CallStatic("setReportUncaughtExceptions", enable);
			#endif
		
	}
	
	public static void TrackEvent(string eventId)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
			if (Application.platform != RuntimePlatform.OSXEditor){
				_tdTrackEvent(eventId);
			}
		#endif
		#if UNITY_ANDROID
			AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
	    	AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
		
			tCAgent.CallStatic("onEvent", currActivity, eventId);	
		#endif	
	    	
	}
	
	public static void TrackEventWithLabel(string eventId, string eventLabel)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
			if (Application.platform != RuntimePlatform.OSXEditor){
				_tdTrackEventLabel(eventId, eventLabel);
			}
		#endif
		#if UNITY_ANDROID
			AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
	    	AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			tCAgent.CallStatic("onEvent", currActivity, eventId, eventLabel);	
		#endif	
	}
	
	public static void TrackEventWithParameters(string eventId, string eventLabel, 
		Dictionary<string, object> parameters)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
			if (Application.platform != RuntimePlatform.OSXEditor) 
				{
					if (parameters != null && parameters.Count > 0 && parameters.Count <= 10) 
					{
						int count = parameters.Count;
						string []keys = new string[count];
						string []stringValues = new string[count];
						double []numberValues = new double[count];
						int index = 0;
						foreach (KeyValuePair<string, object> kvp in parameters)
						{
							
							if (kvp.Value is string) 
							{
								keys[index] = kvp.Key;
								stringValues[index] = (string)kvp.Value;
							}
							else
							{
								try
								{
								  	double tmp = System.Convert.ToDouble(kvp.Value);
								  	numberValues[index] = tmp;
									keys[index] = kvp.Key;
								}
								catch(System.Exception)
								{
									count--;
								  	continue;
								}
							}
							
							index++;
		
						}
						
						_tdTrackEventParameters(eventId, eventLabel, keys, stringValues, numberValues, count);
					}
					else
					{
						_tdTrackEventLabel(eventId, eventLabel);
					}
					
				}
		#endif	
		#if UNITY_ANDROID
				if (parameters != null && parameters.Count > 0 && parameters.Count <= 10) 
			{
				int count = parameters.Count;
				string []keys = new string[count];
				string []stringValues = new string[count];
				double []numberValues = new double[count];
				int index = 0;
				foreach (KeyValuePair<string, object> kvp in parameters)
				{
					
					if (kvp.Value is string) 
					{
						keys[index] = kvp.Key;
						stringValues[index] = (string)kvp.Value;
					}
					else
					{
						try
						{
						  	double tmp = System.Convert.ToDouble(kvp.Value);
						  	numberValues[index] = tmp;
							keys[index] = kvp.Key;
						}
						catch(System.Exception)
						{
							count--;
						  	continue;
						}
					}
					
					index++;

				}
			AndroidJavaClass tCAgent = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
	    	AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
				tCAgent.CallStatic("onEvent", currActivity, eventId, eventLabel, keys, stringValues, numberValues, count);
			}
		#endif
		}
	
	public static void TrackPageBegin(string pageName)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor)
			_tdTrackPageBegin(pageName);
		#endif
	}
	
	public static void TrackPageEnd(string pageName)
	{
		// Call plugin only when running on real device
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor)
			_tdTrackPageEnd(pageName);
		#endif
	}
}
