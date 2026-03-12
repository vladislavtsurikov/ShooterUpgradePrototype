namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats
{
    public readonly struct RuntimeStatBuildContext
    {
        public RuntimeStatBuildContext(string statId)
        {
            StatId = statId;
        }

        public string StatId { get; }
    }
}
