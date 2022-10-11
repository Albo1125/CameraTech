using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTech
{
    public class ANPRModel
    {
        public string ModelName = "";
        public string ANPRAccessType = "";

        public bool CanAccessFixedANPR => ANPRAccessType == "fixed only" || ANPRAccessType == "full";
        public bool CanAccessVehicleANPR => ANPRAccessType == "vehicle only" || ANPRAccessType == "full";

        public ANPRModel(string modelName, string aNPRAccessType)
        {
            ModelName = modelName;
            ANPRAccessType = aNPRAccessType.ToLower();
        }
    }
}
