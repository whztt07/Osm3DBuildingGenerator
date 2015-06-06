using Osm3DBuildingGenerator.BusinessLogicLayer;
using Osm3DBuildingGenerator.BusinessLogicLayer.Texture;
using Osm3DBuildingGenerator.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osm3DBuildingGenerator {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private BufferClass buffer;

        /// <summary>
        /// Standard constructor of MainWindow
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            buffer = new BufferClass();

            this.Background = buffer.getBackgroundBrush(); // Sets the background

            getBuildingSurfaces();  // Build the roofs of the buildings
            getBuildingWalls();     // Build the walls of the buildings
            getLanduseSurfaces();   // Build the landuses
            getGrassSurface();
            getCameraAndLight();    // Set up the camera and the light (same direction)

            // Set up the trackball (rotate, zoom and translate the camera)
            // This class is downloaded from: http://3dtools.codeplex.com/SourceControl/latest#3DTools/3DTools/Trackball.cs
            var trackball = new Trackball();
            trackball.EventSource = viewportGrid;
            myViewport3D.Camera.Transform = trackball.Transform;
        }

        /// <summary>
        /// Put the right surfaces of buildings in the right ContainerUIElement3Ds
        /// Because they can be filtered by the checkboxes that way
        /// </summary>
        private void getBuildingSurfaces() {
            List<List<ModelUIElement3D>> surfaces = buffer.getBuildingSurfaces();

            for (int i = 0; i < surfaces.Count; i++) {
                for(int j = 0; j < surfaces[i].Count; j++) {
                    switch(i) {
                        case 0:
                            regular.Children.Add(surfaces[i][j]);
                            break;
                        case 1:
                            church.Children.Add(surfaces[i][j]);
                            break;
                        case 2:
                            apartment.Children.Add(surfaces[i][j]);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Put the right walls of buildings in the right ContainerUIElement3Ds
        /// Because they can be filtered by the checkboxes that way
        /// </summary>
        private void getBuildingWalls() {
            List<List<ModelUIElement3D>> walls = buffer.getBuildingWalls();

            for (int i = 0; i < walls.Count; i++) {
                for (int j = 0; j < walls[i].Count; j++) {
                    switch (i) {
                        case 0:
                            regular.Children.Add(walls[i][j]);
                            break;
                        case 1:
                            church.Children.Add(walls[i][j]);
                            break;
                        case 2:
                            apartment.Children.Add(walls[i][j]);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Put the right landuses in the right ContainerUIElement3Ds
        /// Because they can be filtered by the checkboxes that way
        /// </summary>
        private void getLanduseSurfaces() {
            List<List<ModelUIElement3D>> surfaces = buffer.getLanduseSurfaces();

            for (int i = 0; i < surfaces.Count; i++) {
                for (int j = 0; j < surfaces[i].Count; j++) {
                    switch (i) {
                        case 0:
                            commercial.Children.Add(surfaces[i][j]);
                            break;
                        case 1:
                            construction.Children.Add(surfaces[i][j]);
                            break;
                        case 2:
                            farmland.Children.Add(surfaces[i][j]);
                            break;
                        case 3:
                            forest.Children.Add(surfaces[i][j]);
                            break;
                        case 4:
                            garages.Children.Add(surfaces[i][j]);
                            break;
                        case 5:
                            grass.Children.Add(surfaces[i][j]);
                            break;
                        case 6:
                            industrial.Children.Add(surfaces[i][j]);
                            break;
                        case 7:
                            railway.Children.Add(surfaces[i][j]);
                            break;
                        case 8:
                            residential.Children.Add(surfaces[i][j]);
                            break;
                        case 9:
                            residential.Children.Add(surfaces[i][j]);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Put the grass surface in the belonging container
        /// </summary>
        private void getGrassSurface()
        {
            ModelUIElement3D surface = buffer.getGrassSurface();

            grassSurface.Children.Add(surface);
        }

        /// <summary>
        /// Set up the camera and the light (in the same direction)
        /// </summary>
        public void getCameraAndLight() {
            Model3DGroup group = new Model3DGroup();
            PerspectiveCamera camera = buffer.getCamera();
            
            DirectionalLight light = new DirectionalLight(System.Windows.Media.Color.FromRgb(255, 255, 255), new Vector3D(camera.LookDirection.X, camera.LookDirection.Y, camera.LookDirection.Z * 10));
            group.Children.Add(light);

            myViewport3D.Camera = camera;
            myModelVisual3D.Content = group;
        }

        /// <summary>
        /// Change the visibility of the regular buildings
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxRegular_Click(object sender, RoutedEventArgs e) {
            if (regular.Visibility == System.Windows.Visibility.Hidden) regular.Visibility = System.Windows.Visibility.Visible;
            else regular.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the church buildings
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxChurch_Click(object sender, RoutedEventArgs e) {
            if (church.Visibility == System.Windows.Visibility.Hidden) church.Visibility = System.Windows.Visibility.Visible;
            else church.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the aparment buildings
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxApartment_Click(object sender, RoutedEventArgs e) {
            if (apartment.Visibility == System.Windows.Visibility.Hidden) apartment.Visibility = System.Windows.Visibility.Visible;
            else apartment.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the commerial landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxCommercial_Click(object sender, RoutedEventArgs e) {
            if (commercial.Visibility == System.Windows.Visibility.Hidden) commercial.Visibility = System.Windows.Visibility.Visible;
            else commercial.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the construction landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxConstruction_Click(object sender, RoutedEventArgs e) {
            if (construction.Visibility == System.Windows.Visibility.Hidden) construction.Visibility = System.Windows.Visibility.Visible;
            else construction.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the farmland landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxFarmland_Click(object sender, RoutedEventArgs e) {
            if (farmland.Visibility == System.Windows.Visibility.Hidden) farmland.Visibility = System.Windows.Visibility.Visible;
            else farmland.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the forest landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxForest_Click(object sender, RoutedEventArgs e) {
            if (forest.Visibility == System.Windows.Visibility.Hidden) forest.Visibility = System.Windows.Visibility.Visible;
            else forest.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the garages landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxGarages_Click(object sender, RoutedEventArgs e) {
            if (garages.Visibility == System.Windows.Visibility.Hidden) garages.Visibility = System.Windows.Visibility.Visible;
            else garages.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the grass landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxGrass_Click(object sender, RoutedEventArgs e) {
            if (grass.Visibility == System.Windows.Visibility.Hidden) grass.Visibility = System.Windows.Visibility.Visible;
            else grass.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the industrial landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxIndustrial_Click(object sender, RoutedEventArgs e) {
            if (industrial.Visibility == System.Windows.Visibility.Hidden) industrial.Visibility = System.Windows.Visibility.Visible;
            else industrial.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the railway landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxRailway_Click(object sender, RoutedEventArgs e) {
            if (railway.Visibility == System.Windows.Visibility.Hidden) railway.Visibility = System.Windows.Visibility.Visible;
            else railway.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the residential landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxResidentia_Click(object sender, RoutedEventArgs e) {
            if (residential.Visibility == System.Windows.Visibility.Hidden) residential.Visibility = System.Windows.Visibility.Visible;
            else residential.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Change the visibility of the other landuses
        /// </summary>
        /// <param name="sender">The object that contains information about the sender of this event</param>
        /// <param name="e">The object that contains information about this event</param>
        private void checkBoxOthers_Click(object sender, RoutedEventArgs e) {
            if (others.Visibility == System.Windows.Visibility.Hidden) others.Visibility = System.Windows.Visibility.Visible;
            else others.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
