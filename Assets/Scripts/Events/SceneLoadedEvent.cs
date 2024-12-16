namespace Events
{
    public struct SceneLoadedEvent
    {
        public readonly string SceneName;

        public SceneLoadedEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}