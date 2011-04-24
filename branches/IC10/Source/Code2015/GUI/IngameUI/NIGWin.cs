﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI.IngameUI
{
    class NIGWin : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;


        Texture overlay;
        Texture background;
        Texture clearTex;


        float showPrg; 
        NIGDialogState state;


        Texture rankBackground;
        Texture homeBackground;
        Texture rankColor;

        GameFontRuan f8;
        GameFontRuan f6;


        Button backButton;
        Button replayButton;
        Button nextButton;
        


        public NIGWin(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
         
            FileLocation fl = FileSystem.Instance.Locate("nig_result_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
          
            fl = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            overlay = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_result_clear.tex", GameFileLocs.GUI);
            clearTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_result_back.tex", GameFileLocs.GUI);
            backButton = new Button();
            backButton.Image = UITextureManager.Instance.CreateInstance(fl);
            backButton.X = 461;
            backButton.Y = 542;
            backButton.Width = backButton.Image.Width;
            backButton.Height = backButton.Image.Height;
            backButton.Enabled = true;
            backButton.IsValid = true;
            backButton.MouseClick += BackButton_Click;

            fl = FileSystem.Instance.Locate("nig_result_replay.tex", GameFileLocs.GUI);
            replayButton = new Button();
            replayButton.Image = UITextureManager.Instance.CreateInstance(fl);
            replayButton.X = 588;
            replayButton.Y = 542;
            replayButton.Width = replayButton.Image.Width;
            replayButton.Height = replayButton.Image.Height;
            replayButton.Enabled = true;
            replayButton.IsValid = true;
            replayButton.MouseClick += ReplayButton_Click;

            fl = FileSystem.Instance.Locate("nig_result_next.tex", GameFileLocs.GUI);
            nextButton = new Button();
            nextButton.Image = UITextureManager.Instance.CreateInstance(fl);
            nextButton.X = 750;
            nextButton.Y = 542;
            nextButton.Width = nextButton.Image.Width;
            nextButton.Height = nextButton.Image.Height;
            nextButton.Enabled = true;
            nextButton.IsValid = true;
            nextButton.MouseClick += NextButton_Click;

            fl = FileSystem.Instance.Locate("nig_rank_bk.tex", GameFileLocs.GUI);
            rankBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_home.tex", GameFileLocs.GUI);
            homeBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_rank_color.tex", GameFileLocs.GUI);
            rankColor = UITextureManager.Instance.CreateInstance(fl);

            f6 = GameFontManager.Instance.FRuanEdged4;
            f8 = GameFontManager.Instance.FRuanEdged6;


            state = NIGDialogState.Hiding;

        }

        public void Show()
        {
            state = NIGDialogState.Showing;
        }
        public void Hide()
        {
            state = NIGDialogState.Hiding;
        }

        public override int Order
        {
            get
            {
                return 97;
            }
        }

        public override bool HitTest(int x, int y)
        {
            if (state != NIGDialogState.Hiding)
            {
                return true;
            }
            return false;
        }

        public override void Update(Apoc3D.GameTime time)
        {
            if (state == NIGDialogState.Hiding)
            {
                showPrg -= time.ElapsedGameTimeSeconds;
                if (showPrg < 0)
                {
                    showPrg = 0;
                }
            }
            else if (state == NIGDialogState.Showing)
            {
                showPrg += time.ElapsedGameTimeSeconds;
                if (showPrg > 1)
                {
                    showPrg = 1;
                }
            }
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            if (state == NIGDialogState.Showing)
            {
                backButton.UpdateInteract(time);
                replayButton.UpdateInteract(time);
                nextButton.UpdateInteract(time);
            }
        }

        void NextButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                Hide();
            }
        }

        void ReplayButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                Hide();
            }
        }


        void BackButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.Over();
                Hide();
            }
        }

        public override void Render(Sprite sprite)
        {
            if (state != NIGDialogState.Hiding)
            {
                ColorValue overlayColor = ColorValue.Transparent;
                overlayColor.A = (byte)(byte.MaxValue * showPrg);

                sprite.Draw(overlay, 0, 0, overlayColor);


            }
        }


        public void RenderRank(Sprite sprite)
        {

            List<Player> players = new List<Player>();
            for (int i = 0; i < gameLogic.LocalPlayerCount; i++)
                players.Add(gameLogic.GetLocalPlayer(i));

            StatisticRank(players);

            int startX = 500;
            int startY = 150;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Type != PlayerType.LocalHuman)
                {
                    string s = (i + 1).ToString();
                    sprite.Draw(rankBackground, startX - 6, startY, ColorValue.White);
                    f8.DrawString(sprite, s, startX, startY - 20, ColorValue.White);


                    Matrix trans = Matrix.Scaling(0.33f, 0.33f, 1) * Matrix.Translation(new Vector3(startX + 19, startY + 14, 0));
                    sprite.SetTransform(trans);

                    f8.DrawString(sprite, "TH", 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);

                    sprite.Draw(rankColor, startX + 35, startY + 9, players[i].SideColor);
                    f8.DrawString(sprite, "8".ToString(), startX + 80, startY - 20, ColorValue.White);
                    startY += 37;
                }
                else
                {
                    string s = (i + 1).ToString();
                    sprite.Draw(homeBackground, startX - 6, startY - 4, players[i].SideColor);
                    f8.DrawString(sprite, s, startX + 2, startY, ColorValue.White);

                    Matrix trans = Matrix.Scaling(0.35f, 0.35f, 1) * Matrix.Translation(new Vector3(startX + 22, startY + 35, 0));
                    sprite.SetTransform(trans);
                    f8.DrawString(sprite, "TH", 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);

                    f6.DrawString(sprite, "8".ToString(), startX + 73, startY - 45, ColorValue.White);
                    startY += 70;
                }
            }

        }


        private static int ComparePlayer(Player a, Player b)
        {
            return b.Area.CityCount.CompareTo(a.Area.CityCount);
        }

        private void StatisticRank(List<Player> players)
        {
            players.Sort(ComparePlayer);
        }


    }
}