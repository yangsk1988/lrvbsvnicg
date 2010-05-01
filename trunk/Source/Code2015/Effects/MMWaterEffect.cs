﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public class MMWaterEffectFactory : EffectFactory
    {
        static readonly string typeName = "MMWater";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem renderSystem;

        public MMWaterEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new MMWaterEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class MMWaterEffect : Effect
    {
        RenderSystem renderSystem;

        PixelShader pixShader;
        VertexShader vtxShader;
        float move;
        Texture reflection;

        public MMWaterEffect(RenderSystem renderSystem)
            : base(false, WaterEffectFactory.Name)
        {
            this.renderSystem = renderSystem;

            FileLocation fl = FileSystem.Instance.Locate("mmwater.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSystem, fl);

            fl = FileSystem.Instance.Locate("water.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSystem, fl);

            fl = FileSystem.Instance.Locate("reflection.tex", GameFileLocs.Nature);
            reflection = TextureManager.Instance.CreateInstance(fl);
        }

        protected override int begin()
        {
            renderSystem.BindShader(vtxShader);
            renderSystem.BindShader(pixShader);

            vtxShader.SetValue("lightDir", EffectParams.LightDir);

            ShaderSamplerState state = new ShaderSamplerState();
            state.AddressU = TextureAddressMode.Wrap;
            state.AddressV = TextureAddressMode.Wrap;
            state.AddressW = TextureAddressMode.Wrap;
            state.MinFilter = TextureFilter.Anisotropic;
            state.MagFilter = TextureFilter.Anisotropic;
            state.MipFilter = TextureFilter.Linear;
            state.MaxAnisotropy = 8;
            state.MipMapLODBias = 0;


            pixShader.SetSamplerState("dudvMap", ref state);
            pixShader.SetSamplerState("normalMap", ref state);

            pixShader.SetSamplerState("reflectionMap", ref state);

            pixShader.SetTexture("texShd", EffectParams.DepthMap[0]);

            state.AddressU = TextureAddressMode.Clamp;
            state.AddressV = TextureAddressMode.Clamp;
            state.AddressW = TextureAddressMode.Clamp;
            state.AddressU = TextureAddressMode.Border;
            state.AddressV = TextureAddressMode.Border;
            state.AddressW = TextureAddressMode.Border;
            state.MinFilter = TextureFilter.Linear;
            state.MagFilter = TextureFilter.Linear;
            state.MipFilter = TextureFilter.None;
            state.BorderColor = ColorValue.White;
            state.MaxAnisotropy = 0;
            state.MipMapLODBias = 0;

            pixShader.SetSamplerState("texShd", ref state);

            move += 0.0033f;
            while (move > 1)
                move--;

            return 1;
        }

        protected override void end()
        {
            renderSystem.BindShader((VertexShader)null);
            renderSystem.BindShader((PixelShader)null);
        }

        public override void BeginPass(int passId)
        {
        }

        public override void EndPass()
        {
        }

        public override bool SupportsMode(RenderMode mode)
        {
            switch (mode)
            {
                case RenderMode.Depth:
                    return false;
                case RenderMode.Final:
                case RenderMode.Simple:
                case RenderMode.Wireframe:
                    return true;
            }
            return false;
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

            Vector3 vpos = EffectParams.CurrentCamera.Position;
            vtxShader.SetValue("mvp", ref mvp);
            vtxShader.SetValue("world", ref op.Transformation);
            vtxShader.SetValue("viewPos", ref vpos);
            Matrix lightPrjTrans;
            Matrix.Multiply(ref op.Transformation, ref EffectParams.DepthViewProj, out lightPrjTrans);

            vtxShader.SetValue("smTrans", lightPrjTrans);

            pixShader.SetTexture("dudvMap", mat.GetTexture(0));
            pixShader.SetTexture("normalMap", mat.GetTexture(1));
            pixShader.SetTexture("reflectionMap", reflection);
            pixShader.SetValue("move", move);
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}