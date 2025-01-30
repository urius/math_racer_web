namespace Data
{
    public class PlayerSocialData
    {
        public string Name { get; private set; }
        public string SocialId { get; private set; }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetSocialId(string socialId)
        {
            SocialId = socialId;
        }
    }
}