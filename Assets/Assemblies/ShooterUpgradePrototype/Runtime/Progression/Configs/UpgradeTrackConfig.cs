namespace ShooterUpgradePrototype.Progression.Configs
{
    public sealed class UpgradeTrackConfig
    {
        public UpgradeTrackConfig(string id, string displayName, int maxLevel)
        {
            Id = id;
            DisplayName = displayName;
            MaxLevel = maxLevel;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public int MaxLevel { get; }
    }
}
