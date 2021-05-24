using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public static class StaticStructureObjects
    {
        public static StructureSelectionPanel theStructureSelectionPanel;
        public static StructurePlacer theStructurePlacer;

        public static void SwitchSelectedStructures(GameObject _structureToReplace)
        {
            theStructurePlacer.selectedStructure = _structureToReplace;
        }
    }
}