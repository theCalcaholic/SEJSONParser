using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        JSON Parser = null;

        public Program()
        {
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // This will deserialize the argument, for demonstration purposes

            if (argument != "")
                Parser = new JSON(argument);

            bool parsingComplete = false;
            try
            {
                // This Method parses the string until a certain time limit is reached or it has finished.
                // Doing this prevents the script from causing lags (we distribute the load evenly over multiple
                // ticks, if necessary)
                parsingComplete = Parser.ParsingComplete();
            }
            catch( Exception e) // in case something went wrong (either your json is wrong or my library has a bug :P)
            {
                Echo("There's somethign wrong with your json: " + e.Message);
                return;
            }

            if( parsingComplete ) // if parsing is complete...
            {
                Echo("Argument has been parsed!");
                Echo(Parser.Result.ToString(true));  // ... output the prettified json
            }
            else  // else...
            {
                Runtime.UpdateFrequency = UpdateFrequency.Once; // ... we'll continue on the next tick
                Echo("Parsing (" + Parser.Progress + "%)..."); // ... and output the parsing progress
            }
        }
    }
}