using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{

    public class CustomizedRenderPipeline : RenderPipeline
    {
        internal ScriptableRenderContext Context;
        public CustomizedRenderPipelineAsset Asset;
        public CustomizedRenderPipeline(CustomizedRenderPipelineAsset asset)
        {
            this.Asset = asset;
        }
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            this.Context = context;

        }

        internal void CommitCommandBuffer(CommandBuffer cmd, bool clear = true)
        {
            Context.ExecuteCommandBuffer(cmd);
            if (clear) cmd.Clear();
        }

    }

}