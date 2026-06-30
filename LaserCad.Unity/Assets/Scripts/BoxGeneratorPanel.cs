using System;
using System.Globalization;
using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry.Units;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel edycji opcji generatora pudelka.
    /// </summary>
    public sealed class BoxGeneratorPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(12f, 196f, 300f, 270f);

        private string widthText;
        private string depthText;
        private string heightText;
        private string materialThicknessText;
        private string kerfText;
        private string fingerWidthText;
        private string clearanceText;
        private int selectedBoxTypeIndex;
        private string statusText = "Gotowe";
        private bool initialized;

        private static readonly string[] BoxTypeLabels =
        {
            "Zamkniete",
            "Otwarte",
            "Z pokrywa",
        };

        private void OnGUI()
        {
            if (ViewportProcessMode.IsViewportProcess())
            {
                return;
            }

            if (applicationController == null)
            {
                return;
            }

            EnsureInitialized();
            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Generator pudelka");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.Label("Wymiary [mm]");

            widthText = DrawTextField("Szerokosc", widthText);
            depthText = DrawTextField("Glebokosc", depthText);
            heightText = DrawTextField("Wysokosc", heightText);
            materialThicknessText = DrawTextField("Material", materialThicknessText);
            kerfText = DrawTextField("Kerf", kerfText);
            fingerWidthText = DrawTextField("Palec", fingerWidthText);
            clearanceText = DrawTextField("Clearance", clearanceText);

            GUILayout.Space(6f);
            GUILayout.Label("Typ");
            selectedBoxTypeIndex = GUILayout.SelectionGrid(selectedBoxTypeIndex, BoxTypeLabels, 1);

            GUILayout.Space(8f);
            if (GUILayout.Button("Zastosuj"))
            {
                ApplyOptions();
            }

            GUILayout.Label(statusText);
            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }

        private static string DrawTextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(86f));
            var result = GUILayout.TextField(value, GUILayout.Width(96f));
            GUILayout.Label("mm", GUILayout.Width(28f));
            GUILayout.EndHorizontal();
            return result;
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            var options = applicationController.CurrentBoxOptions ?? new BoxGeneratorOptions();
            widthText = FormatLength(options.Width);
            depthText = FormatLength(options.Depth);
            heightText = FormatLength(options.Height);
            materialThicknessText = FormatLength(options.MaterialThickness);
            kerfText = FormatLength(options.Kerf);
            fingerWidthText = FormatLength(options.FingerWidth);
            clearanceText = FormatLength(options.Clearance);
            selectedBoxTypeIndex = BoxTypeToIndex(options.BoxType);
            initialized = true;
        }

        private void ApplyOptions()
        {
            try
            {
                var options = new BoxGeneratorOptions(
                    ParseLength(widthText, "Szerokosc"),
                    ParseLength(depthText, "Glebokosc"),
                    ParseLength(heightText, "Wysokosc"),
                    ParseLength(materialThicknessText, "Material"),
                    ParseLength(kerfText, "Kerf"),
                    ParseLength(fingerWidthText, "Palec"),
                    ParseLength(clearanceText, "Clearance"),
                    IndexToBoxType(selectedBoxTypeIndex));

                applicationController.SetBoxOptions(options);
                statusText = "Opcje poprawne";
            }
            catch (Exception exception)
            {
                statusText = exception.Message;
            }
        }

        private static Length ParseLength(string value, string fieldName)
        {
            double millimeters;
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out millimeters))
            {
                throw new ArgumentException(fieldName + ": wpisz liczbe w mm.");
            }

            return Length.FromMillimeters(millimeters);
        }

        private static string FormatLength(Length length)
        {
            return length.Millimeters.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static int BoxTypeToIndex(BoxGeneratorType boxType)
        {
            switch (boxType)
            {
                case BoxGeneratorType.Closed:
                    return 0;
                case BoxGeneratorType.Open:
                    return 1;
                case BoxGeneratorType.WithLid:
                    return 2;
                default:
                    return 1;
            }
        }

        private static BoxGeneratorType IndexToBoxType(int index)
        {
            switch (index)
            {
                case 0:
                    return BoxGeneratorType.Closed;
                case 2:
                    return BoxGeneratorType.WithLid;
                default:
                    return BoxGeneratorType.Open;
            }
        }
    }
}
