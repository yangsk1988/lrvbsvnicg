﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class Picker : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;


        CityObject mouseHoverCity;
        
        Ray selectRay;

        public Ray SelectionRay
        {
            get { return selectRay; }
        }


        public ISelectableObject SelectedObject
        {
            get;
            private set;
        }
        public CityObject SelectedCity
        {
            get;
            private set;
        }
        public ISelectableObject MouseHoverObject
        {
            get;
            private set;

        }
        public CityObject MouseHoverCity
        {
            get { return mouseHoverCity; }
            private set
            {
                mouseHoverCity = value;
            }
        }

        public Picker(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.logic = gamelogic;

        }

        public override bool HitTest(int x, int y)
        {
            return true;
        }
        public override int Order
        {
            get { return 0; }
        }

        public override void Render(Sprite sprite)
        {

        }
        public override void Update(GameTime time)
        {

        }
        public override void UpdateInteract(GameTime time)
        {
            RtsCamera camera = parent.Scene.Camera;

            Vector3 mp = new Vector3(MouseInput.X, MouseInput.Y, 0);
            Vector3 start = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            mp.Z = 1;
            Vector3 end = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            Vector3 dir = end - start;
            dir.Normalize();

            selectRay = new Ray(start, dir);

            SceneObject obj = parent.Scene.Scene.FindObject(selectRay, SelFilter.Instance);
            MouseHoverObject = obj as ISelectableObject;
            MouseHoverCity = MouseHoverObject as CityObject;

            if (MouseInput.IsMouseDownLeft)
            {
                SelectedObject = MouseHoverObject;
                SelectedCity = MouseHoverCity;
            }
        }
    }
}
