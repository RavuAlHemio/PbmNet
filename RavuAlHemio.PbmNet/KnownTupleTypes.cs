using System.Collections.Generic;
using System.Linq;

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

            var pieces = componentString.Split('_');
            return pieces.Select(piece => KnownColors.ContainsKey(piece) ? KnownColors[piece] : Component.Unknown);
        }

        public static string EncodeComponentString(IEnumerable<Component> components)
        {
            var componentList = new List<Component>(components);

            // look into known types
            foreach (var knownType in KnownTypes)
            {
                if (knownType.Value.SequenceEqual(componentList))
                {
                    return knownType.Key;
                }
            }

            // no known type? collect the colors
            var retList = new List<string>();
            foreach (var component in componentList)
            {
                bool foundThisColor = false;
                foreach (var color in KnownColors)
                {
                    if (color.Value == component)
                    {
                        retList.Add(color.Key);
                        foundThisColor = true;
                        break;
                    }
                }

                if (!foundThisColor)
                {
                    retList.Add("UNKNOWN");
                }
            }

            return string.Join("_", retList);
        }
    }
}
