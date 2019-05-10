using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTech
{
    public class CameraTech : BaseScript
    {
        public static Model[] ANPRModels;
        public static Dictionary<string, string> PlateInfo = new Dictionary<string, string>();
        public static FixedANPR[] FixedANPRCameras;
        public float FixedANPRRadius = 28f;
        public static bool FixedANPRAlertsToggle = false;
        public static bool BlipsCreated = false;
        public static string FocussedPlate = null;
        public static Blip RouteBlip = null;
        private static int ANPRHitChance = 95;

        public CameraTech()
        {
            Debug.Write("CameraTech by Albo1125.");

            TriggerEvent("chat:addSuggestion", "/setplateinfo", "Sets markers for the specified plate", new[]
            {
                new { name = "param", help = "Type the plate, followed by semicolon (;), followed by the ANPR markers (leave markers blank to remove). Ex: AB12CDE;Stolen" },
            });

            TriggerEvent("chat:addSuggestion", "/setvehinfo", "Sets markers for the plate of the vehicle you're currently in", new[]
            {
                new { name = "markers", help = "The markers for this vehicle's plate, leave blank to remove from the system. Ex: Stolen" },
            });

            EventHandlers["CameraTech:ClUpdateVehicleInfo"] += new Action<string>((string plateinfo) =>
            {
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    string plate = API.GetVehicleNumberPlateText(Game.Player.Character.CurrentVehicle.Handle).Replace(" ", string.Empty);
                    TriggerServerEvent("CameraTech:UpdateVehicleInfo", plate, plateinfo);
                    if (plateinfo != null)
                    {
                        TriggerEvent("chatMessage", "SYSTEM", new int[] { 0, 0, 0 }, "Plate: " + plate + ". Info: " + plateinfo);
                    }
                    else
                    {
                        TriggerEvent("chatMessage", "SYSTEM", new int[] { 0, 0, 0 }, "Plate: " + plate + ". No info.");
                    }
                }
            });

            EventHandlers["CameraTech:ClFixedANPRAlert"] += new Action<string, string, string, string, string>((string colour, string modelName, string anprname, string dir, string plate) =>
            {
                if (FixedANPRAlertsToggle && Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists() &&
                    ANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                {
                    if (FocussedPlate != null && FocussedPlate == plate)
                    {
                        TriggerEvent("chatMessage", "Fixed ANPR", new int[] { 0, 191, 255 }, colour + " " + modelName + ". " + plate + ". " + anprname + " (" + dir + "). ^*Markers: ^r" + PlateInfo[plate]);
                        PlayANPRAlertSound();
                        if (BlipsCreated) {
                            FixedANPR fixedanpr = FixedANPRCameras.Where(x => x.Name == anprname).FirstOrDefault();
                            if (fixedanpr != null && fixedanpr.blip.Exists())
                            {
                                fixedanpr.blip.ShowRoute = true;
                                RouteBlip = fixedanpr.blip;
                            }
                        }
                        ANPRInterface.FixedANPRHeaderString = anprname + " (" + dir + ")";
                        ANPRInterface.FixedANPRInfo = colour + " " + modelName + ". " + plate + ".";
                        ANPRInterface.FixedANPRMarkers = "~r~" + PlateInfo[plate];
                        ANPRInterface.FlashFixedANPR();
                    }
                    else if (FocussedPlate == null)
                    {
                        TriggerEvent("chatMessage", "Fixed ANPR", new int[] { 0, 191, 255 }, colour + " " + modelName + ". " + plate + ". " + anprname + " (" + dir + "). ^*Markers: ^r" + PlateInfo[plate]);
                        PlayANPRAlertSound();
                        ANPRInterface.FixedANPRHeaderString = anprname + " (" + dir + ")";
                        ANPRInterface.FixedANPRInfo = colour + " " + modelName + ". " + plate + ".";
                        ANPRInterface.FixedANPRMarkers = "~r~" + PlateInfo[plate];
                        ANPRInterface.FlashFixedANPR();
                    }
                }
            });

            EventHandlers["CameraTech:FixedANPRToggle"] += new Action<dynamic>((dynamic) =>
            {
                if (FixedANPRAlertsToggle)
                {
                    FixedANPRAlertsToggle = false;
                    Screen.ShowNotification("Fixed ANPR alerts " + (FixedANPRAlertsToggle ? "activated." : "deactivated."));
                    RemoveANPRBlips();
                }
                else if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    if (ANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                    {
                        FixedANPRAlertsToggle = true;
                        Screen.ShowNotification("Fixed ANPR alerts " + (FixedANPRAlertsToggle ? "activated." : "deactivated."));
                        CreateFixedANPRBlips();
                    }
                    else
                    {
                        Screen.ShowNotification("This vehicle does not have ANPR technology.");
                    }
                }
            });

            EventHandlers["CameraTech:SyncPlateInfo"] += new Action<dynamic>((dynamic plateinfo) =>
            {
                PlateInfo.Clear();
                if (plateinfo != null)
                {
                    foreach (KeyValuePair<string, dynamic> item in plateinfo)
                    {
                        PlateInfo[item.Key] = item.Value.ToString();
                    }
                }
            });

            EventHandlers["CameraTech:FocusANPR"] += new Action<string>((string plate) =>
            {
                if (FixedANPRAlertsToggle && Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists() &&
                    ANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                {
                    FocussedPlate = plate;
                    if (FocussedPlate == null)
                    {
                        TriggerEvent("chatMessage", "Fixed ANPR", new int[] { 0, 191, 255 }, "Removed fixed ANPR plate focus.");
                        if (RouteBlip != null && RouteBlip.Exists())
                        {
                            RouteBlip.ShowRoute = false;
                            RouteBlip = null;
                        }
                    }
                    else
                    {
                        TriggerEvent("chatMessage", "Fixed ANPR", new int[] { 0, 191, 255 }, "Fixed ANPR plate focus: " + plate);
                    }
                }
            });

            string resourceName = API.GetCurrentResourceName();
            string anprvehs = API.LoadResourceFile(resourceName, "anprvehicles.txt");
            ANPRModels = anprvehs.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(x => new Model(x)).ToArray();

            string fixedanprjson = API.LoadResourceFile(resourceName, "fixedanprcameras.json");
            FixedANPRCameras = JsonConvert.DeserializeObject<FixedANPR[]>(fixedanprjson);

            Main();
        }

        public static string DegreesToCardinal(double degrees)
        {
            string[] caridnals = { "N", "NW", "W", "SW", "S", "SE", "E", "NE", "N" };
            return caridnals[(int)Math.Round(((double)degrees % 360) / 45)];
        }

        public async void Main()
        {
            FixedANPR lastTriggeredANPRCamera = null;
            DateTime timeLastTriggeredANPRCamera = DateTime.Now;
            string lastTriggeredDir = "";

            FixedANPR LastBlockedFixedANPR = null;
            DateTime timeLastBlockedANPR = DateTime.Now;
            while (true)
            {
                await Delay(5);
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    Vehicle playerVeh = Game.Player.Character.CurrentVehicle;                   
                    if (BlipsCreated && !ANPRModels.Contains(playerVeh.Model))
                    {
                        RemoveANPRBlips();
                    }
                    else if (FixedANPRAlertsToggle && !BlipsCreated && ANPRModels.Contains(playerVeh.Model))
                    {
                        CreateFixedANPRBlips();
                    }

                    if (RouteBlip != null && RouteBlip.Exists() && Vector3.Distance(playerVeh.Position, RouteBlip.Position) < 140)
                    {
                        RouteBlip.ShowRoute = false;
                        RouteBlip = null;
                    }

                    //Fixed ANPR Detection
                    if (playerVeh.Driver == Game.Player.Character && playerVeh.Speed > 0.2f)
                    {
                        string plate = API.GetVehicleNumberPlateText(playerVeh.Handle).Replace(" ", string.Empty);

                        if (PlateInfo.ContainsKey(plate))
                        {
                            FixedANPR anpr = FixedANPRCameras.OrderBy(x => Vector3.Distance(x.Location, playerVeh.Position)).FirstOrDefault();
                            
                            if (anpr != null)
                            {
                                float ZDiff = Math.Abs(anpr.Z - playerVeh.Position.Z);
                                string dir = DegreesToCardinal(playerVeh.Heading);
                                if (Vector3.Distance(anpr.Location, playerVeh.Position) <= FixedANPRRadius && ZDiff < 3.5f && (lastTriggeredANPRCamera != anpr || (DateTime.Now - timeLastTriggeredANPRCamera).Seconds > 20 || lastTriggeredDir != dir))
                                {
                                    if ((anpr != LastBlockedFixedANPR || (DateTime.Now - timeLastTriggeredANPRCamera).Seconds > 120) && API.GetRandomIntInRange(0, 100) < ANPRHitChance)
                                    {
                                        //alert hit
                                        await Delay(1800);
                                        dir = DegreesToCardinal(playerVeh.Heading);
                                        string modelName = API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(playerVeh.Handle));
                                        
                                        string colour = VehicleColour.GetVehicleColour(playerVeh.Handle).PrimarySimpleColourName;
                                        TriggerServerEvent("CameraTech:FixedANPRAlert", colour, modelName, anpr.Name, dir, plate);
                                        lastTriggeredANPRCamera = anpr;
                                        timeLastTriggeredANPRCamera = DateTime.Now;
                                        lastTriggeredDir = dir;
                                        LastBlockedFixedANPR = null;
                                    }
                                    else
                                    {
                                        if (LastBlockedFixedANPR != anpr)
                                        {
                                            timeLastBlockedANPR = DateTime.Now;
                                        }
                                        LastBlockedFixedANPR = anpr;                                                                               
                                    }
                                }
                            }                            
                        }
                    }
                }
                else if (BlipsCreated)
                {
                    RemoveANPRBlips();
                }
            }
        }

        public static void PlayANPRAlertSound()
        {
            API.PlaySound(-1, "TIMER_STOP", "HUD_MINI_GAME_SOUNDSET", false, 0, false);
        }

        public static void CreateFixedANPRBlips()
        {
            RemoveANPRBlips();
            foreach (FixedANPR anpr in FixedANPRCameras)
            {
                Blip blip = World.CreateBlip(anpr.Location);
                blip.Sprite = BlipSprite.Camera;
                blip.Color = BlipColor.White;
                blip.Scale = 0.7f;
                blip.IsShortRange = true;
                blip.Name = "ANPR";

                anpr.blip = blip;
            }
            BlipsCreated = true;
        }

        public static void RemoveANPRBlips()
        {
            foreach (FixedANPR anpr in FixedANPRCameras)
            {
                if (anpr.blip != null && anpr.blip.Exists())
                {
                    anpr.blip.ShowRoute = false;
                    anpr.blip.Delete();
                }
                anpr.blip = null;
            }
            BlipsCreated = false;
        }
    }
}
