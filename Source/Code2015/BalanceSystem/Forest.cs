﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public enum PlantCategory
    {
        Grass,
        Bush,
        Forest,
    }
    public enum PlantType
    {
        TemperateZone,
        Subtropics,
        Tropics
    }

    public class Forest : NaturalResource
    {
        /// <summary>
        ///  
        /// </summary>
        [SLGValueAttribute()]
        const float AbsorbCarbonRate = 1000;

        [SLGValue]
        const float RecoverRate = 1;

        public float AbsorbCarbonSpeed
        {
            get;
            private set;
        }

        public PlantCategory Category
        {
            get;
            private set;
        }
        public PlantType Type
        {
            get;
            private set;
        }
        public float Radius
        {
            get;
            private set;
        }
        

        public Forest(SimulationRegion region)
            : base(region, NaturalResourceType.Wood)
        {
           
        }

      

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            Category = (PlantCategory)Enum.Parse(typeof(PlantCategory), sect.GetString("Category", ""));
            Type = (PlantType)Enum.Parse(typeof(PlantType), sect.GetString("Kind", ""));
            Radius = sect.GetSingle("Radius");
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;
        }

    }
}
