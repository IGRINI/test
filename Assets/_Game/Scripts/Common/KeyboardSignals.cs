using UnityEngine;

namespace Game.Common
{
    public static class KeyboardSignals
    {
        public class MovePerformed
        {
            public Vector2 Value;
        }
        
        public class JumpPerformed {}

        public class IsSprintPerformed
        {
            public bool IsPerformed;
        }
        public class InteractPerformed {}
        
        public class EscapePerformed {}
    }
}