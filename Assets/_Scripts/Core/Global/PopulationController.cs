namespace _Scripts.Core.Global
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
        private UIUpdateController _uiUpdateController;
        
        [Inject]
        public void Construct(SluggardsManager sluggardsManager, BuildersManager buildersManager, UIUpdateController uiUpdateController)
        {
            _sluggardsManager = sluggardsManager;
            _buildersManager = buildersManager;
            _uiUpdateController = uiUpdateController;
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
        
        public void OnCreate(AgentType agent, Vector3 position, JobType? job)
        {
            // TODO notify manager what type of entity needs to be created 
            switch (agent)
            {
                case AgentType.WithJob:
                    if (job != null)
                    {
                        _uiUpdateController.UpdateUI(AgentType.WithJob, job.Value);

                        switch (job)
                        {
                            case JobType.Builder:
                                _buildersManager.Create(position);
                                break;
                            case JobType.Hauler:
                            case JobType.Lumberjack:
                            case JobType.Miner:
                            case JobType.Farmer:
                            case JobType.Blacksmith:
                            case JobType.Cook:
                            case JobType.Fisherman:
                            case JobType.Herbalist:
                            case JobType.Alchemist:
                            case JobType.Engineer:
                            default:
                                break;
                        }
                    }
                    break;
                case AgentType.Player:
                    break;
                case AgentType.Enemy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agent), agent, null);
            }
        }
    }
}
