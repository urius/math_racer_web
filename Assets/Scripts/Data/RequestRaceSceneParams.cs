namespace Data
{
    public class RequestRaceSceneParams : RequestSceneParamsBase
    {
        public readonly bool IsMultiplayer;

        public RequestRaceSceneParams(bool isMultiplayer)
        {
            IsMultiplayer = isMultiplayer;
        }
    }
}