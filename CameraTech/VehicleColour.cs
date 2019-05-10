using CameraTech;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTech
{
    /// <summary>
    /// Thank you Stealth22 and Alexguirre for their contributions, which I have used as a base for this struct.
    /// </summary>
    public struct VehicleColour
    {
        public static string[] SimpleColours = new string[] { "Black", "Graphite", "Steel", "Silver", "Red", "Pink", "Orange", "Yellow", "Green", "Blue", "Brown", "Purple", "White", "Gray", "Gold", "Steel", "Aluminium", "Beige" };

        public static VehicleColour GetVehicleColour(int handle)
        {
            int primary = -1;
            int secondary = -1;
            API.GetVehicleColours(handle, ref primary, ref secondary);
            VehicleColour vc = new VehicleColour();
            vc.PrimaryColour = (EPaint)primary;
            vc.SecondaryColour = (EPaint)secondary;
            return vc;
        }

        /// <summary>
        /// Gets the colour name
        /// </summary>
        /// <param name="paint">Colour to get the name from</param>
        /// <returns></returns>
        public static string GetColourName(EPaint paint)
        {
            String name = Enum.GetName(typeof(EPaint), paint);
            return name.Replace("_", " ");
        }

        /// <summary>
        /// Gets the simple colour name
        /// </summary>
        /// <param name="paint">Colour to get the simple name from</param>
        /// <returns></returns>
        public static string GetSimpleColourName(EPaint paint)
        {
            String name = Enum.GetName(typeof(EPaint), paint).Replace("_", " ");
            foreach (string simplecolour in SimpleColours)
            {
                if (name.ToLower().Contains(simplecolour.ToLower()))
                {
                    return simplecolour;
                }
            }
            return name;
        }

        /// <summary>
        /// The primary colour paint index 
        /// </summary>
        public EPaint PrimaryColour { get; set; }

        /// <summary>
        /// The secondary colour paint index 
        /// </summary>
        public EPaint SecondaryColour { get; set; }

        /// <summary>
        /// Gets the primary colour name
        /// </summary>
        public string PrimaryColourName
        {
            get { return GetColourName(PrimaryColour); }
        }
        /// <summary>
        /// Gets the secondary colour name
        /// </summary>
        public string SecondaryColourName
        {
            get { return GetColourName(SecondaryColour); }
        }

        /// <summary>
        /// Gets the primary simple colour name
        /// </summary>
        public string PrimarySimpleColourName
        {
            get { return GetSimpleColourName(PrimaryColour); }
        }
        /// <summary>
        /// Gets the secondary simple colour name
        /// </summary>
        public string SecondarySimpleColourName
        {
            get { return GetSimpleColourName(SecondaryColour); }
        }
    }
    
    public enum EPaint
    {
        Unknown = -1,
        /* CLASSIC|METALLIC */
        Black = 0,
        Carbon_Black = 147,
        Graphite = 1,
        Anhracite_Black = 11,
        Black_Steel = 2,
        Dark_Steel = 3,
        Silver = 4,
        Bluish_Silver = 5,
        Rolled_Steel = 6,
        Shadow_Silver = 7,
        Stone_Silver = 8,
        Midnight_Silver = 9,
        Cast_Iron_Silver = 10,
        Red = 27,
        Torino_Red = 28,
        Formula_Red = 29,
        Lava_Red = 150,
        Blaze_Red = 30,
        Grace_Red = 31,
        Garnet_Red = 32,
        Sunset_Red = 33,
        Cabernet_Red = 34,
        Wine_Red = 143,
        Candy_Red = 35,
        Hot_Pink = 135,
        Pfister_Pink = 137,
        Salmon_Pink = 136,
        Sunrise_Orange = 36,
        Orange = 38,
        Bright_Orange = 138,
        Gold = 37,
        Bronze = 90,
        Yellow = 88,
        Race_Yellow = 89,
        Dew_Yellow = 91,
        Green = 139,
        Dark_Green = 49,
        Racing_Green = 50,
        Sea_Green = 51,
        Olive_Green = 52,
        Bright_Green = 53,
        Gasoline_Green = 54,
        Lime_Green = 92,
        Hunter_Green = 144,
        Securiror_Green = 125,
        Midnight_Blue = 141,
        Galaxy_Blue = 61,
        Dark_Blue = 62,
        Saxon_Blue = 63,
        Blue = 64,
        Bright_Blue = 140,
        Mariner_Blue = 65,
        Harbor_Blue = 66,
        Diamond_Blue = 67,
        Surf_Blue = 68,
        Nautical_Blue = 69,
        Racing_Blue = 73,
        Ultra_Blue = 70,
        Light_Blue = 74,
        Police_Car_Blue = 127,
        Epsilon_Blue = 157,
        Chocolate_Brown = 96,
        Bison_Brown = 101,
        Creek_Brown = 95,
        Feltzer_Brown = 94,
        Maple_Brown = 97,
        Beechwood_Brown = 103,
        Sienna_Brown = 104,
        Saddle_Brown = 98,
        Moss_Brown = 100,
        Woodbeech_Brown = 102,
        Straw_Brown = 99,
        Sandy_Brown = 105,
        Bleached_Brown = 106,
        Schafter_Purple = 71,
        Spinnaker_Purple = 72,
        Midnight_Purple = 142,
        Metallic_Midnight_Purple = 146,
        Bright_Purple = 145,
        Cream = 107,
        Ice_White = 111,
        Frost_White = 112,
        Pure_White = 134,
        Default_Alloy = 156,
        Champagne = 93,

        /* MATTE */
        Matte_Black = 12,
        Matte_Gray = 13,
        Matte_Light_Gray = 14,
        Matte_Ice_White = 131,
        Matte_Blue = 83,
        Matte_Dark_Blue = 82,
        Matte_Midnight_Blue = 84,
        Matte_Midnight_Purple = 149,
        Matte_Schafter_Purple = 148,
        Matte_Red = 39,
        Matte_Dark_Red = 40,
        Matte_Orange = 41,
        Matte_Yellow = 42,
        Matte_Lime_Green = 55,
        Matte_Green = 128,
        Matte_Forest_Green = 151,
        Matte_Foliage_Green = 155,
        Matte_Brown = 129,
        Matte_Olive_Darb = 152,
        Matte_Dark_Earth = 153,
        Matte_Desert_Tan = 154,

        /* Util */
        Util_Black = 15,
        Util_Black_Poly = 16,
        Util_Dark_Silver = 17,
        Util_Silver = 18,
        Util_Gun_Metal = 19,
        Util_Shadow_Silver = 20,
        Util_Red = 43,
        Util_Bright_Red = 44,
        Util_Garnet_Red = 45,
        Util_Dark_Green = 56,
        Util_Green = 57,
        Util_Dark_Blue = 75,
        Util_Midnight_Blue = 76,
        Util_Blue = 77,
        Util_Sea_Foam_Blue = 78,
        Util_Lightning_Blue = 79,
        Util_Maui_Blue_Poly = 80,
        Util_Bright_Blue = 81,
        Util_Brown = 108,
        Util_Medium_Brown = 109,
        Util_Light_Brown = 110,
        Util_Off_White = 122,

        /* Worn */
        Worn_Black = 21,
        Worn_Graphite = 22,
        Worn_Silver_Gray = 23,
        Worn_Silver = 24,
        Worn_Blue_Silver = 25,
        Worn_Shadow_Silver = 26,
        Worn_Red = 46,
        Worn_Golden_Red = 47,
        Worn_Dark_Red = 48,
        Worn_Dark_Green = 58,
        Worn_Green = 59,
        Worn_Sea_Wash = 60,
        Worn_Dark_Blue = 85,
        Worn_Blue = 86,
        Worn_Light_Blue = 87,
        Worn_Honey_Beige = 113,
        Worn_Brown = 114,
        Worn_Dark_Brown = 115,
        Worn_Straw_Beige = 116,
        Worn_Off_White = 121,
        Worn_Yellow = 123,
        Worn_Light_Orange = 124,
        Worn_Taxi_Yellow = 126,
        Worn_Orange = 130,
        Worn_White = 132,
        Worn_Olive_Army_Green = 133,

        /* METALS */
        Brushed_Steel = 117,
        Brushed_Black_Steel = 118,
        Brushed_Aluminum = 119,
        Pure_Gold = 158,
        Brushed_Gold = 159,
        Secret_Gold = 160,

        /* CHROME */
        Chrome = 120,
    }
}
