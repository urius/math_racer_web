namespace Events
{
    public struct StartLoadSceneEvent
    {
        public readonly string SceneName;

        public StartLoadSceneEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}