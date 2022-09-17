using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.PrefabsActions
{
    public class PrefabCreator
    {
        private readonly DiContainer _container;

        private PrefabCreator(DiContainer container)
        {
            _container = container;
        }
        
        public GameObject Create(Object prefab)
        {
            return _container.InstantiatePrefab(prefab);
        }
        
        public GameObject Create(Object prefab, Transform parent)
        {
            return _container.InstantiatePrefab(prefab, parent);
        }

        public T Create<T>(Object prefab)
        {
            return _container.InstantiatePrefabForComponent<T>(prefab);
        }

        public T Create<T>(Object prefab, Transform parent)
        {
            return _container.InstantiatePrefabForComponent<T>(prefab, parent);
        }

        public T Create<T>(Object prefab, Transform parent, IEnumerable<object> args)
        {
            return _container.InstantiatePrefabForComponent<T>(prefab, parent, args);
        }
    }
}