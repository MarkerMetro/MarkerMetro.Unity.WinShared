using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarkerMetro.Unity.WinShared.Editor
{
    /// <summary>
    /// Editor utitlty for finding prefabs and so on that reference a given asset
    /// </summary>
    public class FindAssetReferencesWindow : EditorWindow
    {
        [MenuItem("Tools/Marker Metro/Find Asset References...", priority = 40)]
        public static void FindAssetReferences()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length > 0)
            {
#if WITHOUT_GUI
                List<string> referencedFrom = new List<string>();

                referencedFrom.AddRange(
                    FindReferencesInAssets(guids[0],
                        Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories)));

                referencedFrom.AddRange(
                    FindReferencesInMaterials(guids[0],
                        Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories)));

                referencedFrom.AddRange(
                    FindReferencesInScenes(guids[0],
                        Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories)));

                string output = AssetDatabase.GUIDToAssetPath(guids[0]) + " referenced from: \n";

                foreach (string file in referencedFrom)
                {
                    output += file + "\n";
                }

                EditorUtility.DisplayDialog("Asset Reference Finder", output, "OK");

#else
                FindAssetReferencesWindow.CreateFindWindow(guids[0]);

#endif
            }
            else
            {
                EditorUtility.DisplayDialog("Asset Reference Finder", "No Asset Selected", "OK");
            }
        }

#if WITHOUT_GUI
        private static IEnumerator<string> FindReferencesInAssets(string guid, string[] assetFilePaths)
        {
            foreach (string assetFilePath in assetFilePaths)
            {
                string assetTextContent = File.ReadAllText(assetFilePath);
                if (assetTextContent.StartsWith("%YAML") && assetTextContent.Contains(guid))
                {
                    yield return assetFilePath;
                }
            }
        }
