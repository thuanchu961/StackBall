using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace CBGames
{
    [CustomEditor(typeof(AdmobController))]
    public class AdmobControllerCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!ScriptingSymbolsHandler.NamespaceExists(NamespaceData.GoogleMobileAdsNameSpace))
            {
                EditorGUILayout.HelpBox("Google Mobile Ads plugin is not imported. Please click the button bellow to download the plugin.", MessageType.Warning);
                if (GUILayout.Button("Download Plugin", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");
                }
            }
            else
            {
                string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> currentSymbols = new List<string>(symbolStr.Split(';'));
                if (!currentSymbols.Contains(ScriptingSymbolsData.ADMOB))
                {
                    List<string> sbs = new List<string>();
                    sbs.Add(ScriptingSymbolsData.ADMOB);
                    ScriptingSymbolsHandler.AddDefined_ScriptingSymbol(sbs.ToArray(), EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
            base.OnInspectorGUI();
        }
    }
}

