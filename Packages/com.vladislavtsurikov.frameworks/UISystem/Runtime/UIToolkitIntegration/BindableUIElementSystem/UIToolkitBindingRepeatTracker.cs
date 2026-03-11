using System;
using System.Collections.Generic;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    internal sealed class UIToolkitBindingRepeatTracker
    {
        private readonly Dictionary<(Type, string), int> _counters = new();

        public int GetAndIncrement(Type type, string bindingId)
        {
            (Type, string) key = (type, bindingId ?? string.Empty);
            if (!_counters.TryGetValue(key, out int index))
            {
                _counters[key] = 1;
                return 0;
            }

            _counters[key] = index + 1;
            return index;
        }

        public void Reset() => _counters.Clear();
    }
}
