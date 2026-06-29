using LaserCad.Core.Documents;
using TMPro;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Wyswietla podstawowe informacje o dokumencie CAD.
    /// </summary>
    public sealed class DocumentInfoView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text documentNameText;

        [SerializeField]
        private TMP_Text documentStatsText;

        /// <summary>
        /// Aktualizuje teksty UI na podstawie dokumentu domenowego.
        /// </summary>
        /// <param name="document">Dokument, ktory ma zostac opisany w UI.</param>
        public void Show(CadDocument document)
        {
            if (documentNameText != null)
            {
                documentNameText.text = document.Name;
            }

            if (documentStatsText != null)
            {
                documentStatsText.text =
                    $"Format: {document.FormatVersion} | Layers: {document.Layers.Count} | Sketches: {document.Sketches.Count}";
            }
        }
    }
}
