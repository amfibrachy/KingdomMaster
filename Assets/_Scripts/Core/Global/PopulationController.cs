namespace _Scripts.Core.Global
{
    using System;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using JobSystem;
    using NPC;
    using UnityEngine;

    public class PopulationController : MonoBehaviour
    {
        // Injectables
        private UIUpdateController _uiUpdateController;
        private SluggardsManager _sluggardsManager;
        private BuildersManager _buildersManager;
        private LumberjacksManager _lumberjacksManager;
        
        [Inject]
        public void Construct(
            UIUpdateController uiUpdateController, 
            SluggardsManager sluggardsManager, 
            BuildersManager buildersManager, 
            LumberjacksManager lumberjacksManager)
        {
            _uiUpdateController = uiUpdateController;
            _sluggardsManager = sluggardsManager;
            _buildersManager = buildersManager;
            _lumberjacksManager = lumberjacksManager;
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
                    if (npc is IHasJob job)
                    {
                        _uiUpdateController.UpdateDispatchUI(AgentType.WithJob, npc, job.Job);
                        
                        switch (job.Job)
                        {
                            case JobType.Builder:
                                _buildersManager.Dispatch(npc);
                                break;
                            
                            case JobType.Hauler:
                            case JobType.Lumberjack:
                                _lumberjacksManager.Dispatch(npc);
                                break;
                            
                            case JobType.Miner:
                            case JobType.Farmer:
                            case JobType.Blacksmith:
                            case JobType.Cook:
                            case JobType.Fisherman:
                            case JobType.Herbalist:
                            case JobType.Alchemist:
                            case JobType.Engineer:
                            case JobType.None:
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
            
            Destroy(npc.gameObject);
        }
        
        public void OnCreate(AgentType agent, Vector3 position, JobType job)
        {
            // TODO notify manager what type of entity needs to be created 
            switch (agent)
            {
                case AgentType.Sluggard:
                    break;
                
                case AgentType.WithJob:
                    _uiUpdateController.UpdateCreateUI(AgentType.WithJob, job);

                    switch (job)
                    {
                        case JobType.Builder:
                            _buildersManager.Create(position);
                            break;
                        
                        case JobType.Hauler:
                        case JobType.Lumberjack:
                            _lumberjacksManager.Create(position);
                            break;
                        
                        case JobType.Miner:
                        case JobType.Farmer:
                        case JobType.Blacksmith:
                        case JobType.Cook:
                        case JobType.Fisherman:
                        case JobType.Herbalist:
                        case JobType.Alchemist:
                        case JobType.Engineer:
                        case JobType.None:
                        default:
                            break;
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
