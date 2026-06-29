using System;
using System.Collections.Generic;
using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Przechowuje aktualne zaznaczenie encji szkicu w widoku Unity.
    /// </summary>
    public sealed class SelectionService : MonoBehaviour
    {
        private readonly HashSet<Guid> selectedEntityIds = new HashSet<Guid>();

        /// <summary>
        /// Identyfikatory aktualnie zaznaczonych encji.
        /// </summary>
        public IReadOnlyCollection<Guid> SelectedEntityIds
        {
            get { return selectedEntityIds; }
        }

        /// <summary>
        /// Liczba zaznaczonych encji.
        /// </summary>
        public int SelectionCount
        {
            get { return selectedEntityIds.Count; }
        }

        /// <summary>
        /// Czysci zaznaczenie.
        /// </summary>
        public void Clear()
        {
            selectedEntityIds.Clear();
        }

        /// <summary>
        /// Zaznacza tylko jedna encje.
        /// </summary>
        public void SelectOnly(ISketchEntity entity)
        {
            selectedEntityIds.Clear();

            if (entity != null)
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Przelacza zaznaczenie encji bez zmiany pozostalych elementow.
        /// </summary>
        public void Toggle(ISketchEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            if (!selectedEntityIds.Remove(entity.Id))
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Sprawdza, czy encja jest zaznaczona.
        /// </summary>
        public bool IsSelected(ISketchEntity entity)
        {
            return entity != null && selectedEntityIds.Contains(entity.Id);
        }
    }
}
