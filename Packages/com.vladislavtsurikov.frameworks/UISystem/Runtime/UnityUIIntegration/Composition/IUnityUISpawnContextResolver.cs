using UnityEngine;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public interface IUnityUISpawnContextResolver
    {
        (Transform parentTransform, string parentName) Resolve(UnityUIHandler handler);
    }
}
