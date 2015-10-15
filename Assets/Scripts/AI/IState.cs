using UnityEngine.EventSystems;

namespace Assets.Scripts.AI
{
    public interface IState
    {
        /// <summary>
        /// Used for executing the current action in this state.
        /// </summary>
        void ExecuteState();
    }
}
