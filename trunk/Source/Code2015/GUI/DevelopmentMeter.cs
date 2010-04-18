﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class DevelopmentMeter : UIComponent
    {
        struct Entry 
        {
            public Player Player;
            public ProgressBar Bar;
        }

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        FastList<Entry> prgBars = new FastList<Entry>();

        string GetColorBar(ColorValue color)
        {
            if (color == ColorValue.Red) 
            {
                return "ig_red.tex";
            }
            else if (color == ColorValue.Green) 
            {
                return "ig_green.tex";
            }
            else if (color == ColorValue.Yellow)
            {
                return "ig_yellow.tex";
            }
            return "ig_blueBar.tex";
        }

        public DevelopmentMeter(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("ig_scorebg.tex", GameFileLocs.GUI);
            Texture prgBarBg = UITextureManager.Instance.CreateInstance(fl);

            for (int i = 0; i < gamelogic.LocalPlayerCount; i++) 
            {
                Player pl = gamelogic.GetLocalPlayer(i);

                ProgressBar playerProgress = new ProgressBar();
                playerProgress.X = 995;
                playerProgress.Y = 32 + i * 57;
                playerProgress.Width = 277;
                playerProgress.Height = 41;
                playerProgress.Background = prgBarBg;
                fl = FileSystem.Instance.Locate(GetColorBar(pl.SideColor), GameFileLocs.GUI);
                playerProgress.ProgressImage = UITextureManager.Instance.CreateInstance(fl);

                Entry ent;
                ent.Player = pl;
                ent.Bar = playerProgress;
                prgBars.Add(ent);
            }

        }

        public override void Render(Sprite sprite)
        {
            for (int i = 0; i < prgBars.Count; i++)
            {
                prgBars.Elements[i].Bar.Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            for (int i = 0; i < prgBars.Count; i++)
            {
                prgBars.Elements[i].Bar.Value = prgBars.Elements[i].Player.Goal.DevelopmentPercentage;
            }
        }
    }
}
