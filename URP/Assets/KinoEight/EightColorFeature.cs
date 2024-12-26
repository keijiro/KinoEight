using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Kino.PostProcessing.Eight {

sealed class EightColorPass : ScriptableRenderPass
{
    class PassData { public EightColorController Controller { get; set; } }

    public override void RecordRenderGraph(RenderGraph graph,
                                           ContextContainer context)
    {
        // EightColorController component reference
        var camera = context.Get<UniversalCameraData>().camera;
        var ctrl = camera.GetComponent<EightColorController>();
        if (ctrl == null || !ctrl.enabled) return;

        // Not supported: Back buffer source
        var resource = context.Get<UniversalResourceData>();
        if (resource.isActiveTargetBackBuffer) return;

        // Destination texture allocation
        var source = resource.activeColorTexture;
        var desc = graph.GetTextureDesc(source);
        desc.name = "EightColor";
        desc.clearBuffer = false;
        desc.depthBufferBits = 0;
        var dest = graph.CreateTexture(desc);

        // Blit
        var param = new RenderGraphUtils.
          BlitMaterialParameters(source, dest, ctrl.Material, 0);
        graph.AddBlitPass(param, passName: "EightColor");

        // Destination texture as the camera texture
        resource.cameraColor = dest;
    }
}

public sealed class EightColorFeature : ScriptableRendererFeature
{
    EightColorPass _pass;

    public override void Create()
      => _pass = new EightColorPass
           { renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData data)
      => renderer.EnqueuePass(_pass);
}

} // namespace Kino.PostProcessing.Eight
