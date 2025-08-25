// Assets/Editor/MissingScriptCleaner.cs
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MissingScriptCleaner
{
    // ───────────────────────────────── 메뉴 ─────────────────────────────────
    [MenuItem("Tools/Missing Scripts/Clean Selected (Hierarchy)")]
    public static void CleanSelected() {
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0) {
            EditorUtility.DisplayDialog("Missing Scripts", "선택된 오브젝트가 없습니다.", "확인");
            return;
        }
        int total = 0;
        foreach (var go in selection) total += CleanGameObjectRecursive(go);
        EditorUtility.DisplayDialog("Missing Scripts", $"선택 항목에서 제거: {total}개", "확인");
    }

    [MenuItem("Tools/Missing Scripts/Clean Open Scenes")]
    public static void CleanOpenScenes() {
        int scenes = SceneManager.sceneCount;
        if (scenes == 0) {
            EditorUtility.DisplayDialog("Missing Scripts", "열려있는 씬이 없습니다.", "확인");
            return;
        }
        int removedTotal = 0;
        for (int i = 0; i < scenes; i++) {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            int removed = CleanScene(scene);
            removedTotal += removed;
            if (removed > 0) {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }
        EditorUtility.DisplayDialog("Missing Scripts", $"열린 씬에서 제거: {removedTotal}개", "확인");
    }

    [MenuItem("Tools/Missing Scripts/Clean All Scenes In Project")]
    public static void CleanAllScenesInProject() {
        var guids = AssetDatabase.FindAssets("t:Scene");
        int removedTotal = 0;
        try {
            for (int i = 0; i < guids.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Cleaning Scenes", path, (float)i / guids.Length);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                int removed = CleanScene(scene);
                removedTotal += removed;
                if (removed > 0) {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }
        EditorUtility.DisplayDialog("Missing Scripts", $"프로젝트 내 모든 씬에서 제거: {removedTotal}개", "확인");
    }

    [MenuItem("Tools/Missing Scripts/Clean All Prefabs In Project")]
    public static void CleanAllPrefabsInProject() {
        var guids = AssetDatabase.FindAssets("t:Prefab");
        int removedTotal = 0;
        try {
            for (int i = 0; i < guids.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Cleaning Prefabs", path, (float)i / guids.Length);
                var root = PrefabUtility.LoadPrefabContents(path);
                int removed = 0;
                foreach (var go in root.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject))
                    removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

                if (removed > 0) {
                    removedTotal += removed;
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
                PrefabUtility.UnloadPrefabContents(root);
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }
        EditorUtility.DisplayDialog("Missing Scripts", $"모든 프리팹에서 제거: {removedTotal}개", "확인");
    }

    [MenuItem("Tools/Missing Scripts/Clean All (Scenes + Prefabs)")]
    public static void CleanAll() {
        if (!EditorUtility.DisplayDialog("Missing Scripts",
            "모든 씬과 프리팹에서 미싱 스크립트를 제거합니다.\n작업 전 버전관리/백업을 권장합니다.", "진행", "취소"))
            return;

        CleanAllScenesInProject();
        CleanAllPrefabsInProject();
    }

    // ────────────────────────────── 내부 유틸리티 ─────────────────────────────
    private static int CleanScene(Scene scene) {
        int removed = 0;
        var roots = scene.GetRootGameObjects();
        foreach (var root in roots) removed += CleanGameObjectRecursive(root);
        return removed;
    }

    private static int CleanGameObjectRecursive(GameObject go) {
        int count = 0;
        // Undo 지원
        Undo.RegisterFullObjectHierarchyUndo(go, "Remove Missing Scripts");
        count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        foreach (Transform child in go.transform)
            count += CleanGameObjectRecursive(child.gameObject);
        return count;
    }
}
#endif
