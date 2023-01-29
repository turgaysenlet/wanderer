namespace Wanderer.Software.StateChart
{
    [Serializable()]
    public class StateChartCls<T> where T : class, IConvertible, new()
    {
        public Type StateEnumType
        {
            get
            {
                return typeof(T).GetType();
            }
        }
        private T currentState = new T();
        public T CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                int v = value.ToInt32(null);
                int c = currentState.ToInt32(null);

                if (c != v)
                {
                    previousState = currentState;
                    if (BeforeStateTransition != null)
                    {
                        BeforeStateTransition(this, value, previousState);
                    }
                    currentState = DoStateTransition(this, value);
                    if (AfterStateTransition != null)
                    {
                        AfterStateTransition(this, currentState, previousState);
                    }
                }
            }
        }

        private T previousState = new T();
        public T PreviousState
        {
            get
            {
                return previousState;
            }
        }
        /// <summary>
        /// Checks in current state is or in the given state.
        /// </summary>
        /// <param name="stateOrMacroState"></param>
        /// <returns>If the current state is the given state or a substate of the given state.</returns>
        public bool IsInState(T stateOrMacroState)
        {
            return IsInState(stateOrMacroState, currentState);
        }
        public static bool IsInState(T stateOrMacroState, T currentState)
        {
            bool result = false;
            if (currentState.ToInt32(null) == stateOrMacroState.ToInt32(null))
            {
                result = true;
            }
            else
            {
                int s = stateOrMacroState.ToInt32(null);
                int c = currentState.ToInt32(null);
                if ((s & 0x0FF) == 0x00)
                {
                    if ((c & s & 0xF00) != 0x00)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if ((s & 0x00F) == 0x0)
                {
                    if ((c & s & 0xF00) != 0x0 && (c & s & 0x0F0) != 0x0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        public event StateTransitionDlt BeforeStateTransition;
        public event StateTransitionDlt AfterStateTransition;
        public event DoStateTransitionDlt DoStateTransition;
        public delegate void StateTransitionDlt(StateChartCls<T> stateChart, T current, T previous);
        public delegate T DoStateTransitionDlt(StateChartCls<T> stateChart, T value);
    }
}