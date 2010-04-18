﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.GUI;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.Logic
{
    class GoalPieceMaker : IUpdatable
    {
        RenderSystem renderSys;
        Camera camera;
        GoalIcons goalIcos;

        FastList<bool> passTable = new FastList<bool>();

        PlayerArea area;

        float timeCounter;

        public GoalPieceMaker(PlayerArea area, RenderSystem rs, Camera camera, GoalIcons icos)
        {
            this.area = area;
            this.camera = camera;
            this.renderSys = rs;
            this.goalIcos = icos;
        }

        #region IUpdatable 成员

        public void Update(GameTime dt)
        {
            while (passTable.Count < area.CityCount)
                passTable.Add(false);

            timeCounter += dt.ElapsedGameTimeSeconds;

            float timeLine = 9f / (float)Math.Sqrt(area.CityCount) + 3;
            if (timeCounter > timeLine)
            {
                int idx = Randomizer.GetRandomInt(area.CityCount) % area.CityCount;

                int tries = 0;
                while (passTable[idx] && tries < passTable.Count)
                {
                    idx++;
                    tries++;
                    idx %= passTable.Count;
                }
                if (passTable[idx])
                    Array.Clear(passTable.Elements, 0, passTable.Count);

                passTable[idx] = true;

                City cc = area.GetCity(idx);
                PieceCategoryProbability p = cc.GetProbability();
                
                MdgType pieceType;

                float rnd = Randomizer.GetRandomSingle();
                if (rnd < p.Health)
                {
                    int p2 = Randomizer.GetRandomInt(3);
                    switch (p2)
                    {
                        case 0:
                            pieceType = MdgType.Diseases;
                            break;
                        case 1:
                            pieceType = MdgType.MaternalHealth;
                            break;
                        default:
                            pieceType = MdgType.ChildMortality; 
                            break;
                    }
                }
                else if (rnd < p.Health + p.Environment)
                {
                    pieceType = MdgType.Environment;
                }
                else if (rnd < p.Health + p.Environment + p.Education)
                {
                    bool p2 = Randomizer.GetRandomBool();
                    pieceType = p2 ? MdgType.Education : MdgType.GenderEquality;
                }
                else
                {
                    pieceType = MdgType.Hunger;
                }


                CityObject cityObj = cc.Parent;
                Viewport vp = renderSys.Viewport;
                Vector3 ppos = vp.Project(cityObj.Position,
                    camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                Vector2 velocity = Vector2.Zero;
                if (scrnPos.X < vp.Width && scrnPos.Y < vp.Height)
                {
                    velocity = new Vector2(vp.Width * 0.5f - ppos.X, vp.Height * 0.5f - ppos.Y);
                    velocity.Normalize();
                }
                else
                {
                    velocity = new Vector2(ppos.X, ppos.Y);
                    velocity.Normalize();
                }

                MdgPiece piece = new MdgPiece(goalIcos.Manager, goalIcos.PhysicsWorld, pieceType, 0, new Vector2(ppos.X, ppos.Y), 0);
                
                piece.Velocity = velocity * 250;
                goalIcos.Manager.Add(piece);

                timeCounter = 0;
            }
        }

        #endregion
    }
}
