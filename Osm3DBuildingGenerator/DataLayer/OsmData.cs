using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Osm3DBuildingGenerator.BusinessLogicLayer;
using System.IO;
using OsmSharp.Osm.Xml.Streams;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using Osm3DBuildingGenerator.BusinessLogicLayer.Texture;
using System.Windows;

namespace Osm3DBuildingGenerator.DataLayer {

    /// <summary>
    /// This class contains, gets and gives through all the data that's needed to show the osm buildings and landuses
    /// </summary>
    class OsmData : DataRequest {
        // Objects and variable that are used to get the data
        private const string OSMFILE = @"gent.osm";
        FileInfo fileInfo;
        XmlOsmStreamSource xmlSource;

        // Data that is used to draw the buildings and landuses
        private const double ZOOM = 10;
        private const double STARTANGLE = 10; // in degrees
        private Map map;
        private Dictionary<long, OsmSharp.Osm.Node> nodes;
        private List<Building> buildings;
        private List<Landuse> landuses;

        // The used brushes to draw buildings or landuses of a certain kind
        private readonly SolidColorBrush SURFACEBRUSH;
        private readonly BitmapImage bitmapImage;
        private readonly ImageBrush WALLBRUSH;
        private readonly ImageBrush GRASSBRUSH;
        private readonly SolidColorBrush LANDUSECOMMERCIALBRUSH;
        private readonly SolidColorBrush LANDUSECOSTRUCIONBRUSH;
        private readonly SolidColorBrush LANDUSEFARMLANDBRUSH;
        private readonly SolidColorBrush LANDUSEFORESTBRUSH;
        private readonly SolidColorBrush LANDUSEGARAGESBRUSH;
        private readonly SolidColorBrush LANDUSEGRASSBRUSH;
        private readonly SolidColorBrush LANDUSEINDUSTRIALBRUSH;
        private readonly SolidColorBrush LANDUSERAILWAYBRUSH;
        private readonly SolidColorBrush LANDUSERESIDENTIALBRUSH;
        private readonly SolidColorBrush LANDUSEDEFAULTBRUSH;

