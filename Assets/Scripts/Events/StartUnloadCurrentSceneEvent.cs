namespace Events
{
    public struct StartUnloadCurrentSceneEvent
    {
        public readonly string CurrentSceneName;
        public readonly string NextSceneName;

        public StartUnloadCurrentSceneEvent(string currentSceneName, string nextSceneName)
        {
            CurrentSceneName = currentSceneName;
            NextSceneName = nextSceneName;
        }
    }
}