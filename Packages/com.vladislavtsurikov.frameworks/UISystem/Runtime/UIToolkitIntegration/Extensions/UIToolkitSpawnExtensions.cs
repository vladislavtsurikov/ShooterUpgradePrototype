#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public static class UIToolkitSpawnExtensions
    {
        public static UIToolkitSpawnOperation Spawn(this ChildSpawningUIToolkitHandler handler) => new();
    }
}
#endif
