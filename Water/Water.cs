using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterProject
{
    public class Water
    {
        // Props
        public int Amount { get; private set; } 
        public double Temperature { get; private set; }
        public WaterState State { get; private set; }
        public double ProportionFirstState { get; private set; }
        public double ProportionSecondState { get; private set; }

        private const double CaloriesToMeltIcePerGram = 80;
        private const double CaloriesToEvaporatetWaterPerGram = 600;

        // constructors
        public Water(int amount, int temp)
        {
            if (temp == 0 || temp == 100)
            {
                throw new ArgumentException("When temperature is 0 or 100, you must provide a value for proportion");
            }
            Amount = amount;
            Temperature = temp;
            State = Temperature < 0 ? WaterState.Ice :
                Temperature > 100 ? WaterState.Gas :
                WaterState.Fluid;
        }

        public Water(int amount, int temp, double ratio ) 
        {
            Amount = amount;
            Temperature = temp;
            ProportionFirstState = ratio;
            ProportionSecondState = 1 - ratio;
            if (temp == 0)
            {
                State = WaterState.IceAndFluid;
            }

            if (temp == 100)
            {
                State = WaterState.FluidAndGas;
            }
        }

        public void AddEnergy(double energy)
        {
            // if below 0
            var energyPerGram = energy / (double)Amount;
            var temperature = Temperature + energyPerGram;
            if ( ((temperature < 0) || (Temperature > 0)) && (temperature < 100))
            {
                Temperature = temperature;
                return;
            }

            double caloriesForMeltingEverything = 0;
            if (Temperature < 0)
            {
                // if between ice and fluid
                State = WaterState.IceAndFluid;
                var caloriesForHeatingToZero = -Temperature * Amount;
                energy -= caloriesForHeatingToZero;

                caloriesForMeltingEverything = CaloriesToMeltIcePerGram * Amount;
                ProportionFirstState = 1 - energy / caloriesForMeltingEverything;
                ProportionSecondState = 1 - ProportionFirstState;
                Temperature = 0;
                if (ProportionFirstState > 0) return;
            }
            

            // if fluid
            // if 0
            //if (energy == caloriesForMeltingEverything)
            //{
            //    State = WaterState.Fluid;
            //    return;
            //}
            State = WaterState.Fluid;
            energy -= caloriesForMeltingEverything;

            energyPerGram = energy / (double)Amount;
            temperature = Temperature + energyPerGram;
            if (!(temperature > 100))
            {
                Temperature = temperature;
                return;
            }

            State = WaterState.FluidAndGas;
            var caloriesForHeatingTo100 = (100 - Temperature) * Amount;
            energy -= caloriesForHeatingTo100;

            var caloriesForEvaporatingEverything = CaloriesToEvaporatetWaterPerGram * Amount;
            ProportionFirstState = 1 - energy / caloriesForEvaporatingEverything;
            ProportionSecondState = 1 - ProportionFirstState;
            Temperature = 100;
            if (ProportionFirstState > 0) return;

            State = WaterState.Gas;
            energy -= caloriesForEvaporatingEverything;

            energyPerGram = energy / (double)Amount;
            Temperature += energyPerGram;
        }
    }
}
