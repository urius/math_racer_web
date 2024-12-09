namespace Events
{
    public struct RequestNextSceneEvent
    {
        public readonly string SceneName;

        public RequestNextSceneEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}