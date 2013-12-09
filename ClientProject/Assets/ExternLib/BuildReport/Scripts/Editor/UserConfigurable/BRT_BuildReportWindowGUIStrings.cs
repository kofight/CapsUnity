#if UNITY_EDITOR
using UnityEditor;

public partial class BRT_BuildReportWindow : EditorWindow
{
	// GUI messages, labels

	const string WAITING_FOR_BUILD_TO_COMPLETE_MSG = "Waiting for build to complete... Click this window if not in focus to refresh.";

	const string NO_BUILD_INFO_FOUND_MSG = "No Build Info.\n\nClick \"Get From Log\" to retrieve the last build info from the Editor log. Click Open to manually open a previously saved build report file.";



	const string TIME_OF_BUILD_LABEL = "Time of Build:";


	const string UNCOMPRESSED_TOTAL_SIZE_LABEL = "Uncompressed\nTotal Build Size:";
	const string COMPRESSED_TOTAL_SIZE_LABEL = "Compressed\nTotal Build Size:";
	const string TOTAL_SIZE_LABEL = "Total Build Size:";
	const string MONO_DLLS_LABEL = "Included DLLs:";
	const string SCRIPT_DLLS_LABEL = "Script DLLs:";

	const string OPEN_SERIALIZED_BUILD_INFO_TITLE = "Open Build Info XML File";

	const string TOTAL_SIZE_BREAKDOWN_LABEL = "Size Breakdown:";

	const string TOTAL_SIZE_BREAKDOWN_MSG_PRE_BOLD = "Based on";
	const string TOTAL_SIZE_BREAKDOWN_MSG_BOLD = "uncompressed";
	const string TOTAL_SIZE_BREAKDOWN_MSG_POST_BOLD = "build size";


	const string ASSET_SIZE_BREAKDOWN_LABEL = "Asset Breakdown:";

	const string ASSET_SIZE_BREAKDOWN_MSG_PRE_BOLD = "Sorted by";
	const string ASSET_SIZE_BREAKDOWN_MSG_BOLD = "uncompressed";
	const string ASSET_SIZE_BREAKDOWN_MSG_POST_BOLD = "size. Click on name to select/ping it in the Project window. Click on checkbox to include it in size calculations.";

	const string NO_FILES_FOR_THIS_CATEGORY = "None";

	const string NON_APPLICABLE_PERCENTAGE = "N/A";

	const string OVERVIEW_CATEGORY_LABEL = "Overview";
	const string USED_ASSETS_CATEGORY_LABEL = "Used Assets";
	const string UNUSED_ASSETS_CATEGORY_LABEL = "Unused Assets";
	const string OPTIONS_CATEGORY_LABEL = "Options";
	const string HELP_CATEGORY_LABEL = "Help & Info";

	const string REFRESH_LABEL = "Get From Log";
	const string OPEN_LABEL = "Open";
	const string SAVE_LABEL = "Save";

	const string SAVE_MSG = "Save Build Info to XML";

	const string SELECT_ALL_LABEL = "Select All";
	const string SELECT_NONE_LABEL = "Select None";
	const string SELECTED_QTY_LABEL = "Selected: ";
	const string SELECTED_SIZE_LABEL = "Total size: ";
	const string SELECTED_PERCENT_LABEL = "Total percentage: ";

	const string BUILD_TYPE_PREFIX_MSG = "For ";
	const string BUILD_TYPE_SUFFIX_MSG = "";
	const string UNITY_VERSION_PREFIX_MSG = ", built in ";

	const string COLLECT_BUILD_INFO_LABEL = "Collect and save build info automatically after building";
	const string SHOW_AFTER_BUILD_LABEL = "Show Build Report Window automatically after building";
	const string INCLUDE_SVN_LABEL = "Include SVN metadata in unused assets scan";
	const string INCLUDE_GIT_LABEL = "Include Git metadata in unused assets scan";
	const string FILE_FILTER_DISPLAY_TYPE_LABEL = "Draw file filters as:";

	const string FILE_FILTER_DISPLAY_TYPE_DROP_DOWN_LABEL = "Dropdown box";
	const string FILE_FILTER_DISPLAY_TYPE_BUTTONS_LABEL = "Buttons";

	const string SAVE_PATH_LABEL = "Current Build Report save path: ";
	const string SAVE_FOLDER_NAME_LABEL = "Folder name for Build Reports:";
	const string SAVE_PATH_TYPE_LABEL = "Save build reports:";

	const string SAVE_PATH_TYPE_PERSONAL_DEFAULT_LABEL = "In user's personal folder";
	const string SAVE_PATH_TYPE_PERSONAL_WIN_LABEL = "In \"My Documents\" folder";
	const string SAVE_PATH_TYPE_PERSONAL_MAC_LABEL = "In Home folder";
	const string SAVE_PATH_TYPE_PROJECT_LABEL = "Beside project folder";

	const string EDITOR_LOG_LABEL = "Unity Editor.log path ";
	const string EDITOR_LOG_INVALID_MSG = "Invalid path. Please change the path by clicking \"Set Override Log\"";

	const string SET_OVERRIDE_LOG_LABEL = "Set Override Log";
	const string CLEAR_OVERRIDE_LOG_LABEL = "Clear Override Log";

	const string FILTER_GROUP_TO_USE_LABEL = "File Filter Group To Use:";
	const string FILTER_GROUP_FILE_PATH_LABEL = "Configured File Filter Group: ";

	const string FILTER_GROUP_TO_USE_CONFIGURED_LABEL = "Always use configured file filter group";
	const string FILTER_GROUP_TO_USE_EMBEDDED_LABEL = "Use file filter group embedded in file if available";

	const string OPEN_IN_FILE_BROWSER_DEFAULT_LABEL = "Open in file browser";
	const string OPEN_IN_FILE_BROWSER_WIN_LABEL = "Show in Explorer";
	const string OPEN_IN_FILE_BROWSER_MAC_LABEL = "Reveal in Finder";



	const string CALCULATION_LEVEL_FULL_NAME = "Full Report";
	const string CALCULATION_LEVEL_FULL_DESC = "Calculate everything. Will show size breakdown, \"Used Assets\", and \"Unused Assets\" list. This can be slow if you have a large project with thousands of files or objects in scenes. If you get out of memory errors, try the other calculation levels.";

	const string CALCULATION_LEVEL_NO_PREFAB_NAME = "Do not calculate unused prefabs";
	const string CALCULATION_LEVEL_NO_PREFAB_DESC = "Will calculate everything, except that it will not determine whether a prefab is unused. It will still show which other assets are unused. If you have scenes that use hundreds to thousands of prefabs and you get an out of memory error when generating a build report, try this setting.";

	const string CALCULATION_LEVEL_NO_UNUSED_NAME = "Do not calculate unused assets";
	const string CALCULATION_LEVEL_NO_UNUSED_DESC = "Will display overview data and \"Used Assets\" list only. It will not determine which assets are unused.";

	const string CALCULATION_LEVEL_MINIMAL_NAME = "Overview only (minimum calculations)";
	const string CALCULATION_LEVEL_MINIMAL_DESC = "Will display overview data only. This is the fastest but also shows the least information.";
}

#endif
