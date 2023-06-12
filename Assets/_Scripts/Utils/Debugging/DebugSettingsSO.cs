namespace _Scripts.Utils.Debugging
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scriptable Objects/Debugging/Settings", fileName = "so_debug_settings")]
    public class DebugSettingsSO : ScriptableObject
    {
        [SerializeField] private bool _showLogs = true;

        public bool ShowLogs => _showLogs;
    }
}
