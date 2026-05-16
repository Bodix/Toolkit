using System.Collections;
using Bodix.Evolunity.Components;

namespace _WIP.Test
{
    public class FrameSkipAnimation : AnimationBehaviour
    {
        public int FrameCount = 3;

        protected override IEnumerator AnimationCoroutine()
        {
            for (int i = 0; i < FrameCount; i++)
                yield return null;
        }
    }
}