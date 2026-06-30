using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel historii komend edycyjnych MVP.
    /// </summary>
    public sealed class UndoRedoPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(616f, 276f, 300f, 118f);

        private void OnGUI()
        {
            if (applicationController == null)
            {
                return;
            }

            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Historia");
        }

        private void DrawWindow(int windowId)
        {
            var history = applicationController.CurrentHistory;
            int undoCount = history != null ? history.UndoCount : 0;
            int redoCount = history != null ? history.RedoCount : 0;

            GUILayout.Label("Undo: " + undoCount + " | Redo: " + redoCount);

            GUILayout.BeginHorizontal();
            GUI.enabled = history != null && history.CanUndo;
            if (GUILayout.Button("Cofnij"))
            {
                applicationController.Undo();
            }

            GUI.enabled = history != null && history.CanRedo;
            if (GUILayout.Button("Ponow"))
            {
                applicationController.Redo();
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }
    }
}
