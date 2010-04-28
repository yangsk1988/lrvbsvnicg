﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        class MenuScene : StaticModelObject
        {

            const float RotSpeed = 3;
            Vector3 RotAxis = new Vector3(2505.168f, 4325.199f, 4029.689f);
            float angle;
            
            public MenuScene(RenderSystem rs)
            {
                FileLocation fl = FileSystem.Instance.Locate("start.mesh", GameFileLocs.Model);

                ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                BoundingSphere.Radius = PlanetEarth.PlanetRadius;
                Transformation = Matrix.Identity;
                RotAxis.Normalize();

            }

            public override bool IsSerializable
            {
                get { return false; }
            }

            public override RenderOperation[] GetRenderOperation()
            {
                Matrix rot = Matrix.RotationAxis(RotAxis, angle);
                RenderOperation[] ops = base.GetRenderOperation();

                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Transformation *= rot;
                    }
                }
                return ops;
            }

            public override void Update(GameTime dt)
            {
                base.Update(dt);

                angle -= MathEx.Degree2Radian(RotSpeed * dt.ElapsedGameTimeSeconds);
            }
        }

        class MenuCamera : Camera
        {
            public MenuCamera(float aspect)
                : base(aspect)
            {
                FieldOfView = 35;
                Position = new Vector3(-5260.516f, 6214.899f, -15371.574f);
                NearPlane = 100;
                FarPlane = 25000;
            }
            public override void UpdateProjection()
            {
                float fovy = MathEx.Degree2Radian(23.5f);
                NearPlaneHeight = (float)(Math.Tan(fovy * 0.5f)) * NearPlane * 2;
                NearPlaneWidth = NearPlaneHeight * AspectRatio;

                Frustum.proj = Matrix.PerspectiveRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);

            }
            public override void Update(GameTime time)
            {
                //UpdateProjection();

                //x->z y->x z->y 
                Vector3 target = new Vector3(-3151.209f, 6214.899f, 325.246f);

                base.Update(time);
                Frustum.view = Matrix.LookAtRH(Position, target, Vector3.UnitY);
                Frustum.Update();

                orientation = Quaternion.RotationMatrix(Frustum.view);

                Matrix m = Matrix.Invert(Frustum.view);
                front = m.Forward;// MathEx.GetMatrixFront(ref m);
                top = m.Up;// MathEx.GetMatrixUp(ref m);
                right = m.Right;// MathEx.GetMatrixRight(ref m);
            }
            public override float GetSMScale()
            {
                return 20;
            }
            public override Matrix GetSMTrans()
            {
                //Vector3 pos = new Vector3(-10799.082f, -3815.834f, 6951.33f);
                Vector3 pos = new Vector3(-5009.926f, 8066.071f, -16341.605f);//-6522.938f, 8066.071f, -12065.895f);// new Vector3(-3815.834f, 6951.33f, -10799.082f);
                Vector3 target = new Vector3(-2578.986f, 4845.344f, -2702.878f);// new Vector3(-3151.209f, 6214.899f, 325.246f);//-2702.878  -2578.986  4845.344


                return Matrix.LookAtRH(pos, target, Vector3.UnitY);
            }
        }

        SceneRenderer renderer;



        Code2015 game;
        MainMenu mainMenu;
        SelectScreen sideSelect;
        RenderTarget renderTarget;

        public UIComponent CurrentScreen
        {
            get;
            set;
        }
        public MainMenu GetMainMenu()
        {
            return mainMenu;
        }
        public SelectScreen GetSelectScreen()
        {
            return sideSelect;
        }

        public Texture Earth 
        {
            get { return renderTarget.GetColorBufferTexture(); }
        }

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this);
            this.sideSelect = new SelectScreen(game, this);

            this.CurrentScreen = mainMenu;
            CreateScene(rs);
           
        }

        void CreateScene(RenderSystem rs)
        {
            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctreeSceneManager(new OctreeBox(PlanetEarth.PlanetRadius * 4f), PlanetEarth.PlanetRadius / 75f);
            sm.PostRenderer = new BloomPostRenderer(rs);
            sm.UseShadow = true;

            MenuCamera camera = new MenuCamera(Program.ScreenWidth / (float)Program.ScreenHeight);

            renderTarget = rs.ObjectFactory.CreateRenderTarget(Program.ScreenWidth, Program.ScreenHeight, Apoc3D.Media.ImagePixelFormat.A8R8G8B8);

            camera.RenderTarget = renderTarget;
            renderer = new SceneRenderer(rs, sm);
            renderer.ClearColor = ColorValue.TransparentWhite;
            renderer.RegisterCamera(camera);


            renderer.ClearScreen = true;
            MenuScene obj = new MenuScene(rs);
            renderer.SceneManager.AddObjectToScene(obj);
        }

        public void Render()
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen == mainMenu)
                {
                    renderer.RenderScene();
                }
            }
        }


        public override void Render(Sprite sprite)
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    CurrentScreen.Render(sprite);
                }
            }
        }
        public override void Update(GameTime time)
        {
            if (!game.IsIngame)
            {
                renderer.Update(time);

                if (CurrentScreen != null)
                {
                    CurrentScreen.Update(time);

                    EffectParams.LightDir = -renderer.CurrentCamera.Front;
                }
            }
        }
    }

}
