using System.Collections.Generic;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class RecursiveFieldsDrawer
    {
        private static readonly Dictionary<object, bool> s_foldoutStates = new();

        public bool IsExpanded(object value) => GetFoldoutState(value);

        protected bool GetFoldoutState(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!s_foldoutStates.TryGetValue(value, out var state))
            {
                s_foldoutStates[value] = false;
                return false;
            }

            return state;
        }

        protected void SetFoldoutState(object value, bool state)
        {
            if (value == null)
            {
                return;
            }

            s_foldoutStates[value] = state;
        }
    }
}
