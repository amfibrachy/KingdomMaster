namespace _Scripts.Core
{
    using InputActions;

    public class InputManager
    {
        private static InputActions _input;
        
        private static InputActions Instance
        {
            get
            {
                if (_input == null)
                {
                    _input = new InputActions();
                    _input.Enable();
                    
                    _input.Player.Disable();
                    _input.UI.Disable();
                }
                
                return _input;
            }
        }

        public static InputActions.PlayerActions Player => Instance.Player;
        public static InputActions.UIActions UI => Instance.UI;

        private InputManager() { }
    }
}
