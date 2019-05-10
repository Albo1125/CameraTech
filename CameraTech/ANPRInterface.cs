﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Font = CitizenFX.Core.UI.Font;
using Rectangle = CitizenFX.Core.UI.Rectangle;

namespace CameraTech
{
    internal class ANPRInterface : BaseScript
    {
        public static string VehicleANPRHeaderString = "";
        public static string FixedANPRHeaderString = "";
        public static string VehicleANPRInfo = "~g~ACTIVATED";
        public static string FixedANPRInfo = "~g~ACTIVATED";
        public static string VehicleANPRMarkers = "";
        public static string FixedANPRMarkers = "";

        private static bool MasterInterfaceToggle = true;
        private const float scale = 0.345f;

        public ANPRInterface()
        {
            EventHandlers["CameraTech:MasterInterfaceToggle"] += new Action<dynamic>((dynamic) =>
            {
                MasterInterfaceToggle = !MasterInterfaceToggle;
                Screen.ShowNotification("ANPR Interface " + (MasterInterfaceToggle ? "activated." : "deactivated."));
            });
            drawingAsync();
        }

        private async Task drawingAsync()
        {
            while (true)
            {
                await Delay(0);

                if (MasterInterfaceToggle && (CameraTech.FixedANPRAlertsToggle || VehicleANPR.Active))
                {
                    //API.DrawRect(0.508f, 0.94f, 0.196f, 0.116f, 0, 0, 0, 150);
                    Rectangle rect = new Rectangle(new PointF(Screen.Width * 0.001f, Screen.Height * 0.5f), new SizeF(Screen.Width * 0.16f, Screen.Height * 0.126f), Color.FromArgb(150, 0, 0, 0));
                    Text vehicleANPRHeader = new Text("Vehicle ANPR - " + VehicleANPRHeaderString, new PointF(Screen.Width * 0.08f, Screen.Height * 0.501f), scale, Color.FromArgb(255, 0, 191, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    Text vehicleANPRInfoText = new Text(VehicleANPRInfo, new PointF(Screen.Width * 0.08f, Screen.Height * 0.519f), scale, Color.FromArgb(255, 255, 255, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    Text vehicleANPRMarkersText = new Text(VehicleANPRMarkers, new PointF(Screen.Width * 0.08f, Screen.Height * 0.537f), scale, Color.FromArgb(255, 255, 255, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    if (!VehicleANPR.Active)
                    {
                        vehicleANPRInfoText.Caption = "~r~DISABLED";
                        vehicleANPRMarkersText.Caption = "";
                    }

                    string focussedString = "Fixed";
                    if (CameraTech.FocussedPlate != null)
                    {
                        focussedString = "Focussed";
                    }
                    Text fixedANPRHeader = new Text(focussedString + " ANPR - " + FixedANPRHeaderString, new PointF(Screen.Width * 0.08f, Screen.Height * 0.555f), scale, Color.FromArgb(255, 0, 191, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    Text fixedANPRInfoText = new Text(FixedANPRInfo, new PointF(Screen.Width * 0.08f, Screen.Height * 0.573f), scale, Color.FromArgb(255, 255, 255, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    Text fixedANPRMarkersText = new Text(FixedANPRMarkers, new PointF(Screen.Width * 0.08f, Screen.Height * 0.591f), scale, Color.FromArgb(255, 255, 255, 255), Font.ChaletComprimeCologne, Alignment.Center);
                    if (!CameraTech.FixedANPRAlertsToggle)
                    {
                        fixedANPRInfoText.Caption = "~r~DISABLED";
                        fixedANPRMarkersText.Caption = "";
                    }

                    if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists() && Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists() &&
                            CameraTech.ANPRModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                    {
                        rect.Draw();                       
                        vehicleANPRHeader.Draw();
                        vehicleANPRInfoText.Draw();
                        vehicleANPRMarkersText.Draw();
                        fixedANPRHeader.Draw();
                        fixedANPRInfoText.Draw();
                        fixedANPRMarkersText.Draw();
                    }
                }
            }
        }

        public static async Task FlashVehicleANPR(int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                VehicleANPRHeaderString = "~r~" + VehicleANPRHeaderString;
                await Delay(200);
                VehicleANPRHeaderString = VehicleANPRHeaderString.Replace("~r~", "");
                await Delay(200);
            }
        }

        public static async Task FlashFixedANPR(int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                FixedANPRHeaderString = "~r~" + FixedANPRHeaderString;
                await Delay(200);
                FixedANPRHeaderString = FixedANPRHeaderString.Replace("~r~", "");
                await Delay(200);
            }
        }
    }
}
