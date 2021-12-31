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
            // Coroutine delayCoroutine = Delay.ForSeconds(60, () => Debug.Log("Delay coroutine"));
            // Coroutine repeatCoroutine = Repeat.EverySeconds(60, () => Debug.Log("Repeat coroutine"));
            Coroutine staticCoroutine = StaticCoroutine.Start(SomeCoroutine());
            // StaticCoroutine.Stop(delayCoroutine);
            // StaticCoroutine.Stop(repeatCoroutine);
            StaticCoroutine.Stop(staticCoroutine);
            
            // You can specify the MonoBehaviour instance on which to execute the coroutine.
            ExampleBehaviour exampleBehaviour = GetComponent<ExampleBehaviour>();
            Coroutine delayCoroutine = Delay.ForSeconds(60, () => Debug.Log("Delay coroutine"), exampleBehaviour);
            Coroutine repeatCoroutine = Repeat.EverySeconds(60, () => Debug.Log("Repeat coroutine"), this);
            // In this case, you can stop the coroutine as usual.
            // See the description of the StaticCoroutine.Stop() method for details.
            exampleBehaviour.StopCoroutine(delayCoroutine);
            StopCoroutine(repeatCoroutine);
        }

        public void EnumerableExtensions()
        {
            GameObject[] objects =
            {
                new GameObject("Cube"),
                new GameObject("Sphere"),
                new GameObject("Cone")
            };

            // Shuffle the array.
            objects = objects.Shuffle().ToArray();
            // Output the array to the console.
            // Output: Cone (UnityEngine.GameObject), Sphere (UnityEngine.GameObject), Cube (UnityEngine.GameObject)
            Debug.Log(objects.AsString());
            // Output the array to the console by specifying the string selector and separator.
            // Output: Cone : Sphere : Cube
            Debug.Log(objects.AsString(item => item.name, " : "));
            // Get random object from array.
            GameObject randomObj = objects.Random();
        }

        private IEnumerator SomeCoroutine()
        {
            yield return null;
        }
    }
}