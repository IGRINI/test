using System.Collections.Generic;
using System.Linq;
using Game.Entities.Modifiers;
using UnityEngine;

namespace Game.Entities
{
    public abstract class BaseEntityModel : MonoBehaviour
    {
        private List<Modifier> _modifiers = new();

        protected IEnumerable<Modifier> Modifiers => _modifiers;

        protected void AddModifier(Modifier modifier)
        {
            _modifiers.Add(modifier);
        }

        protected void RemoveModifier<T>()
        {
            _modifiers.Remove(
                _modifiers.Find(modifier => modifier.GetType() == typeof(T))
            );
        }

        protected float GetSpeedMultiplier()
        {
            var speedModifiers =
                Modifiers.Where(modifier => modifier.Functions.Contains(Modifier.Type.SpeedMultiplier));
            
            if (!speedModifiers.Any()) return 1;
            
            var averageMultiplier = speedModifiers.Average(modifier => modifier.GetSpeedMultiplier());
        
            if (averageMultiplier > 0)
                return averageMultiplier;
            
            return 1;
        }
    }
}