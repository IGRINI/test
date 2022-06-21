using System.Linq;

namespace Game.Entities.Modifiers
{
    
    public abstract class Modifier
    {
        public enum Type
        {
            SpeedMultiplier,
            Speed,
            JumpForce
        }

        public virtual Type[] Functions { get; private set; }

        public virtual float GetSpeedMultiplier()
        {
            return 0;
        }
        
        public virtual float GetSpeed()
        {
            return 0;
        }
        
        public virtual float GetJumpForce()
        {
            return 0;
        }
    }
}