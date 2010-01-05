﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

    public enum Grade { Bad, Medium, Fine };
   
    public class FarmLand : NaturalResource
    {
        [SLGValueAttribute()]
        const float INITFoodAmount = 100000;
        const float ABSORBCarbonSpeed = 1000;
        const float SOURCEProduceSpeed = 500;

        FastList<PlantSpecies> FoodPlants;
        public FarmLand(SimulateRegion region)
            : base(region)
        {
            FoodPlants = new FastList<PlantSpecies>();
            this.InitSourceAmount = INITFoodAmount;
            this.AbsorbCarbonSpeed = ABSORBCarbonSpeed;
            this.SourceProduceSpeed = SOURCEProduceSpeed;
        }

        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }     
        public Grade GradeOfSoil
        {
            get;
            private set;
        }
        

        public void GetConsumeSpeed()
        {
            float consumespeed = 0;
            for (int i = 0; i < this.CityCount; i++)
            {
                consumespeed += (this[i].GetPluginFoodCostSpeed() + this[i].FoodCostSpeed);
            }
            this.SourceConsumeSpeed = consumespeed;
        }
       

        //public bool Add(PlantSpecies foodplant)
        //{
        //    FoodPlants.Add(foodplant);
           
        //    return true;
        //}

        //public void Remove(PlantSpecies foodplant)
        //{
        //    FoodPlants.Remove(foodplant);
        //}

        
        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;       
            this.InitSourceAmount = INITFoodAmount;
            this.RemainingAmount += (SourceConsumeSpeed - SourceProduceSpeed) * hours;
            this.CarbonChange += -(this.InitSourceAmount * this.AbsorbCarbonSpeed*hours);
        }


    }
}
