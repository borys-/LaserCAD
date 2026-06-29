using LaserCad.Core.Documents;
using System.Reflection;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Wyswietla podstawowe informacje o dokumencie CAD.
    /// </summary>
    public sealed class DocumentInfoView : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour documentNameText;

        [SerializeField]
        private MonoBehaviour documentStatsText;

        /// <summary>
        /// Aktualizuje teksty UI na podstawie dokumentu domenowego.
        /// </summary>
        /// <param name="document">Dokument, ktory ma zostac opisany w UI.</param>
        public void Show(CadDocument document)
        {
            if (documentNameText != null)
            {
                SetText(documentNameText, document.Name);
            }

            if (documentStatsText != null)
            {
                SetText(
                    documentStatsText,
                    $"Format: {document.FormatVersion} | Layers: {document.Layers.Count} | Sketches: {document.Sketches.Count}");
            }
        }

        private static void SetText(MonoBehaviour target, string value)
        {
            PropertyInfo textProperty = target.GetType().GetProperty("text");
            if (textProperty != null && textProperty.CanWrite && textProperty.PropertyType == typeof(string))
            {
                textProperty.SetValue(target, value);
            }
        }
    }
}