        /// <summary>
        /// Standard constructor of OsmData
        /// </summary>
        public OsmData() {    
            fileInfo = new FileInfo(OSMFILE);
            xmlSource = new XmlOsmStreamSource(fileInfo.OpenRead());

            nodes = new Dictionary<long, OsmSharp.Osm.Node>();
            buildings = new List<Building>();
            landuses = new List<Landuse>();

            /* Declare the colors of the brushes*/

            // Create Wallbrush
            WallTexture texture = new WallTexture(100, 1);
            Bitmap bitmap = texture.DrawTexture();

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            Rect rect = new Rect(0, 0, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            WALLBRUSH = new ImageBrush(bitmapImage);
            WALLBRUSH.Viewport = rect;
            WALLBRUSH.ViewportUnits = BrushMappingMode.Absolute;
            WALLBRUSH.Stretch = Stretch.None;
            WALLBRUSH.AlignmentX = AlignmentX.Left;
            WALLBRUSH.AlignmentY = AlignmentY.Top;
            WALLBRUSH.TileMode = TileMode.Tile;


            // Create GrassBrush
            DiamondSquare textureGrass = new DiamondSquare(1, 256, 256);
            Bitmap bitmapGrass = textureGrass.getPoints("green");

            using (MemoryStream memory = new MemoryStream())
            {
                bitmapGrass.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            Rect rectGrass = new Rect(0, 0, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            GRASSBRUSH = new ImageBrush(bitmapImage);
            GRASSBRUSH.Viewport = rectGrass;
            GRASSBRUSH.ViewportUnits = BrushMappingMode.Absolute;
            GRASSBRUSH.Stretch = Stretch.None;
            GRASSBRUSH.AlignmentX = AlignmentX.Left;
            GRASSBRUSH.AlignmentY = AlignmentY.Top;
            GRASSBRUSH.TileMode = TileMode.Tile;

            // Define rest of brushes
            SURFACEBRUSH = new SolidColorBrush(Colors.Black);
            LANDUSECOMMERCIALBRUSH = new SolidColorBrush(Colors.LightPink);
            LANDUSECOSTRUCIONBRUSH = new SolidColorBrush(Colors.DarkOliveGreen);
            LANDUSEFARMLANDBRUSH = new SolidColorBrush(Colors.LightYellow);
            LANDUSEFORESTBRUSH = new SolidColorBrush(Colors.LightGreen);
            LANDUSEGARAGESBRUSH = new SolidColorBrush(Colors.Gray);
            LANDUSEGRASSBRUSH = new SolidColorBrush(Colors.LawnGreen);
            LANDUSEINDUSTRIALBRUSH = new SolidColorBrush(Colors.MediumPurple);
            LANDUSERAILWAYBRUSH = new SolidColorBrush(Colors.Black);
            LANDUSERESIDENTIALBRUSH = new SolidColorBrush(Colors.WhiteSmoke);
            LANDUSEDEFAULTBRUSH = new SolidColorBrush(Colors.Orange);

            getBounds();                // declares the object map
            getNodes();                 // fills the dictonary nodes
            getBuildingsAndLanduses();  // fills the lists buildings and landuses
        }

        /// <summary>
        /// This method gets sets the minimum and maximum latitude/longitude into the map object
        /// It doesn't get it by the xmlSource but by a simple fileReader
        /// </summary>
        private void getBounds() {
            foreach (string line in File.ReadLines(OSMFILE)) {
                if (line.Contains("bounds")) {
                    string[] waarden = line.Split('"');
                    float minLat = float.Parse(waarden[1], System.Globalization.CultureInfo.InvariantCulture);
                    float minLong = float.Parse(waarden[3], System.Globalization.CultureInfo.InvariantCulture);
                    float maxLat = float.Parse(waarden[5], System.Globalization.CultureInfo.InvariantCulture);
                    float maxLong = float.Parse(waarden[7], System.Globalization.CultureInfo.InvariantCulture);

                    map = new Map(minLat, minLong, maxLat, maxLong);
                    return; // Jump out of the method
                }
            }
            throw new ApplicationException("Invalid osm file"); // If the data isn't found an exeption is thrown
        }

        /// <summary>
        /// Sets all the nodes into the global variable nodes
        /// </summary>
        private void getNodes() {
            xmlSource.Initialize();

            while (xmlSource.MoveNextNode()) {
                OsmSharp.Osm.Node node = (OsmSharp.Osm.Node)xmlSource.Current();
                if ((node != null) && (node.Id.HasValue && (node.Visible.HasValue) && (bool)(node.Visible))) {
                    nodes.Add((long)node.Id, node);
                }
            }
        }

        /// <summary>
        /// Sets all the info about the buildings and landuses into their lists
        /// </summary>
        private void getBuildingsAndLanduses() {
            FileInfo fileInfo = new FileInfo(OSMFILE);
            XmlOsmStreamSource xmlSource = new XmlOsmStreamSource(fileInfo.OpenRead());
            xmlSource.Initialize();

            while (xmlSource.MoveNextWay()) {
                OsmSharp.Osm.Way way = (OsmSharp.Osm.Way)xmlSource.Current();
                if ((way != null) && (way.Visible.HasValue) && (bool)(way.Visible) && (way.Tags != null)) {

                    // Info about buildings
                    if (way.Tags.ContainsKey("building")) {
                        List<long> buildingNodes = new List<long>();
                        int height = 10; // Standard height is 10
                        string amenity = "";

                        // Check amenity and if it is a church
                        if (way.Tags.ContainsKey("amenity")) {
                            amenity = way.Tags["amenity"];

                            if (amenity == "place_of_worship") height = 25;
                        }

                        // Check height
                        if (way.Tags.ContainsKey("height")) {
                            height = (int)(float.Parse(way.Tags["height"], System.Globalization.CultureInfo.InvariantCulture));
                        } else if (way.Tags.ContainsKey("building:levels")) {
                            string[] verdiepen = way.Tags["building:levels"].Split('-');
                            height = 3 * Convert.ToInt32(verdiepen[verdiepen.Length - 1]);
                            amenity = "apartment";
                        }

                        foreach (long id in way.Nodes) buildingNodes.Add(id);

                        buildings.Add(new Building(buildingNodes, height, amenity, ZOOM));
                    }

                    // Info about landuses
                    if (way.Tags.ContainsKey("landuse")) {
                        List<long> landuseNodes = new List<long>();
                        foreach (long id in way.Nodes) landuseNodes.Add(id);

                        landuses.Add(new Landuse(landuseNodes, 0, way.Tags["landuse"], ZOOM));
                    }
                }
            }

            xmlSource.Dispose();
        }

        /// <summary>
        /// Returns all the building roofs sorted by 3 types (regular -, church - and apartment buildings)
        /// </summary>
        /// <returns>A list of types and within all the building surfaces of that type</returns>
        public List<List<ModelUIElement3D>> getBuildingSurfaces() {
            List<List<ModelUIElement3D>> surfaces = new List<List<ModelUIElement3D>>();

            List<ModelUIElement3D> regular = new List<ModelUIElement3D>();
            List<ModelUIElement3D> church = new List<ModelUIElement3D>();
            List<ModelUIElement3D> apartment = new List<ModelUIElement3D>();

            for(int i = 0; i < buildings.Count; i++) {
                switch (buildings[i].Kind) {
                    case "apartment":
                        try {
                            apartment.Add(buildings[i].get3DSurface(nodes, map, SURFACEBRUSH));
                        } catch (Exception) {
                            buildings.RemoveAt(i);
                        }
                        break;
                    case "place_of_worship":
                        try {
                            church.Add(buildings[i].get3DSurface(nodes, map, SURFACEBRUSH));
                        } catch (Exception) {
                            buildings.RemoveAt(i);
                        }
                        break;
                    default:
                        try {
                            regular.Add(buildings[i].get3DSurface(nodes, map, SURFACEBRUSH));
                        } catch (Exception) {
                            buildings.RemoveAt(i);
                        }
                        break;
                }
            }

            surfaces.Add(regular);
            surfaces.Add(church);
            surfaces.Add(apartment);

            return surfaces;
        }

        /// <summary>
        /// Returns all the building walls sorted by 3 types (regular -, church - and apartment buildings)
        /// </summary>
        /// <returns>A list of types and within all the building walls of that type</returns>
        public List<List<ModelUIElement3D>> getBuildingWalls() {
            List<List<ModelUIElement3D>> walls = new List<List<ModelUIElement3D>>();

            List<ModelUIElement3D> regular = new List<ModelUIElement3D>();
            List<ModelUIElement3D> church = new List<ModelUIElement3D>();
            List<ModelUIElement3D> apartment = new List<ModelUIElement3D>();

            foreach (Building building in buildings) {
                switch (building.Kind) {
                    case "apartment":
                        apartment.Add(building.get3DWalls(nodes, map, WALLBRUSH));
                        break;
                    case "place_of_worship":
                        church.Add(building.get3DWalls(nodes, map, WALLBRUSH));
                        break;
                    default:
                        regular.Add(building.get3DWalls(nodes, map, WALLBRUSH));
                        break;
                }
            }

            walls.Add(regular);
            walls.Add(church);
            walls.Add(apartment);

            return walls;
        }

        /// <summary>
        /// Returns all the landuses sorted by 10 types
        /// </summary>
        /// <returns>A list of types and within all the landuses of that type</returns>
        public List<List<ModelUIElement3D>> getLanduseSurfaces() {
            List<List<ModelUIElement3D>> surfaces = new List<List<ModelUIElement3D>>();

            List<ModelUIElement3D> commercial = new List<ModelUIElement3D>();
            List<ModelUIElement3D> construction = new List<ModelUIElement3D>();
            List<ModelUIElement3D> farmland = new List<ModelUIElement3D>();
            List<ModelUIElement3D> forest = new List<ModelUIElement3D>();
            List<ModelUIElement3D> garages = new List<ModelUIElement3D>();
            List<ModelUIElement3D> grass = new List<ModelUIElement3D>();
            List<ModelUIElement3D> industrial = new List<ModelUIElement3D>();
            List<ModelUIElement3D> railway = new List<ModelUIElement3D>();
            List<ModelUIElement3D> residential = new List<ModelUIElement3D>();
            List<ModelUIElement3D> other = new List<ModelUIElement3D>();

            foreach (Landuse landuse in landuses) {
                switch (landuse.Kind) {
                    case "commercial":
                        try {
                        commercial.Add(landuse.get3DSurface(nodes, map, LANDUSECOMMERCIALBRUSH));
                        } catch (Exception) { }
                        break;
                    case "construction":
                        try {
                        construction.Add(landuse.get3DSurface(nodes, map, LANDUSECOSTRUCIONBRUSH));
                        } catch (Exception) { }
                        break;
                    case "farmland":
                        try {
                        farmland.Add(landuse.get3DSurface(nodes, map, LANDUSEFARMLANDBRUSH));
                        } catch (Exception) { }
                        break;
                    case "forest":
                        try {
                        forest.Add(landuse.get3DSurface(nodes, map, LANDUSEFORESTBRUSH));
                        } catch (Exception) { }
                        break;
                    case "garages":
                        try {
                        garages.Add(landuse.get3DSurface(nodes, map, LANDUSEGARAGESBRUSH));
                        } catch (Exception) { }
                        break;
                    case "grass":
                        try {
                        grass.Add(landuse.get3DSurface(nodes, map, LANDUSEGRASSBRUSH));
                        } catch (Exception) { }
                        break;
                    case "greenfield":
                        try {
                        grass.Add(landuse.get3DSurface(nodes, map, LANDUSEGRASSBRUSH));
                        } catch (Exception) { }
                        break;
                    case "industrial":
                        try {
                        industrial.Add(landuse.get3DSurface(nodes, map, LANDUSEINDUSTRIALBRUSH));
                        } catch (Exception) { }
                        break;
                    case "railway":
                        try {
                        railway.Add(landuse.get3DSurface(nodes, map, LANDUSERAILWAYBRUSH));
                        } catch (Exception) { }
                        break;
                    case "residential":
                        try {
                        residential.Add(landuse.get3DSurface(nodes, map, LANDUSERESIDENTIALBRUSH));
                        } catch (Exception) { }
                        break;
                    case "village_green":
                        try {
                        grass.Add(landuse.get3DSurface(nodes, map, LANDUSEGRASSBRUSH));
                        } catch (Exception) { }
                        break;
                    default:
                        try {
                        other.Add(landuse.get3DSurface(nodes, map, LANDUSEDEFAULTBRUSH));
                        } catch (Exception) { }
                        break;
                }            
            }

            surfaces.Add(commercial);
            surfaces.Add(construction);
            surfaces.Add(farmland);
            surfaces.Add(forest);
            surfaces.Add(garages);
            surfaces.Add(grass);
            surfaces.Add(industrial);
            surfaces.Add(railway);
            surfaces.Add(residential);
            surfaces.Add(other); 

            return surfaces;
        }

        /// <summary>
        /// Returns the grass surface
        /// </summary>
        /// <returns>ModelUIElement3D of the grass surface</returns>
        public ModelUIElement3D getGrassSurface()
        {
            // Surrounding tags of the mesh
            ModelUIElement3D model = new ModelUIElement3D();
            GeometryModel3D geometryModel = new GeometryModel3D();

            // Mesh and his his properties
            MeshGeometry3D mesh = new MeshGeometry3D();
            DiffuseMaterial material = new DiffuseMaterial((System.Windows.Media.Brush)GRASSBRUSH);
            Point3DCollection positions = new Point3DCollection();
            PointCollection texturePoints = new PointCollection();
            Int32Collection indices = new Int32Collection();

            // Add Points of surface and with add points of surface with height 0
            positions.Add(new Point3D(8000, 8000, -1));
            positions.Add(new Point3D(-8000, 8000, -1));
            positions.Add(new Point3D(8000, -8000, -1));
            positions.Add(new Point3D(-8000, -8000, -1));

            // Add indices to the collection
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);
            indices.Add(1);
            indices.Add(2);
            indices.Add(3);

            texturePoints.Add(new System.Windows.Point(0, 0));
            texturePoints.Add(new System.Windows.Point(2000, 0));
            texturePoints.Add(new System.Windows.Point(0, 2000));
            texturePoints.Add(new System.Windows.Point(2000, 2000));

            // Add these collections to the mesh
            mesh.Positions = positions;
            mesh.TriangleIndices = indices;
            mesh.TextureCoordinates = texturePoints;

            // Set the color of front and back of the triangle
            geometryModel.Material = material;
            geometryModel.BackMaterial = material;

            // Add the mesh to the model
            geometryModel.Geometry = mesh;
            model.Model = geometryModel;

            return model;
        }

        /// <summary>
        /// Returns the brush for the background
        /// </summary>
        /// <returns>ImageBrush for the background</returns>
        public ImageBrush getBackgroundBrush()
        {
            DiamondSquare background = new DiamondSquare(0.7, 256, 256);
            Bitmap bitmap = background.getPoints("blue");

            ImageBrush brush = new ImageBrush();
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            brush.ImageSource = bitmapImage;

            return brush;
        }

        /// <summary>
        /// Returns a camera that is 1.5 times the size of the village/city away from the center of the city
        /// </summary>
        /// <returns></returns>
        public PerspectiveCamera getCamera() {
            PerspectiveCamera myCamera = new PerspectiveCamera();

            // Get the size of the village/city and it's max/min x/y-position
            int minX = (int)((128 / Math.PI) * Math.Pow(2, ZOOM) * (map.MinLong + Math.PI));
            int minY = (int)((128 / Math.PI) * Math.Pow(2, ZOOM) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + map.MinLat / 2))));
            int maxX = (int)((128 / Math.PI) * Math.Pow(2, ZOOM) * (map.MaxLong + Math.PI));
            int maxY = (int)((128 / Math.PI) * Math.Pow(2, ZOOM) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + map.MaxLat / 2))));

            // Get the X and Y position of the camera
            int positionX =  (int)(-1.5 * (maxX - minX));
            int positionY = (int)(-1.5 * (maxY - minY));

            // Calculate the height of the Z position by the given angle and the laws of Pythagoras
            double angle = STARTANGLE / 180 * Math.PI;
            double adjecentSide = Math.Sqrt(Math.Pow(1.5 * (maxX - minX), 2) + Math.Pow(1.5 * (maxY - minY), 2));
            double oppositeSide = adjecentSide * Math.Sin(angle); //aanliggende zijde = hoogte

            // Put all the data into the myCamera object
            myCamera.Position = new Point3D(positionX, positionY, oppositeSide); 
            myCamera.LookDirection = new Vector3D(1.5 * (maxX - minX), 1.5 * (maxY - minY), -oppositeSide);
            myCamera.UpDirection = new Vector3D(0, 0, 1);
            myCamera.FieldOfView = 60;

            return myCamera;
        }
            
    }
}