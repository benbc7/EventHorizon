/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

public class HotkeyEditor : EditorWindow {

	private enum WindowType {
		None, CSWindow
	}

	private static WindowType windowType = WindowType.None;

	private static string className = "";
	private static string selectedPath = "";

	private static string[] defaultCS;

	private static HotkeyEditor window;

	private static bool textFocused;
	private static bool showNameError;

	[MenuItem ("Assets/Create/New C# Script %#c")]
	private static void NewCSMenuItem () {

		if (window != null) {
			window.Close ();
			window = null;
		}

		windowType = WindowType.CSWindow;
		string path;
		UnityEngine.Object obj = Selection.activeObject;
		if (obj == null) {
			path = "";
		} else {
			path = AssetDatabase.GetAssetPath (obj.GetInstanceID ());
		}

		if (path.Length > 0) {
			if (Directory.Exists (path)) {
				CSDialogeWindow (path);
			} else {
				CSDialogeWindow (Directory.GetParent (path).ToString ());
			}
		}
	}

	private static void CSDialogeWindow (string path) {
		selectedPath = path;
		window = CreateInstance<HotkeyEditor> ();
		window.position = new Rect (Screen.width / 2, Screen.height / 2, 250, 150);
		window.ShowPopup ();
		window.Focus ();
	}

	private static void CreatCSFile () {
		string newFilePath = selectedPath + "/" + className + ".cs";
		if (!File.Exists (newFilePath)) {
			using (StreamWriter outFile = new StreamWriter (newFilePath)) {
				foreach (string line in defaultCS) {
					if (line.Contains ("#SCRIPTNAME#")) {
						outFile.WriteLine (line.Replace ("#SCRIPTNAME#", className));
					} else if (line.Contains ("#NOTRIM#")) {
						outFile.WriteLine (line.Replace ("#NOTRIM#", ""));
					} else {
						outFile.WriteLine (line);
					}
				}
			}
			AssetDatabase.Refresh ();
		} else {
			Debug.LogError ("File already exists. Please enter a different name.");
		}
	}

	private void SCFileGUI (Event guiEvent) {
		GUILayout.Label ("Class Name");
		GUI.SetNextControlName ("Class Name");
		className = GUILayout.TextField (className);
		if (className.Length > 0) {
			className = char.ToUpper (className [0]) + className.Substring (1);
			className = className.Replace (" ", "_");
		}
		if (!textFocused) {
			EditorGUI.FocusTextInControl ("Class Name");
			textFocused = true;
		}
		GUILayout.Label (selectedPath + "/" + className + ".cs");
		if (GUILayout.Button ("Create Script (Return)") || guiEvent.isKey && guiEvent.keyCode == KeyCode.Return) {
			if (className.Length > 0) {
				className = char.ToUpper (className [0]) + className.Substring (1);
				CreatCSFile ();
				ResetSCWindow ();
			} else {
				showNameError = true;
				window.Repaint ();
			}
		}

		if (GUILayout.Button ("Cancel (ESC)") || guiEvent.isKey && guiEvent.keyCode == KeyCode.Escape) {
			ResetSCWindow ();
		}
		EditorGUILayout.HelpBox ((showNameError) ? "Class name cannot be blank" : "", (showNameError) ? MessageType.Warning : MessageType.None);
	}

	private void ResetSCWindow () {
		textFocused = false;
		showNameError = false;
		selectedPath = "";
		className = "";
		windowType = WindowType.None;
		Close ();
		window = null;
	}

	private void OnGUI () {
		Event guiEvent = Event.current;

		switch (windowType) {
			case WindowType.CSWindow: {
				SCFileGUI (guiEvent);
			}
			break;
		}
	}

	private void OnEnable () {
		string defaultCSPath = EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt";
		if (File.Exists (defaultCSPath)) {
			defaultCS = File.ReadAllLines (defaultCSPath);
		}
	}
}