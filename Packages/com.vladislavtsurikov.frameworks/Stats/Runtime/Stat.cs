using OdinSerializer;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [CreateAssetMenu(menuName = "ActionFlow/Stats/Stat", fileName = "Stat")]
    public sealed class Stat : SerializedScriptableObject
    {
        [SerializeField]
        private string _id;

        [OdinSerialize]
        [HideInInspector]
        private StatsComponentStack _componentStack = new();

        public string Id => _id;
        public StatsComponentStack ComponentStack => _componentStack;

        private void OnEnable()
        {
            _componentStack ??= new StatsComponentStack();
            _componentStack.Setup(true, new object[] { this });

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void OnDisable() => _componentStack?.OnDisable();
    }
}