#else
        struct FindResult
        {
            public string AssetPath;
            public bool IsMatch;
        }

        private static IEnumerator<FindResult> FindReferencesInAssets(string guid, string[] assetFilePaths)
        {
            foreach (string assetFilePath in assetFilePaths)
            {
                FindResult result = new FindResult();
                result.AssetPath = assetFilePath;

                string assetTextContent = File.ReadAllText(assetFilePath);

                result.IsMatch = assetTextContent.StartsWith("%YAML") && assetTextContent.Contains(guid);

                yield return result;
            }
        }

        public static void CreateFindWindow(string guidToFind)
        {
            FindAssetReferencesWindow window = (FindAssetReferencesWindow)EditorWindow.GetWindow(typeof (FindAssetReferencesWindow));

            window.titleContent = new GUIContent("Find Refs");
            window.minSize = new Vector2(640.0f, 360.0f);

            window.Show();

            window.BeginSearch(guidToFind);
        }

        void BeginSearch(string guidToFind)
        {
            this.unityNeedsARest = 2;

            this.guidToFind = guidToFind;
            this.referencedFrom.Clear();
            this.stage = 0;
            this.BeginStage();
        }

        const int MaxRefsToDisplay = 100;

        string guidToFind;

        static readonly string[] assetTypes = { "scenes", "prefabs", "materials" };

        int stage;
        IEnumerator<FindResult> currentRoutine;
        uint totalItems;
        uint currentItem;

        List<string> referencedFrom = new List<string>();

        Vector2 scrollViewPosition;
        float scrollViewWidth;

        int unityNeedsARest;
        
        void OnGUI()
        {
            if (this.currentRoutine != null)
            {
                // If you show a progress bar immediately after creating a window, certain versions of Unity freeze
                if (this.unityNeedsARest > 0)
                {
                    this.unityNeedsARest -= 1;
                }
                else
                {
                    string currentFile = this.currentRoutine.Current.AssetPath;

                    // Debug.LogFormat("{0}/{1}: {2}", currentItem, totalItems, currentFile);

                    string message;
                    if (this.stage >= 0 && this.stage < assetTypes.Length)
                    {
                        message = string.Format("Looking in {0}... {1}", assetTypes[this.stage], currentFile);
                    }
                    else
                    {
                        message = "Finishing.";
                    }

                    float progress = 1;
                    if (this.totalItems > 0 && this.currentItem < this.totalItems)
                    {
                        progress = (float)this.currentItem / (float)this.totalItems;
                    }

                    if (EditorUtility.DisplayCancelableProgressBar("Finding References", message, progress))
                    {
                         this.stage = assetTypes.Length;
                    }
                }
            }

            string windowDescriptionMessage;
            if (this.currentRoutine != null) {
                windowDescriptionMessage = "Searching for references to {0} (looking for '{1}' in asset files).";
            } else {
                windowDescriptionMessage = "Found the following references to {0} (looked for '{1}' in asset files).";
            }

            GUILayout.Label(string.Format(windowDescriptionMessage, AssetDatabase.GUIDToAssetPath(this.guidToFind), this.guidToFind), EditorStyles.wordWrappedLabel);

            if (this.referencedFrom.Count > MaxRefsToDisplay)
            {
                EditorGUILayout.HelpBox(string.Format("Display limited to {0} referencing assets. Full list written to editor log.", MaxRefsToDisplay), MessageType.Warning);
            }

            this.scrollViewPosition = EditorGUILayout.BeginScrollView(this.scrollViewPosition);

            if (this.referencedFrom.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int refIndex = 0; refIndex < this.referencedFrom.Count && refIndex < MaxRefsToDisplay; ++refIndex)
                {
                    stringBuilder.AppendLine(this.referencedFrom[refIndex]);
                }
                string text = stringBuilder.ToString();
                float pixelHeight = EditorStyles.textArea.CalcHeight(new GUIContent(text), this.scrollViewWidth);
                EditorGUILayout.SelectableLabel(text, EditorStyles.textArea, GUILayout.MinHeight(pixelHeight), GUILayout.ExpandHeight(true));

                if (Event.current.type == EventType.Repaint)
                {
                    this.scrollViewWidth = GUILayoutUtility.GetLastRect().width;
                }

                // TODO: perhaps draw a button on each row that selects the asset in question (in the project view) when clicked?
                // for (int refIndex = 0; refIndex < this.referencedFrom.Count && refIndex < MaxRefsToDisplay; ++refIndex)
                // {
                //      EditorGUILayout.SelectableLabel(this.referencedFrom[refIndex], EditorStyles.label);
                // }
            }
            else
            {
                EditorGUILayout.HelpBox("No references found.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.HelpBox("Asset type must be set to Text in the project's Editor Settings for this to work.", MessageType.Info);

            if (this.stage >= assetTypes.Length)
            {
                this.currentRoutine = null;
                EditorUtility.ClearProgressBar();
            }

        }

        void OnInspectorUpdate()
        {
            try
            {
                if (this.currentRoutine != null) {
                    if (this.currentRoutine.MoveNext())
                    {
                        this.currentItem++;
                        if (this.currentRoutine.Current.IsMatch)
                        {
                            this.referencedFrom.Add(this.currentRoutine.Current.AssetPath);
                        }
                    }
                    else
                    {
                        this.stage++;
                        this.BeginStage();

                    }
                    this.Repaint();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        IEnumerator<FindResult> DoNothing()
        {
            yield break;
        }

        void BeginStage()
        {
            string[] assetFilePaths = null;
            switch (this.stage)
            {
                case 0:
                {
                    assetFilePaths = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
                } break;
                case 1:
                {
                    assetFilePaths = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
                } break;
                case 2:
                {
                    assetFilePaths = Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories);
                } break;
                case 3:
                //if (this.referencedFrom.Count > MaxRefsToDisplay)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(string.Format("References to {0} ({1}):", AssetDatabase.GUIDToAssetPath(this.guidToFind), this.guidToFind));
                    for (int refIndex = 0; refIndex < referencedFrom.Count; ++refIndex)
                    {
                        stringBuilder.AppendLine(referencedFrom[refIndex]);
                    }
                    Debug.Log(stringBuilder.ToString());
                } break;
                default:
                {
                    Debug.LogError("Invalid stage");
                } break;
            }

            if (assetFilePaths != null)
            {
                this.totalItems = (uint)assetFilePaths.Length;
                this.currentItem = 0;

                this.currentRoutine = FindReferencesInAssets(this.guidToFind, assetFilePaths);
            }
            else
            {
                this.currentItem = 0;
                this.totalItems = 0;
                this.currentRoutine = DoNothing();
            }
        }
#endif
    }
}