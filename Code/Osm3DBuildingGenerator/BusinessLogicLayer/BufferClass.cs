using Osm3DBuildingGenerator.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Osm3DBuildingGenerator.BusinessLogicLayer {

    /// <summary>
    /// This class gets data from the DataLayer and gives it to the PresentationLayer
    /// </summary>
    class BufferClass : DataRequest {
        private OsmData data;

        /// <summary>
        /// Standard constructor
        /// </summary>
        public BufferClass() {
            data = new OsmData();
        }

        /// <summary>
        /// Returns list of building surfaces
        /// </summary>
        /// <returns>list of building surfaces</returns>
        public List<List<ModelUIElement3D>> getBuildingSurfaces() {
            return data.getBuildingSurfaces();
        }

        /// <summary>
        /// Returns list of building walls
        /// </summary>
        /// <returns>list of building walls</returns>
        public List<List<ModelUIElement3D>> getBuildingWalls() {
            return data.getBuildingWalls();
        }

        /// <summary>
        /// Returns list of landuses
        /// </summary>
        /// <returns>list of landuses</returns>
        public List<List<ModelUIElement3D>> getLanduseSurfaces() {
            return data.getLanduseSurfaces();
        }

        /// <summary>
        /// Returns a grass surface
        /// </summary>
        /// <returns>grass surface</returns>
        public ModelUIElement3D getGrassSurface()
        {
            return data.getGrassSurface();
        }

        /// <summary>
        /// Returns a background brush
        /// </summary>
        /// <returns>backgroung brush</returns>
        public ImageBrush getBackgroundBrush()
        {
            return data.getBackgroundBrush();
        }

        /// <summary>
        /// Returns camera
        /// </summary>
        /// <returns>PerspectiveCamera</returns>
        public PerspectiveCamera getCamera() {
            return data.getCamera();
        }
    }
}
