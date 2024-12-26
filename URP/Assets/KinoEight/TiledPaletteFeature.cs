using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Kino.PostProcessing.Eight {

sealed class TiledPalettePass : ScriptableRenderPass
{
    public override void RecordRenderGraph(RenderGraph graph,
                                           ContextContainer context)
    {
        // Controller component reference retrieval
        var camera = context.Get<UniversalCameraData>().camera;
        var ctrl = camera.GetComponent<TiledPaletteController>();
        if (ctrl == null || !ctrl.enabled) return;

        // Unsupported case: Back buffer source
        var resource = context.Get<UniversalResourceData>();
        if (resource.isActiveTargetBackBuffer) return;

        // Destination texture allocation
        var source = resource.activeColorTexture;
        var desc = graph.GetTextureDesc(source);
        desc.name = "TiledPalette";
        desc.enableRandomWrite = true;
        desc.clearBuffer = false;
        desc.depthBufferBits = 0;
        var dest = graph.CreateTexture(desc);

        // Compute pass registration
        using (var builder = graph.AddComputePass("TiledPalette", out object _))
        {
            builder.UseTexture(source);
            builder.UseTexture(dest, AccessFlags.Write);
            builder.SetRenderFunc((object _, ComputeGraphContext ctx)
                                    => ctrl.ExecutePass(ctx, source, dest, desc));
        }

        // Destination texture as the camera texture
        resource.cameraColor = dest;
    }
}

public sealed class TiledPaletteFeature : ScriptableRendererFeature
{
    TiledPalettePass _pass;

    public override void Create()
      => _pass = new TiledPalettePass
           { renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData data)
      => renderer.EnqueuePass(_pass);
}

} // namespace Kino.PostProcessing.Eight
