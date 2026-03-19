using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelProgression.Runtime.ProgressionTables
{
    [Serializable]
    public abstract class ProgressionTable
    {
        public abstract string DisplayName { get; }
        public virtual string Description => string.Empty;
        public virtual bool CanEditValuesDirectly => false;

        public abstract IReadOnlyList<float> BuildValues();

        public virtual void SetValues(IReadOnlyList<float> values)
        {
        }

        public virtual void SetValuesCount(int count)
        {
        }

        public virtual void SetValue(int level, float value)
        {
        }

        protected static List<float> EnsureValues(IReadOnlyList<float> values)
        {
            if (values == null || values.Count == 0)
            {
                return new List<float> { 0f };
            }

            return values.ToList();
        }

        protected static List<float> ResizeValues(IReadOnlyList<float> values, int count)
        {
            List<float> result = EnsureValues(values);
            int targetCount = Mathf.Max(1, count);
            while (result.Count < targetCount)
            {
                float nextValue = result.Count == 0 ? 0f : result[result.Count - 1];
                result.Add(nextValue);
            }

            while (result.Count > targetCount)
            {
                result.RemoveAt(result.Count - 1);
            }

            return result;
        }
    }
}
