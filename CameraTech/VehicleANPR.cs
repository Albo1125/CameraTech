using System;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System.Dynamic;

namespace CameraTech
{
    internal class VehicleANPR : BaseScript
    {
        public static bool Active = false;
        private float frontRange = 75;
        private float backRange = -75;
        private float raycastRadius = 6.2f;
        private Vehicle ANPRvehicle;

        private List<String> checkedPlates = new List<string>();
        

        public VehicleANPR()
        {
            
            EventHandlers["CameraTech:ToggleVehicleANPR"] += new Action<dynamic>((dynamic) =>
            {
                if (Active)
                {
                    Active = false;
                    CameraTech.FixedANPRAlertsToggle = false;
                    CameraTech.RemoveANPRBlips();
                    ANPRvehicle = null;
                    Screen.ShowNotification("Vehicle ANPR " + (Active ? "activated." : "deactivated."));
                }
                else if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    if (CameraTech.ANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                    {
                        Active = true;
                        Screen.ShowNotification("Vehicle ANPR " + (Active ? "activated." : "deactivated.") + " Fixed ANPR alerts activated.");
                        checkedPlates.Clear();
                        ANPRvehicle = Game.Player.Character.CurrentVehicle;
                        CameraTech.FixedANPRAlertsToggle = true;
                        CameraTech.CreateFixedANPRBlips();
                    }
                    else
                    {
                        Screen.ShowNotification("This vehicle does not have ANPR technology.");
                    }
                }
            });

            EventHandlers["CameraTech:ReadPlateInFront"] += new Action<dynamic>((dynamic) =>
            {
                ReadPlateFront();
            });

            EventHandlers["CameraTech:SyncPlateInfo"] += new Action<dynamic>((dynamic plateinfo) =>
            {
                checkedPlates.Clear();
            });
                  
            RunChecks();
        }

        public async void RunChecks()
        {
            
            while (true)
            {
                await Delay(5);               
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
                    if (!Active && ANPRvehicle != null && playerVeh == ANPRvehicle)
                    {
                        Active = true;
                        Screen.ShowNotification("Vehicle ANPR " + (Active ? "activated." : "deactivated."));
                    }

                    if (Active)
                    {
                        
                        Vector3 frontCoord = playerVeh.GetOffsetPosition(new Vector3(0, raycastRadius - 0.5f, 0));

                        Vector3 frontCoordC = playerVeh.GetOffsetPosition(new Vector3(-raycastRadius + 0.1f, frontRange, 0));
                        CheckANPRShapeTest("Front ANPR (L)", ShapeTest.StartShapeTest(frontCoord, frontCoordC, raycastRadius, playerVeh), playerVeh, true);

                        Vector3 frontCoordD = playerVeh.GetOffsetPosition(new Vector3(raycastRadius - 0.1f, frontRange, 0));
                        CheckANPRShapeTest("Front ANPR (R)", ShapeTest.StartShapeTest(frontCoord, frontCoordD, raycastRadius, playerVeh), playerVeh, false);

                        Vector3 backCoord = playerVeh.GetOffsetPosition(new Vector3(0, -raycastRadius + 0.5f, 0));

                        Vector3 backCoordA = playerVeh.GetOffsetPosition(new Vector3(-raycastRadius + 0.1f, backRange, 0));
                        CheckANPRShapeTest("Rear ANPR (L)", ShapeTest.StartShapeTest(backCoord, backCoordA, raycastRadius, playerVeh), playerVeh, true);

                        Vector3 backCoordB = playerVeh.GetOffsetPosition(new Vector3(raycastRadius - 0.1f, backRange, 0));
                        CheckANPRShapeTest("Rear ANPR (R)", ShapeTest.StartShapeTest(backCoord, backCoordB, raycastRadius, playerVeh), playerVeh, false);
                    }
                }
                else if (Active)
                {
                    Active = false;
                    Screen.ShowNotification("Vehicle ANPR " + (Active ? "activated." : "deactivated."));
                }

            }
        }

        public void ReadPlateFront()
        {
            if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
            {
                Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
                Vector3 frontCoordA = playerVeh.GetOffsetPosition(new Vector3(0, 1, 0));
                Vector3 frontCoordB = playerVeh.GetOffsetPosition(new Vector3(0, 30, 0));
                ShapeTest stest = ShapeTest.StartShapeTest(frontCoordA, frontCoordB, 7, playerVeh);
                if (stest.hit && API.IsEntityAVehicle(stest.hitEntity))
                {
                    TriggerEvent("chatMessage", "Read Plate (Front)", new int[] { 0, 191, 255 }, API.GetVehicleNumberPlateText(stest.hitEntity).Trim() + ".");
                }
            }
        }

        private void CheckANPRShapeTest(string cameraname, ShapeTest stest, Vehicle originVeh, bool left)
        {
            if (stest.hit && API.IsEntityAVehicle(stest.hitEntity))
            {
                Vehicle hitVeh = (Vehicle)Vehicle.FromHandle(stest.hitEntity);
                bool positionedRight = Vector3.Distance(originVeh.GetOffsetPosition(Vector3.Left), hitVeh.Position) > Vector3.Distance(originVeh.GetOffsetPosition(Vector3.Right), hitVeh.Position);
                if ((left && positionedRight) || (positionedRight && !positionedRight))
                {
                    return;
                }

                string modelName = API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(stest.hitEntity));
                string plate = API.GetVehicleNumberPlateText(stest.hitEntity).Replace(" ", string.Empty);

                if (!checkedPlates.Contains(plate))
                {
                    checkedPlates.Add(plate);
                    if (CameraTech.PlateInfo != null && CameraTech.PlateInfo.ContainsKey(plate))
                    {
                        float distance = Vector3.Distance(stest.from, hitVeh.Position);
                        string colour = VehicleColour.GetVehicleColour(stest.hitEntity).PrimarySimpleColourName;
                        TriggerEvent("chatMessage", cameraname, new int[] { 255, 128, 0 }, colour + " " + modelName + ". " + plate + ". Dist: " + (int)Math.Round(distance) + ". ^*Markers: ^r" + CameraTech.PlateInfo[plate]);
                        CameraTech.PlayANPRAlertSound();
                        ANPRInterface.VehicleANPRHeaderString = cameraname;
                        ANPRInterface.VehicleANPRInfo = colour + " " + modelName + ". " + plate + ".";
                        ANPRInterface.VehicleANPRMarkers = "~r~" + CameraTech.PlateInfo[plate];
                        ANPRInterface.FlashVehicleANPR();
                    }
                    else
                    {
                        PlayANPRScanSound();
                    }
                }
            }
        }

        
        private void PlayANPRScanSound()
        {
            //API.PlaySoundFrontend(-1, "PIN_BUTTON", "ATM_SOUNDS", true);
        }
    }
}
