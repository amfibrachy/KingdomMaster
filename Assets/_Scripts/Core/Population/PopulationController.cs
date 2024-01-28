namespace _Scripts.Core
{
    using System;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using JobSystem;
    using UnityEngine;

    public class PopulationController : MonoBehaviour
    {
        // Injectables
        private SluggardsManager _sluggardsManager;
        private BuildersManager _buildersManager;
        
        [Inject]
        public void Construct(SluggardsManager sluggardsManager, BuildersManager buildersManager)
        {
            _sluggardsManager = sluggardsManager;
            _buildersManager = buildersManager;
        }
        
        public void OnDispatch<T>(FSM<T> npc, AgentType agent) where T : IFSM<T>
        {
            // TODO notify manager based on what FSM was dispatched
            switch (agent)
            {
                case AgentType.Sluggard:
                    _sluggardsManager.Dispatch(npc);
                    break;
                case AgentType.WithJob:
                    break;
                case AgentType.Player:
                    break;
                case AgentType.Enemy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agent), agent, null);
            }
            
            Destroy(npc.gameObject);
        }
    }
}
