using Data;

namespace Events
{
    public struct RequestNextSceneEvent
    {
        public readonly string SceneName;
        public readonly RequestSceneParamsBase RequestSceneParams;

        public RequestNextSceneEvent(string sceneName = null, RequestSceneParamsBase requestSceneParams = null)
        {
            SceneName = sceneName;
            RequestSceneParams = requestSceneParams;
        }
    }
}