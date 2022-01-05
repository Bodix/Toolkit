using System.Collections;
using System.Linq;
using Evolutex.Evolunity.Extensions;
using Evolutex.Evolunity.Utilities;
using UnityEngine;

namespace _WIP
{
    public class Cheatsheet : MonoBehaviour
    {
        private void Start()
        {
            EnumerableExtensions();
        }

        public void Coroutines()
        {
            // Calls the function in the next frame.
            Delay.ForOneFrame(() => Debug.Log("Hello in the next frame"));
            // Calls the function after a N of seconds.
            Delay.ForSeconds(3, () => Debug.Log("Hello after three seconds"));
            // Calls the function after a N of frames.
            Delay.ForSeconds(300, () => Debug.Log("Hello after three hundred frames"));

            // Calls the function periodically every N seconds.
            Repeat.EverySeconds(1, () => Debug.Log("Hello every second"));
            // Calls the function periodically every N frames.
            Repeat.EveryFrames(10, () => Debug.Log("Hello every ten frames"));
            // Calls the function periodically every frame.
            // Analogous to "Update", but you can use it not only from MonoBehaviour classes.
            Repeat.EveryFrame(() => Debug.Log("Hello every frame"));

            // Starts a static coroutine. You can use this outside of MonoBehaviour.
            StaticCoroutine.Start(SomeCoroutine());

            // You can cache a coroutine instance and stop it at any time.
            Coroutine delayCoroutine = Delay.ForSeconds(60, () => Debug.Log("Delay coroutine"));
            Coroutine repeatCoroutine = Repeat.EverySeconds(60, () => Debug.Log("Repeat coroutine"));
            Coroutine staticCoroutine = StaticCoroutine.Start(SomeCoroutine());
            // To stop a cached coroutine instance use StaticCoroutine.Stop method.
            // See the description of the StaticCoroutine.Stop method for details.
            StaticCoroutine.Stop(delayCoroutine);
            StaticCoroutine.Stop(repeatCoroutine);
            StaticCoroutine.Stop(staticCoroutine);
            
            // You can specify the MonoBehaviour instance on which to execute the coroutine.
            ExampleBehaviour exampleBehaviour = GetComponent<ExampleBehaviour>();
            Coroutine delayCoroutine2 = Delay.ForSeconds(60, () => Debug.Log("Delay coroutine"), exampleBehaviour);
            Coroutine repeatCoroutine2 = Repeat.EverySeconds(60, () => Debug.Log("Repeat coroutine"), this);
            // In this case, you can stop the coroutine as usual.
            exampleBehaviour.StopCoroutine(delayCoroutine2);
            this.StopCoroutine(repeatCoroutine2);
        }

        public void EnumerableExtensions()
        {
            GameObject[] objects =
            {
                new GameObject("Cube"),
                new GameObject("Sphere"),
                new GameObject("Cone")
            };

            // Output the array to the console.
            // Output: Cone (UnityEngine.GameObject), Sphere (UnityEngine.GameObject), Cube (UnityEngine.GameObject)
            Debug.Log(objects.AsString());
            // Output the array to the console by specifying the string selector and separator.
            // Output: Cone : Sphere : Cube
            Debug.Log(objects.AsString(item => item.name, " : "));
            
            // Get random object from the array.
            GameObject randomObj = objects.Random();
            
            // Shuffle the array.
            objects = objects.Shuffle().ToArray();

            // Remove duplicates from the array.
            objects = objects.RemoveDuplicates().ToArray();

            // ForEach as extension method.
            objects.ForEach(Debug.Log);
            objects.ForEach((x, index) => Debug.Log(index + " : " + x.name + ", "));
            // ForEach with lazy execution.
            objects.ForEachLazy(Debug.Log);
            objects.ForEachLazy((x, index) => Debug.Log(index + " : " + x.name + ", "));
        }
        
        public class ExampleBehaviour : MonoBehaviour
        {
        
        }

        private IEnumerator SomeCoroutine()
        {
            yield return null;
        }
    }
}