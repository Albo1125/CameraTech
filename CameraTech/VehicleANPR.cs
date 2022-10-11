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

        public static List<string> checkedPlates = new List<string>();

        public VehicleANPR()
        {
            EventHandlers["fivemskillsreset"] += new Action<dynamic>((dynamic) =>
            {
                ANPRInterface.toggleMasterInterface(false);
                toggleVehicleANPR(false);
                CameraTech.toggleFixedANPR(false);
            });

            EventHandlers["CameraTech:ToggleVehicleANPR"] += new Action<dynamic>((dynamic) =>
            {
                if (Active)
                {
                    toggleVehicleANPR(false);
                    Screen.ShowNotification("Vehicle ANPR deactivated.");
                }
                else if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    if (CameraTech.VehicleANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                    {
                        toggleVehicleANPR(true);
                        Screen.ShowNotification("Vehicle ANPR activated.");
                        if (!ANPRInterface.MasterInterfaceToggle)
                        {
                            ANPRInterface.toggleMasterInterface(true);
                        }
                        if (!CameraTech.FixedANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                        {
                            CameraTech.toggleFixedANPR(false);
                        }
                    }
                    else
                    {
                        Screen.ShowNotification("This vehicle does not have Vehicle ANPR technology.");
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

        public static void toggleVehicleANPR(bool toggle)
        {
            if (!toggle)
            {
                Active = false;
                checkedPlates.Clear();
            } 
            else
            {
                Active = true;
                checkedPlates.Clear();
            }
        }

        public async void RunChecks()
        {
            
            while (true)
            {
                await Delay(5);               
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists())
                {
                    Vehicle playerVeh = Game.Player.Character.CurrentVehicle;

                    if (ANPRInterface.MasterInterfaceToggle && Active && playerVeh == ANPRInterface.ANPRvehicle)
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
                        CameraTech.PlayANPRAlertSound(true);
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
