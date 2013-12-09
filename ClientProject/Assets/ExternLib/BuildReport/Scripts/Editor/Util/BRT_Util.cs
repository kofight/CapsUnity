using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BuildReportTool
{



// per platform identification
// needed to handle special cases
// example: some platforms have a compressed build, some do not.
// also, native plugins are handled differently in each platform.
public enum BuildPlatform
{
	None,

	// -------
	// Mobiles
	// -------

	Android,
	iOS,


	// --------
	// Web
	// --------

	Web,

	// --------
	// Desktops
	// --------

	MacOSX, // universal (has 32 bit and 64 bit)

	// distinctions between 32 or 64 bit need to be made to
	// determine which existing native plugins are used or not
	Windows32,
	Windows64,

	Linux32,
	Linux64,
	LinuxUniversal,


	// ------
	// Others
	// ------

	// currently not handled in any special way (probably needs to be):
	XBOX360,
	PS3,
	Wii,
	Flash
}


public static class Util
{
	// care should be taken when using ShouldGetBuildReportNow and ShouldSaveGottenBuildReportNow
	// as they are effectively global variables
	// unfortunately this is the only way I can ensure persistence of bool variables in between recompilations
	// SerializeField seems to fail at times

	public static BuildPlatform GetBuildPlatformBasedOnUnityBuildTarget(BuildTarget b)
	{
		switch (b)
		{
			case BuildTarget.WebPlayer:
				return BuildPlatform.Web;
			case BuildTarget.WebPlayerStreamed:
				return BuildPlatform.Web;
			case BuildTarget.NaCl:
				return BuildPlatform.Web;


			case BuildTarget.StandaloneOSXIntel:
				return BuildPlatform.MacOSX;

			case BuildTarget.StandaloneWindows:
				return BuildPlatform.Windows32;
			case BuildTarget.StandaloneWindows64:
				return BuildPlatform.Windows64;

			case BuildTarget.iPhone:
				return BuildPlatform.iOS;

			case BuildTarget.Android:
				return BuildPlatform.Android;


			case BuildTarget.XBOX360:
				return BuildPlatform.XBOX360;

			case BuildTarget.PS3:
				return BuildPlatform.PS3;

			case BuildTarget.Wii:
				return BuildPlatform.Wii;


			case BuildTarget.FlashPlayer:
				return BuildPlatform.Flash;

#if (UNITY_4 || UNITY_4_1 || UNITY_4_2)
			case BuildTarget.StandaloneLinux:
				return BuildPlatform.Linux32;

			case BuildTarget.StandaloneLinux64:
				return BuildPlatform.Linux64;

			case BuildTarget.StandaloneLinuxUniversal:
				return BuildPlatform.LinuxUniversal;

			case BuildTarget.StandaloneOSXUniversal:
				return BuildPlatform.MacOSX;
#endif
		}

		return BuildPlatform.None;
	}

	public static bool ShouldGetBuildReportNow
	{
		get
		{
			return EditorPrefs.GetBool("BRT_ShouldGetBuildReportNow", false);
		}
		set
		{
			EditorPrefs.SetBool("BRT_ShouldGetBuildReportNow", value);
		}
	}
	public static bool ShouldSaveGottenBuildReportNow
	{
		get
		{
			return EditorPrefs.GetBool("BRT_ShouldSaveGottenBuildReportNow", false);
		}
		set
		{
			EditorPrefs.SetBool("BRT_ShouldSaveGottenBuildReportNow", value);
		}
	}


	public static bool IsAScriptDLL(string filename)
	{
		return filename.StartsWith("Assembly-");
	}

	public static string RemovePrefix(string prefix, string val)
	{
		if (val.StartsWith(prefix))
		{
			return val.Substring(prefix.Length, val.Length - prefix.Length);
		}
		return val;
	}

	public static string RemoveSuffix(string suffix, string val, int offset = 0)
	{
		if (val.EndsWith(suffix))
		{
			return val.Substring(0, val.Length - suffix.Length + offset);
		}
		return val;
	}



	static string GetLastFolder(string inFolder)
	{
		inFolder = inFolder.Replace('\\', '/');

		//Debug.Log("folder: " + inFolder);
		//string folderName = Path.GetDirectoryName(folderEntries[n]);

		int lastSlashIdx = inFolder.LastIndexOf('/');
		if (lastSlashIdx == -1)
		{
			return "";
		}
		return inFolder.Substring(lastSlashIdx+1, inFolder.Length-lastSlashIdx-1);
	}

	public static string FindAssetFolder(string folderToStart, string desiredFolderName)
	{
		string[] folderEntries = Directory.GetDirectories(folderToStart);

		for (int n = 0, len = folderEntries.Length; n < len; ++n)
		{
			string folderName = GetLastFolder(folderEntries[n]);
			//Debug.Log("folderName: " + folderName);

			if (folderName == desiredFolderName)
			{
				return folderEntries[n];
			}
			else
			{
				string recursed = FindAssetFolder(folderEntries[n], desiredFolderName);
				string recursedFolderName = GetLastFolder(recursed);
				if (recursedFolderName == desiredFolderName)
				{
					return recursed;
				}
			}
		}
		return "";
	}




	public static int GetFileSizeInBytes(string filename)
	{
		FileInfo fi = new FileInfo(filename);
		return (int)fi.Length;
	}

	public static string GetFileSizeReadable(string filename)
	{
		return GetFileSizeReadableFromBytes(GetFileSizeInBytes(filename));
	}

	const double ONE_TERABYTE = 1099511627776.0;
	const double ONE_GIGABYTE = 1073741824.0;
	const double ONE_MEGABYTE = 1048576.0;
	const double ONE_KILOBYTE = 1024.0;

	public static string GetFileSizeReadableFromBytes(double bytes)
	{
		//return EditorUtility.FormatBytes(bytes);
		return MyFileSizeReadable(bytes);
	}

	static string MyFileSizeReadable(double bytes)
	{

		double converted = bytes;
		string units = "B";

		if (bytes >= ONE_TERABYTE)
		{
			converted = bytes / ONE_TERABYTE;
			units = "TB";
		}
		else if (bytes >= ONE_GIGABYTE)
		{
			converted = bytes / ONE_GIGABYTE;
			units = "GB";
		}
		else if (bytes >= ONE_MEGABYTE)
		{
			converted = bytes / ONE_MEGABYTE;
			units = "MB";
		}
		else if (bytes >= ONE_KILOBYTE)
		{
			converted = bytes / ONE_KILOBYTE;
			units = "KB";
		}

		return String.Format("{0:0.##} {1}", converted, units);
	}

	public static double GetApproxSizeFromString(string size)
	{
		if (size == "N/A")
		{
			return 0;
		}

		string units = size.Substring(size.Length - 2, 2);
		string numOnly = size.Substring(0, size.Length - 2);

		double num = double.Parse(numOnly);

		//Debug.Log(size + " " + num);

		switch (units)
		{
			case "KB":
				return num * ONE_KILOBYTE;
			case "MB":
				return num * ONE_MEGABYTE;
			case "GB":
				return num * ONE_GIGABYTE;
			case "TB":
				return num * ONE_TERABYTE;
		}
		return 0;
	}


	static bool TextFileHasContents(string filepath, string contents)
	{
		FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		StreamReader sr = new StreamReader(fs);

		bool ret = false;

		string line;
		while ((line = sr.ReadLine()) != null)
		{
			if (line.IndexOf(contents) != -1)
			{
				ret = true;
				break;
			}
		}

		fs.Close();
		return ret;
	}

	public static bool FileHasContents(string filepath, string contents)
	{
		return TextFileHasContents(filepath, contents);
	}



	public static string GetTextFileContents(string file)
	{
		// thanks to http://answers.unity3d.com/questions/167518/reading-editorlog-in-the-editor.html
		FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		StreamReader sr = new StreamReader(fs);

		string contents = "";
		if (fs != null && sr != null)
		{
			contents = sr.ReadToEnd();
		}

		fs.Close();
		return contents;
	}

	public static void DeleteSizePartFile(SizePart file)
	{
		//Debug.Log("going to delete " + file.Name);
		DeleteFile(file.Name);
	}

	static void DeleteFile(string file)
	{
		string projectFolder = RemoveSuffix("Assets", Application.dataPath);

		DeleteFile(projectFolder, file);
	}

	public static bool HaveToUseSystemForDelete(string file)
	{
		return IsFileAUnixHiddenFile(file) || IsFileInVersionControlMetadataFolder(file);
	}

	static void DeleteFile(string projectFolder, string file)
	{
		//Debug.Log("will delete " + file);

		if (HaveToUseSystemForDelete(file))
		{
			string fileAbsPath = Path.Combine(projectFolder, file);
			//Debug.Log("will system delete " + fileAbsPath);
			SystemDeleteFile(fileAbsPath);
		}
		else
		{
			// AssetDatabase.MoveAssetToTrash also deletes .meta file if it exists
			AssetDatabase.MoveAssetToTrash(file);
		}
	}

	static void SystemDeleteFile(string file)
	{
		File.Delete(file);
	}


	public static bool IsFileInAPath(string filepath, string pathToCheck)
	{
		return filepath.ToLower().IndexOf(pathToCheck) != -1;
	}

	public static bool IsFileOfType(string filepath, string typeExtenstion)
	{
		return filepath.EndsWith(typeExtenstion);
	}

	public static bool IsFileAUnixHiddenFile(string filepath)
	{
		return Path.GetFileName(filepath).StartsWith(".");
	}

	public static bool IsFileInBuildReportFolder(string filepath)
	{
		return IsFileInAPath(filepath, "/buildreport/");
	}

	public static bool IsFileInEditorFolder(string filepath)
	{
		return IsFileInAPath(filepath, "/editor/");
	}

	public static bool IsFileInVersionControlMetadataFolder(string filepath)
	{
		return IsFileInAPath(filepath, "/.svn/") ||
			IsFileInAPath(filepath, "/.git/") ||
			IsFileInAPath(filepath, "/.cvs/");
	}


	public static string GetPackageFileContents(string filename)
	{
		// try default path first
		string defaultBuildReportToolFullPath = Application.dataPath + "/" + BuildReportTool.Options.BUILD_REPORT_TOOL_DEFAULT_FOLDER_NAME;

		string filePath = defaultBuildReportToolFullPath + "/" + filename;

		if (File.Exists(filePath))
		{
			return GetTextFileContents(filePath);
		}

		// not in default path
		// search for it

#if BRT_SHOW_MINOR_WARNINGS
		Debug.LogWarning(BuildReportTool.Options.BUILD_REPORT_PACKAGE_MOVED_MSG);
#endif

		string folderPath = BuildReportTool.Util.FindAssetFolder(Application.dataPath, BuildReportTool.Options.BUILD_REPORT_TOOL_DEFAULT_FOLDER_NAME);

		if (!string.IsNullOrEmpty(folderPath))
		{
			filePath = folderPath + "/" + filename;

			if (File.Exists(filePath))
			{
				return GetTextFileContents(filePath);
			}
		}

		// could not find it
		// giving up
		Debug.LogError(BuildReportTool.Options.BUILD_REPORT_PACKAGE_MISSING_MSG);

		return "";
	}




	// thanks to http://answers.unity3d.com/questions/16804/retrieving-project-name.html
	public static string GetProjectName(string projectAssetsFolderPath)
	{
		var dp = projectAssetsFolderPath;
		string[] s = dp.Split("/"[0]);
		return s[s.Length - 2];
	}





	// we have two ways of getting user home folder here:

	// from http://stackoverflow.com/questions/1143706/getting-the-path-of-the-home-directory-in-c
	public static string UserHomePath
	{
		get
		{
			string homePath = (System.Environment.OSVersion.Platform == PlatformID.Unix ||
				System.Environment.OSVersion.Platform == PlatformID.MacOSX)
				? System.Environment.GetEnvironmentVariable("HOME")
				: System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

			return homePath;
		}
	}

	//[MenuItem("Window/Test 3")]
	public static string GetUserHomeFolder()
	{
		string ret;
		ret = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal).ToString();
		//Debug.Log("GetUserHomeFolder: " + ret);
		ret = ret.Replace("\\", "/");
		return ret;
	}




	//[MenuItem("Window/Test 4")]
	public static void OpenInFileBrowserTest()
	{
		//string path = "/Users/Ferds/Unity Projects/BuildReportTool/BuildReportUnityProject/Assets/BuildReportDebug";
		//string path = "/Users/Ferds/Unity Projects/BuildReportTool/BuildReportUnityProject/Assets/BuildReportDebug/EditorMorel.log.txt";
		//string path = "/Users/Ferds/UnityBuildReports/";
		string path = "/Users/Ferds/UnityBuildReports/test4.xml";

		OpenInFileBrowser(path);
	}


	public static void OpenInMacFileBrowser(string path)
	{
		bool openInsidesOfFolder = false;

		// try mac
		string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

		if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}

		//Debug.Log("macPath: " + macPath);
		//Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

		if (!macPath.StartsWith("\""))
		{
			macPath = "\"" + macPath;
		}
		if (!macPath.EndsWith("\""))
		{
			macPath = macPath + "\"";
		}
		string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
		//Debug.Log("arguments: " + arguments);
		try
		{
			System.Diagnostics.Process.Start("open", arguments);
		}
		catch(System.ComponentModel.Win32Exception e)
		{
			// tried to open mac finder in windows
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenInWinFileBrowser(string path)
	{
		bool openInsidesOfFolder = false;

		// try windows
		string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

		if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}
		try
		{
			System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
		}
		catch(System.ComponentModel.Win32Exception e)
		{
			// tried to open win explorer in mac
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenInFileBrowser(string path)
	{
		if (IsInWinOS)
		{
			OpenInWinFileBrowser(path);
		}
		else if (IsInMacOS)
		{
			OpenInMacFileBrowser(path);
		}
		else // couldn't determine OS
		{
			OpenInWinFileBrowser(path);
			OpenInMacFileBrowser(path);
		}
	}

	public static bool IsInMacOS
	{
		get
		{
			return SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
		}
	}

	public static bool IsInWinOS
	{
		get
		{
			return SystemInfo.operatingSystem.IndexOf("Windows") != -1;
		}
	}

	static string GetEditorLogFileInWindows(string editorFilename = "Editor.log")
	{
		string editorLogSubPath = "/Unity/Editor/" + editorFilename;

		// try getting from LOCALAPPDATA
		// this is the one used from after Windows XP

		string localAppDataVar = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");

		if (!string.IsNullOrEmpty(localAppDataVar))
		{
			string nonXpStyleAppDataPath = localAppDataVar.Replace("\\", "/");
			if (Directory.Exists(nonXpStyleAppDataPath))
			{
				return nonXpStyleAppDataPath + editorLogSubPath;
			}
		}

		// didn't find it in LOCALAPPDATA
		// try USERPROFILE (WinXP style)

		string userProfileVar = System.Environment.GetEnvironmentVariable("USERPROFILE");

		if (!string.IsNullOrEmpty(userProfileVar))
		{
			string xpStyleAppDataPath = userProfileVar.Replace("\\", "/") + "/Local Settings/Application Data";
			if (Directory.Exists(xpStyleAppDataPath))
			{
				return xpStyleAppDataPath + editorLogSubPath;
			}
		}

		Debug.LogError("Could not find path to Unity Editor log!");

		return "";
	}

	public static string EditorLogDefaultPath
	{
		get
		{
			if (System.Environment.OSVersion.Platform == PlatformID.Unix ||
				System.Environment.OSVersion.Platform == PlatformID.MacOSX)
			{
				return UserHomePath + "/Library/Logs/Unity/Editor.log";
			}

			return GetEditorLogFileInWindows();
		}
	}

	public static string EditorPrevLogPath
	{
		get
		{
			if (System.Environment.OSVersion.Platform == PlatformID.Unix ||
				System.Environment.OSVersion.Platform == PlatformID.MacOSX)
			{
				return UserHomePath + "/Library/Logs/Unity/Editor-prev.log";
			}

			return GetEditorLogFileInWindows("Editor-prev.log");
		}
	}

	public static string UsedEditorLogPath
	{
		get
		{
			if (IsDefaultEditorLogPathOverriden)
			{
				return BuildReportTool.Options.EditorLogOverridePath;
			}
			return EditorLogDefaultPath;
		}
	}

	public static string EditorLogPathOverrideMessage
	{
		get
		{
			if (IsDefaultEditorLogPathOverriden)
			{
				return "(Overriden)";
			}
			return "(Default)";
		}
	}

	public static bool IsDefaultEditorLogPathOverriden
	{
		get
		{
			return !string.IsNullOrEmpty(BuildReportTool.Options.EditorLogOverridePath);
		}
	}

	public static bool UsedEditorLogExists
	{
		get
		{
			return File.Exists(UsedEditorLogPath);
		}
	}




	public static string GetBuildManagedFolder(string buildFilePath)
	{
		string buildFolder = buildFilePath;

		const string WINDOWS_APP_FILE_TYPE = ".exe";
		const string MAC_APP_FILE_TYPE = ".app";

		if (buildFolder.EndsWith(WINDOWS_APP_FILE_TYPE)) // Windows
		{
			//
			// example:
			// "/Users/Ferds/Unity Projects/BuildReportTool/testwin64.exe"
			//
			// need to remove ".exe" at end
			// then append "_Data" at end
			//
			buildFolder = buildFolder.Substring(0, buildFolder.Length - WINDOWS_APP_FILE_TYPE.Length);
			buildFolder += "_Data/Managed";
		}
		else if (buildFolder.EndsWith(MAC_APP_FILE_TYPE)) // Mac OS X
		{
			//
			// example:
			// "/Users/Ferds/Unity Projects/BuildReportTool/testmac.app"
			//
			// .app is really just a folder.
			//
			buildFolder += "/Contents/Data/Managed";
		}
		else if (Directory.Exists(buildFolder + "/Data/Managed/")) // iOS
		{
			buildFolder += "/Data/Managed";
		}
		else if (!Directory.Exists(buildFolder))
		{
			// happens with users who use custom builders
			//Debug.LogWarning("Folder \"" + buildFolder + "\" does not exist.");
			return "";
		}

		buildFolder += "/";

		return buildFolder;
	}

	static string GetProjectTempStagingArea(string projectDataPath)
	{
		string tempFolder = projectDataPath;
		const string assets = "Assets";
		tempFolder = tempFolder.Substring(0, tempFolder.Length - assets.Length);
		tempFolder += "Temp/StagingArea";
		return tempFolder;
	}

	public static bool AttemptGetWebTempStagingArea(string projectDataPath, out string path)
	{
		string tempFolder = GetProjectTempStagingArea(projectDataPath) + "/Data/Managed/";

		if (Directory.Exists(tempFolder))
		{
			path = tempFolder;
			return true;
		}
		path = "";
		return false;
	}

	public static bool AttemptGetAndroidTempStagingArea(string projectDataPath, out string path)
	{
		string tempFolder = GetProjectTempStagingArea(projectDataPath) + "/assets/bin/Data/Managed/";

		//Debug.Log(tempFolder);

		if (Directory.Exists(tempFolder))
		{
			path = tempFolder;
			return true;
		}
		path = "";
		return false;
	}

	public static bool AttemptGetUnityFolderMonoDLLs(bool wasWebBuild, bool wasAndroidApkBuild, string editorAppContentsPath, ApiCompatibilityLevel monoLevel, StrippingLevel codeStrippingLevel, out string path, out string higherPriorityPath)
	{
		bool success = false;

		// more hackery
		// attempt to get DLL size info
		// from Unity install folder
		//
		// this only happens in:
		//  1. Web build
		//  2. Android build
		//  3. Custom builders
		//
		string[] pathTries = new string[]
		{
			editorAppContentsPath + "/Frameworks/Mono/lib/mono",
			editorAppContentsPath + "/Mono/lib/mono",
			"/Applications/Unity/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"C:/Program Files (x86)/Unity/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity/Editor/Data/Mono/lib/mono",
#if UNITY_3_5
			"/Applications/Unity3/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"/Applications/Unity 3/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"/Applications/Unity3.5/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"/Applications/Unity 3.5/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"C:/Program Files (x86)/Unity3/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity 3/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity3.5/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity 3.5/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity3/Editor/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity 3/Editor/Data/Mono/lib/mono",
#endif
#if (UNITY_4 || UNITY_4_1)
			"/Applications/Unity4/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"/Applications/Unity 4/Unity.app/Contents/Frameworks/Mono/lib/mono",
			"C:/Program Files (x86)/Unity4/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity 4/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity4/Editor/Data/Mono/lib/mono",
			"C:/Program Files (x86)/Unity 4/Editor/Data/Mono/lib/mono",
#endif
		};

		string tryPath = "";

		for (int n = 0, len = pathTries.Length; n < len; ++n)
		{
			tryPath = pathTries[n];
			if (Directory.Exists(tryPath))
			{
				break;
			}
			tryPath = "";
		}

		if (!string.IsNullOrEmpty(tryPath))
		{
			success = true;

			// "unity_web" is obviously for the web build. Presumably, this one has DLLs removed that can compromise web security.
			// "2.0" is likely the full featured Mono libraries
			// "unity" is most likely the one used when selecting 2.0 subset in the player settings. this is the setting by default.
			// "micro" is probably the one used in StrippingLevel.UseMicroMSCorlib. only makes sense to be here when building on Android.
			//   since in iOS, we already have the DLL files. No need for this hackery in iOS. But since in Android we do not have a project folder,
			//   we resort to this.

			if (wasWebBuild)
			{
				path = tryPath + "/unity_web/";
			}
			else if (monoLevel == ApiCompatibilityLevel.NET_2_0_Subset)
			{
				path = tryPath + "/unity/";
			}
			else
			{
				path = tryPath + "/2.0/";
			}

			if (wasAndroidApkBuild && codeStrippingLevel == StrippingLevel.UseMicroMSCorlib)
			{
				higherPriorityPath = tryPath + "/micro/";
			}
			else
			{
				higherPriorityPath = "";
			}
		}
		else
		{
			path = "";
			higherPriorityPath = "";
		}

		return success;
	}




	public static string[] GetAllScenesUsedInProject()
	{
		return EditorBuildSettings.scenes.Where(s => s.enabled).Select(n => n.path).ToArray();
	}


	public static BuildReportTool.SizePart CreateSizePartFromFile(string filename, string fileFullPath)
	{
		BuildReportTool.SizePart outPart = new BuildReportTool.SizePart();

		outPart.Name = System.Security.SecurityElement.Escape(filename);

		if (File.Exists(fileFullPath))
		{
			int fileSizeBytes = GetFileSizeInBytes(fileFullPath);
			outPart.SizeBytes = fileSizeBytes;
			outPart.Size = GetFileSizeReadableFromBytes(fileSizeBytes);

		}
		else
		{
			outPart.SizeBytes = -1;
			outPart.Size = "???";
		}

		/// \todo perhaps compute percentage: file size of this DLL out of total build size (would need to convert string of total build size into an int of bytes)
		outPart.Percentage = -1;

		return outPart;
	}



	static void SaveTextFile(string saveFilePath, string data)
	{
		string folder = System.IO.Path.GetDirectoryName(saveFilePath);
		System.IO.Directory.CreateDirectory(folder);

#if UNITY_WEBPLAYER && !UNITY_EDITOR
		Debug.LogError("Current build target is set to Web Player. Cannot perform file input/output when in Web Player.");
#else
		System.IO.StreamWriter write = new System.IO.StreamWriter(saveFilePath, false, System.Text.Encoding.UTF8); // Unity's TextAsset.text borks when encoding used is UTF8 :(
		write.Write(data);
		write.Flush();
		write.Close();
		write.Dispose();
#endif
	}

	static string FixXmlBuildReportFile(string serializedBuildInfoFilePath)
	{
		string xmlData = GetTextFileContents(serializedBuildInfoFilePath);

		if (string.IsNullOrEmpty(xmlData))
		{
			return "";
		}

		xmlData = xmlData.Replace("BuildSizePart", "SizePart");

		// quick and dirty fix for invalid XML characters in filenames
		xmlData = xmlData.Replace("&#x1;", "");
		xmlData = xmlData.Replace("&#x2;", "");
		xmlData = xmlData.Replace("&#x3;", "");
		xmlData = xmlData.Replace("&#x4;", "");
		xmlData = xmlData.Replace("&#x5;", "");
		xmlData = xmlData.Replace("&#x6;", "");
		xmlData = xmlData.Replace("&#x7;", "");
		xmlData = xmlData.Replace("&#x8;", "");
		xmlData = xmlData.Replace("&#xb;", "");
		xmlData = xmlData.Replace("&#xc;", "");
		xmlData = xmlData.Replace("&#xe;", "");
		xmlData = xmlData.Replace("&#xf;", "");
		xmlData = xmlData.Replace("&#x10;", "");
		xmlData = xmlData.Replace("&#x11;", "");
		xmlData = xmlData.Replace("&#x12;", "");
		xmlData = xmlData.Replace("&#x13;", "");
		xmlData = xmlData.Replace("&#x14;", "");
		xmlData = xmlData.Replace("&#x15;", "");
		xmlData = xmlData.Replace("&#x16;", "");
		xmlData = xmlData.Replace("&#x17;", "");
		xmlData = xmlData.Replace("&#x18;", "");
		xmlData = xmlData.Replace("&#x19;", "");
		xmlData = xmlData.Replace("&#x1a;", "");
		xmlData = xmlData.Replace("&#x1b;", "");
		xmlData = xmlData.Replace("&#x1c;", "");
		xmlData = xmlData.Replace("&#x1d;", "");
		xmlData = xmlData.Replace("&#x1e;", "");
		xmlData = xmlData.Replace("&#x1f;", "");
		xmlData = xmlData.Replace("&#x7f;", "");
		xmlData = xmlData.Replace("&#x81;", "");

		SaveTextFile(serializedBuildInfoFilePath, xmlData);

		return xmlData;
	}

	public static string MyHtmlDecode(string input)
	{
		input = input.Replace("&lt;", "<");
		input = input.Replace("&gt;", ">");
		input = input.Replace("&amp;", "&");
		input = input.Replace("&apos;", "'");
		input = input.Replace("&quot;", "\"");
		input = input.Replace("&ntilde;", "ñ");
		input = input.Replace("&Ntilde;", "Ñ");
		input = input.Replace("&copy;", "©");
		input = input.Replace("&reg;", "®");
		input = input.Replace("&#8482;", "™");
		input = input.Replace("&euro;", "€");

		return input;
	}



	public static BuildInfo OpenSerializedBuildInfo(string serializedBuildInfoFilePath, bool fromMainThread = true)
	{
		BuildInfo ret = null;

		XmlSerializer x = new XmlSerializer( typeof(BuildInfo) );

		string correctedXmlData = FixXmlBuildReportFile(serializedBuildInfoFilePath);

		try
		{
			// when the string has contents, it means there were corrections to the xml data
			// and we should load that updated content instead of reading the file
			if (!string.IsNullOrEmpty(correctedXmlData))
			{
				TextReader reader = new StringReader(correctedXmlData);
				ret = (BuildInfo)x.Deserialize(reader);
			}
			else
			{
				// no corrections in the xml file
				// proceed to open the file normally
				using(FileStream fs = new FileStream(serializedBuildInfoFilePath, FileMode.Open))
				{
					XmlReader reader = new XmlTextReader(fs);
					ret = (BuildInfo)x.Deserialize(reader);
					fs.Close();
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}

		if (fromMainThread)
		{
			if (BuildInfoHasContents(ret))
			{
				ret.OnDeserialize();
				ret.SetSavedPath(serializedBuildInfoFilePath);
			}
			else
			{
				Debug.LogError("Build Report Tool: Invalid data in build info file: " + serializedBuildInfoFilePath);
			}
		}

		return ret;
	}

	public static void SerializeBuildInfoAtFolder(BuildInfo buildInfo, string pathToSave)
	{
		string filePath = pathToSave;
		if (!string.IsNullOrEmpty(pathToSave))
		{
			if (!Directory.Exists(pathToSave))
			{
				Directory.CreateDirectory(pathToSave);
			}
			filePath += "/";
		}
		//Debug.Log("built folder");
		filePath += buildInfo.ProjectName + "-" + buildInfo.BuildType + buildInfo.TimeGot.ToString("-yyyyMMMdd-HHmmss") + ".xml";
		//Debug.Log("built filepath " + filePath);

		SerializeBuildInfo(buildInfo, filePath);
	}

	public static void SerializeBuildInfo(BuildInfo buildInfo, string serializedBuildInfoFilePath)
	{
		XmlSerializer x = new XmlSerializer( typeof(BuildInfo) );
		TextWriter writer = new StreamWriter(serializedBuildInfoFilePath);
		x.Serialize(writer, buildInfo);
		writer.Close();

		buildInfo.SetSavedPath(serializedBuildInfoFilePath);

		Debug.Log("Build Report Tool: Saved build info at \"" + buildInfo.SavedPath + "\"");
	}

	public static bool BuildInfoHasContents(BuildInfo n)
	{
		return n != null && n.HasContents;
	}
}

} // namespace BuildReportTool
