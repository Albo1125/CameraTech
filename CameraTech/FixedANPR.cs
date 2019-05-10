using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTech
{
    public class FixedANPR
    {
        public string Name;
        public float X, Y, Z;

        public Vector3 Location => new Vector3(X, Y, Z);

        [JsonIgnore]
        public Blip blip;

        public FixedANPR(string name, float x, float y, float z)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }

    }
}
