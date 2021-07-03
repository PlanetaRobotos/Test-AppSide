using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Pool : MonoBehaviour
    {
        private static Pool _instance;

        private readonly Dictionary<Type, Queue<object>> _pools = new Dictionary<Type, Queue<object>>();

        private void Awake()
        {
            _instance = this;
        }

        public static void AddToPool<T>(T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(false);
            _instance._pools[typeof(T)].Enqueue(obj);
        }

        public static T GetFromPool<T>() where T : MonoBehaviour
        {
            T obj = (T) _instance._pools[typeof(T)].Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
    }
}