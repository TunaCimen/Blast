using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class Pooler<T> where T:MonoBehaviour
    {
        private Stack<T>[] pool;
        public Pooler(int length)
        {
            pool = new Stack<T>[length];
            for (var i = 0; i < length; i++)
            {
                pool[i] = new Stack<T>();
            }

        }
        
        public T CullObject(int typeIndex, Vector3 pos,Func<Vector3,T> generator)
        {
            T t;
            if (pool[typeIndex].TryPop(out t))
            {
                t.transform.position = pos;
                return t;
            }
            t = generator(pos);
            return t;
        }
        
        public void ReturnObject(T obj, int typeIndex,Action<T> remover)
        {
            remover(obj);
            pool[typeIndex].Push(obj);
        }
    }
}