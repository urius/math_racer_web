#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    [InitializeOnLoadAttribute]
    public static class DefaultSceneLoader
    {
        static DefaultSceneLoader()
        {
            var myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/BootstrapScene.unity");
            EditorSceneManager.playModeStartScene = myWantedStartScene;
            
            EditorApplication.playModeStateChanged -= LoadDefaultScene;
            EditorApplication.playModeStateChanged += LoadDefaultScene;
        }

        static void LoadDefaultScene(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                //EditorSceneManager.LoadScene(0);
            }
        }
    }
}
#endif