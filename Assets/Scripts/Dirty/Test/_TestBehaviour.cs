using System;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dirty.Test
{
    public class _TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private _Test test;

        private void Awake()
        {
            Test();
        }

        [Button]
        public void Test()
        {
            Thread thread = new Thread(Debug.Log);
            thread.IsBackground = true;
            thread.Start();
            
            ThreadPool.QueueUserWorkItem(x =>
            {
                int random = Random.Range(5, 10);
            });
        }

        public void Log()
        {
            Debug.Log("Log");
        }
    }
}