using System;
using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    internal class KnownTupleTypes
    {
        private static readonly Dictionary<string, Component[]> KnownTypes = new Dictionary<string, Component[]>
        {
            {"BLACKANDWHITE", new [] {Component.White}},
            {"BLACKANDWHITE_ALPHA", new [] {Component.White, Component.Alpha}},
            {"GRAYSCALE", new [] {Component.White}},
            {"GRAYSCALE_ALPHA", new [] {Component.White, Component.Alpha}},
            {"RGB", new [] {Component.Red, Component.Green, Component.Blue}},
            {"RGB_ALPHA", new [] {Component.Red, Component.Green, Component.Blue, Component.Alpha}},

            // extension, compatible with GhostScript
            {"CMYK", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black}},

            // extensions
            {"CMYK_ALPHA", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.Alpha}},
            {"CMYKOG", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.Orange, Component.SubtractiveGreen}},
            {"CMYKOG_ALPHA", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.Orange, Component.SubtractiveGreen, Component.Alpha}},
            {"CMYKcm", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.LightCyan, Component.LightMagenta}},
            {"CMYKcm_ALPHA", new [] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.LightCyan, Component.LightMagenta, Component.Alpha}}
        };

        private static readonly Dictionary<string, Component> KnownColors = new Dictionary<string, Component>
        {
            {"UNKNOWN", Component.Unknown},
            {"WHITE", Component.White},
            {"RED", Component.Red},
            {"GREEN", Component.Green},
            {"BLUE", Component.Blue},
            {"CYAN", Component.Cyan},
            {"MAGENTA", Component.Magenta},
            {"YELLOW", Component.Yellow},
            {"BLACK", Component.Black},
            {"ALPHA", Component.Alpha},
            {"ORANGE", Component.Orange},
            {"SUBTRACTIVEGREEN", Component.SubtractiveGreen},
            {"LIGHTCYAN", Component.LightCyan},
            {"LIGHTMAGENTA", Component.LightMagenta}
        };


        public static IEnumerable<Component> DecodeComponentString(string componentString)
        {
            if (KnownTypes.ContainsKey(componentString))
            {
                return KnownTypes[componentString];
            }

            // TODO
            throw new NotImplementedException();
        }
    }
}
