using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Resettables.Components
{
    // [RequireComponent(typeof(TrailRenderer))]
    public class TrailRendererResetter : ComponentResetter<TrailRenderer>
    {
        protected override List<IResettable> GetResettables()
        {
            return new List<IResettable>
            {
                new ResettableMember<ShadowCastingMode>(
                    Component.shadowCastingMode, x => Component.shadowCastingMode = x),
                new ResettableMember<bool>(
                    Component.receiveShadows, x => Component.receiveShadows = x),
                new ResettableMember<bool>(
                    Component.allowOcclusionWhenDynamic, x => Component.allowOcclusionWhenDynamic = x),
                new ResettableMember<MotionVectorGenerationMode>(
                    Component.motionVectorGenerationMode, x => Component.motionVectorGenerationMode = x),
                new ResettableMember<int>(
                    Component.rendererPriority, x => Component.rendererPriority = x),
                new ResettableMember<Color>(Component.startColor, x => Component.startColor = x),
            };
        }
    }
}