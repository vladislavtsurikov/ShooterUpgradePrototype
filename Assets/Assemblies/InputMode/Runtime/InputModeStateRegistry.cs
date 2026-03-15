using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace AutoStrike.Input.Services.States
{
    public sealed class InputModeStateRegistry : IDisposable
    {
        private static readonly Type KeyboardMouseStateType = typeof(KeyboardMouseInputModeState);

        public InputModeStateRegistry()
        {
            All = ReflectionFactory.CreateAllInstances<InputModeState>().ToList();
        }

        public IReadOnlyList<InputModeState> All { get; }

        public InputModeState ResolveByType(Type stateType)
        {
            if (stateType == null)
            {
                return null;
            }

            foreach (InputModeState state in All)
            {
                if (state.GetType() == stateType)
                {
                    return state;
                }
            }

            return null;
        }

        public InputModeState GetHighestPriorityState(InputDevice lastUsedDevice)
        {
            InputModeState highestPriorityState = null;
            int highestPriority = int.MinValue;

            foreach (InputModeState state in All)
            {
                int priority = state.GetPriority(lastUsedDevice);
                state.SetActive(priority >= 0);

                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    highestPriorityState = state;
                }
            }

            if (highestPriorityState != null)
            {
                return highestPriorityState;
            }

            InputModeState fallbackState = ResolveByType(KeyboardMouseStateType);
            if (fallbackState == null && All.Count > 0)
            {
                fallbackState = All[0];
            }

            fallbackState?.SetActive(true);
            return fallbackState;
        }

        public void Dispose()
        {
            foreach (InputModeState state in All)
            {
                state.Dispose();
            }
        }
    }
}
