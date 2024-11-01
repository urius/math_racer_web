namespace Events
{
    public struct RequestLoadSceneEvent
    {
        public readonly string SceneName;

        public RequestLoadSceneEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}