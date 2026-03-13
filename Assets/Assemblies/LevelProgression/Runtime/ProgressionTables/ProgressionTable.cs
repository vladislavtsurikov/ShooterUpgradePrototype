using System;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.LevelProgression
{
    [Serializable]
    public abstract class ProgressionTable
    {
        public abstract string DisplayName { get; }
        public virtual string Description => string.Empty;
        public virtual bool CanEditValuesDirectly => false;

        public virtual void Initialize(List<float> values)
        {
            EnsureValues(values);
        }

        public virtual void RebuildValues(List<float> values)
        {
            EnsureValues(values);
        }

        public virtual void SetValuesCount(List<float> values, int count)
        {
            ResizeValues(values, count);
        }

        public virtual void SetValue(List<float> values, int level, float value)
        {
            EnsureValues(values);
            values[Mathf.Clamp(level, 0, values.Count - 1)] = value;
        }

        protected static void EnsureValues(List<float> values)
        {
            if (values.Count == 0)
            {
                values.Add(0f);
            }
        }

        protected static void ResizeValues(List<float> values, int count)
        {
            int targetCount = Mathf.Max(1, count);
            while (values.Count < targetCount)
            {
                float nextValue = values.Count == 0 ? 0f : values[values.Count - 1];
                values.Add(nextValue);
            }

            while (values.Count > targetCount)
            {
                values.RemoveAt(values.Count - 1);
            }
        }
    }
}
