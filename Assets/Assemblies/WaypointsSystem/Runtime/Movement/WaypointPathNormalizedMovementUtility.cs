using UnityEngine;

namespace WaypointsSystem.Runtime
{
    public static class WaypointPathNormalizedMovementUtility
    {
        public static float GetNextNormalized(float currentNormalized, float deltaNormalized, bool loop, int direction, out int nextDirection)
        {
            float safeDelta = Mathf.Max(0f, deltaNormalized);
            int safeDirection = direction >= 0 ? 1 : -1;

            if (loop)
            {
                nextDirection = safeDirection;
                return Mathf.Repeat(currentNormalized + safeDelta * safeDirection, 1f);
            }

            float rawNext = Mathf.Clamp01(currentNormalized) + safeDelta * safeDirection;
            while (rawNext > 1f || rawNext < 0f)
            {
                if (rawNext > 1f)
                {
                    rawNext = 2f - rawNext;
                    safeDirection = -1;
                    continue;
                }

                rawNext = -rawNext;
                safeDirection = 1;
            }

            nextDirection = safeDirection;
            return Mathf.Clamp01(rawNext);
        }
    }
}
