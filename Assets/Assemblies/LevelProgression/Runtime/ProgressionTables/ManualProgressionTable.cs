using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ActionFlow.Runtime.LevelProgression
{
    [Serializable]
    public sealed class ManualProgressionTable : ProgressionTable
    {
        public override string DisplayName => "Manual Progression";
        public override string Description => "Each level value is authored by hand.";
        public override bool CanEditValuesDirectly => true;

        public override void Initialize(List<float> values)
        {
            EnsureValues(values);
        }
    }
}
