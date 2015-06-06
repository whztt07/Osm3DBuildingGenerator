using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Osm3DBuildingGenerator {
    interface DataRequest {


        List<List<ModelUIElement3D>> getBuildingSurfaces();

        /// <summary>
        /// Returns list of building walls
        /// </summary>
        /// <returns>list of building walls</returns>
        List<List<ModelUIElement3D>> getBuildingWalls();
    

        /// <summary>
        /// Returns list of landuses
        /// </summary>
        /// <returns>list of landuses</returns>
        List<List<ModelUIElement3D>> getLanduseSurfaces();

        /// <summary>
        /// Returns camera
        /// </summary>
        /// <returns>PerspectiveCamera</returns>
        PerspectiveCamera getCamera();
    }
}
