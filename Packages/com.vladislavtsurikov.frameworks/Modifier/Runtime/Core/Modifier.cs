using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Modifier
{
    public abstract class Modifier : SerializedScriptableObject
    {
        [SerializeField]
        private string _id;

        [OdinSerialize]
        [HideInInspector]
        private ModifierComponentStack _componentStack = new();

        public string Id => _id;
        public ModifierComponentStack ComponentStack => _componentStack;

        private void OnEnable()
        {
            _componentStack ??= new ModifierComponentStack();
            _componentStack.Setup(true, new object[] { this });
        }

        private void OnDisable() => _componentStack?.OnDisable();
    }
}
