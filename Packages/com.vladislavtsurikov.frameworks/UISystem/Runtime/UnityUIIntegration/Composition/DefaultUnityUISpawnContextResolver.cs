namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public sealed class DefaultUnityUISpawnContextResolver : IUnityUISpawnContextResolver
    {
        public static DefaultUnityUISpawnContextResolver Instance { get; } = new();

        private DefaultUnityUISpawnContextResolver()
        {
        }

        public (UnityEngine.Transform parentTransform, string parentName) Resolve(UnityUIHandler handler) =>
            (handler.ResolveSpawnParentTransform(), handler.ResolveSpawnParentName());
    }
}
