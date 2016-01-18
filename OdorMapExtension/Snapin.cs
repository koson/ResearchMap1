using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using NUnit.Framework;
namespace ResearchMap1.OdorMapExtension
{
    class Snapin : Extension
    {
        public override void Activate()
        {
            const string MenuKey = "kOdorMap";
            const string SubMenuKey = "kOdorMapSub1";

            App.HeaderControl.Add(new RootItem(MenuKey, Properties.Settings.Default.odormapMenu));
            App.HeaderControl.Add(new SimpleActionItem(MenuKey, Properties.Settings.Default.odormapSub1, method1));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Reproject a Shapefile", ReprojectShapefile));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Adding a Geographic Coordinate System to a Feature Set", AddingGeographicCoordinateSystemToFeatureSet));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "get ProjectionInfo", getProjectionInfo));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Multi Polygon Feature Set", MultypgFSCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Multi Point Feature Set", MultiptFSCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Multi Line Feature Set", MultilsFSCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "test convert coordinate", convertCoordinate));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Add Point", AddPoints));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Move Point", MovePoints));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "creates a line feature", CreatesLineFeature));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "get the value of a single cell in an attribute table", TableSingleCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "loop through a Feature Set's attribute table and get all the values", TableCS));
            App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Create Attributes", CreateAttributes));

            App.HeaderControl.Add(new MenuContainerItem(MenuKey, SubMenuKey, Properties.Settings.Default.odormapSub));
            App.HeaderControl.Add(new SimpleActionItem(MenuKey, SubMenuKey, "odor map 3", OnMenuClickEventHandler));

            base.Activate();
        }

        private void CreateAttributes(object sender, EventArgs e)
        {
            //http://dotspatial.codeplex.com/wikipage?title=CreateAttributes&referringTitle=Desktop_SampleCode
            // define the feature type for this file
            FeatureSet fs = new FeatureSet(FeatureType.Polygon);


            // Add Some Columns
            fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));

            // create a geometry (square polygon)
            List<Coordinate> vertices = new List<Coordinate>();

            vertices.Add(new Coordinate(11219035 , 1542354 ));
            vertices.Add(new Coordinate(11219035, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 0));
            Polygon geom = new Polygon(vertices);

            fs.AddFeature(geom);



            vertices.Clear();
            vertices.Add(new Coordinate(11219035 + 100, 1542354));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 200, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 200, 1542354 + 0));
            geom = new Polygon(vertices);


            // add the geometry to the featureset. 
            IFeature feature = fs.AddFeature(geom);


            // now the resulting features knows what columns it has
            // add values for the columns
            feature.DataRow.BeginEdit();
            feature.DataRow["ID"] = 1;
            feature.DataRow["Text"] = "Hello World";
            feature.DataRow.EndEdit();


            // save the feature set
            fs.SaveAs("d:\\test.shp", true);
        }

        private void TableCS(object sender, EventArgs e)
        {
            //http://dotspatial.codeplex.com/wikipage?title=TableCS&referringTitle=Desktop_SampleCode
            IFeatureSet fs = FeatureSet.Open(@"C:\Temp\province_wgs84_z47.shp");
            fs.FillAttributes();
            DataTable dtOriginal = fs.DataTable;
            for (int row = 0; row < dtOriginal.Rows.Count; row++)
            {
                object[] original = dtOriginal.Rows[row].ItemArray;
            }
        }


        private void TableSingleCS(object sender, EventArgs e)
        {
            //http://dotspatial.codeplex.com/wikipage?title=TableSingleCS&referringTitle=Desktop_SampleCode
            IFeatureSet fs = FeatureSet.Open(@"C:\Temp\province_wgs84_z47.shp");
            fs.FillAttributes();
            DataTable dt = fs.DataTable;
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                object val = dt.Rows[row]["PROV_NAM_T"];
                Console.WriteLine(val.ToString());
            }
        }

        private void CreatesLineFeature(object sender, EventArgs e)
        {
            //http://dotspatial.codeplex.com/wikipage?title=LFCS&referringTitle=Desktop_SampleCode
            //Creates a random number generator
            Random rnd = new Random();
            //creates a new coordiante array
            Coordinate[] c = new Coordinate[36];
            //for loop that will generate 36 random numbers
            for (int i = 0; i < 36; i++)
            {
                c[i] = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);

            }
            //creates a linestring from the coordinate array
            LineString ls = new LineString(c);
            //creates a feature from the linestring
            Feature f = new Feature(ls);
            FeatureSet fs = new FeatureSet(f.FeatureType);
            fs.Features.Add(f);
        }

        //http://dotspatial.codeplex.com/wikipage?title=RandomPoints&referringTitle=Desktop_SampleCode
        IFeatureSet _myPoints = new FeatureSet(FeatureType.Point);
        private void AddPoints(object sender, EventArgs e)
        {
            // Create the featureset if one hasn't been created yet.
            if (_myPoints == null) _myPoints = new FeatureSet(FeatureType.Point);

            // Assume background layers have been added, and get the current map extents.

            double xmin = App.Map.Extent.MinX;
            double xmax = App.Map.Extent.MaxX;
            double ymin = App.Map.Extent.MinY;
            double ymax = App.Map.Extent.MaxY;

            // Randomly generate 10 points that are in the map extent
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                double x = xmin + rnd.NextDouble() * (xmax - xmin);
                double y = ymin + rnd.NextDouble() * (ymax - ymin);
                Coordinate c = new Coordinate(x, y);
                _myPoints.Features.Add(c);
            }

            // Add a layer to the map, and we know it is a point layer so cast it specifically.
            IMapPointLayer pointLayer = App.Map.Layers.Add(_myPoints) as IMapPointLayer;

            // Control what the points look like through a symbolizer (or pointLayer.Symbology for categories)
            if (pointLayer != null)
            {
                pointLayer.LegendText = "MovingPoints";
                pointLayer.Symbolizer = new PointSymbolizer(Color.Blue, DotSpatial.Symbology.PointShape.Ellipse, 7);
            }
        }
        private void MovePoints(object sender, EventArgs e)
        {
            Random rnd = new Random();


            foreach (IFeature feature in _myPoints.Features)
            {
                // Coordinates can be updated geographically like
                // feature.Coordinates[0].X += (rnd.NextDouble() - .5);
                // feature.Coordinates[0].Y += (rnd.NextDouble() - .5);

                // Or controled in pixels with the help of the map
                System.Drawing.Point pixelLocation = App.Map.ProjToPixel(feature.Coordinates[0]);

                // Control movement in terms of pixels
                int dx = Convert.ToInt32((rnd.NextDouble() - .5) * 50); // shift left or right 5 pixels
                int dy = Convert.ToInt32((rnd.NextDouble() - .5) * 50); // shift up or down 5 pixels
                pixelLocation.X = pixelLocation.X + dx;
                pixelLocation.Y = pixelLocation.Y + dy;

                // Convert the pixel motions back to geographic motions.
                Coordinate c = App.Map.PixelToProj(pixelLocation);
                feature.Coordinates[0] = c;
            }

            // Refresh the cached representation because we moved points around.
            App.Map.MapFrame.Invalidate();
            App.Map.Invalidate();
        }
        private void convertCoordinate(object sender, EventArgs e)
        {
            double[] xy = new double[] { 100.78518031542525, 13.722204938478965 };
            // z values if any.  Typically this is just 0.
            double[] z = new double[] { 0 };
            // Source projection information.
            ProjectionInfo source = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator;
            // Destination projection information.
            ProjectionInfo dest = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;

//            Console.WriteLine("Coordinate (WGS1984) = (" + xy[0].ToString() + ", " + xy[1].ToString() + ").");
            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, dest, source, 0, 1);
//            Console.WriteLine("Coordinate (World.WebMercator) = (" + xy[0].ToString() + ", " + xy[1].ToString() + ").");
        }

        private void MultilsFSCS(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);
            for (int ii = 0; ii < 40; ii++)
            {
                Coordinate[] coord = new Coordinate[36];
                for (int i = 0; i < 36; i++)
                {
                    coord[i] = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
                }
                LineString ls = new LineString(coord);
                f = new Feature(ls);
                fs.Features.Add(f);
            }
            fs.SaveAs("C:\\Temp\\test.shp", true);
        }

        private void MultiptFSCS(object sender, EventArgs e)
        {
            Coordinate[] c = new Coordinate[50];
            Random rnd = new Random();
            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);
            for (int i = 0; i < 50; i++)
            {
                c[i] = new Coordinate((rnd.Next(0, 50) + 360) - 90, (rnd.NextDouble() * 360) - 180);
                fs.Features.Add(c[i]);
            }
            fs.SaveAs("C:\\Temp\\MultiptFSCS.shp", true);
        }

        private void MultypgFSCS(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Polygon[] pg = new Polygon[100];
            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);
            for (int i = 0; i < 100; i++)
            {
                Coordinate center = new Coordinate((rnd.Next(50) * 360) - 180, (rnd.Next(60) * 180) - 90);
                Coordinate[] coord = new Coordinate[50];
                for (int ii = 0; ii < 50; ii++)
                {
                    coord[ii] = new Coordinate(center.X + Math.Cos((ii * 10) * Math.PI / 10), center.Y + (ii * 10) * Math.PI / 10);
                }
                coord[49] = new Coordinate(coord[0].X, coord[0].Y);
                pg[i] = new Polygon(coord);
                fs.Features.Add(pg[i]);
            }
            fs.SaveAs("C:\\Temp\\test.shp", true);
        }




        private void getProjectionInfo(object sender, EventArgs e)
        {
            var infoBuiltIn = KnownCoordinateSystems.Geographic.World.WGS1984;

            const string esri = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            var infoFromEsri = ProjectionInfo.FromEsriString(esri);

            var expected = infoFromEsri.ToProj4String();
            var actual = infoBuiltIn.ToProj4String();

            Assert.AreEqual(expected, actual);
        }

        private void AddingGeographicCoordinateSystemToFeatureSet(object sender, EventArgs e)
        {

        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        private void OnMenuClickEventHandler(object sender, EventArgs e)
        {
            MessageBox.Show("Clicked " + ((SimpleActionItem)sender).Caption);
        }




        private void ReprojectShapefile(object sender, EventArgs e)
        {

            //Sets up a array to contain the x and y coordinates
            double[] xy = new double[2];
            xy[0] = 100;
            xy[1] = 13;
            //An array for the z coordinate
            double[] z = new double[1];
            z[0] = 1;
            //Defines the starting coordiante system


            ProjectionInfo pStart = KnownCoordinateSystems.Geographic.World.WGS1984;
            //Defines the ending coordiante system
            //            ProjectionInfo pEnd = KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
            ProjectionInfo pEnd = KnownCoordinateSystems.Projected.WorldSpheroid.Mercatorsphere;
            //Calls the reproject function that will transform the input location to the output locaiton
            Reproject.ReprojectPoints(xy, z, pStart, pEnd, 0, 1);
            MessageBox.Show("The points have been reprojected.");
        }

        private void method1(object sender, EventArgs e)
        {
            //MessageBox.Show("Hello???", "Method 1");
            //// interleaved x and y values, so like x1, y1, x2, y2 etc.
            //double[] xy = new double[] { 11219088, 1542359 };
            //// z values if any.  Typically this is just 0.
            //double[] z = new double[] { 0 };
            //// Source projection information.
            //ProjectionInfo source = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone47N;
            //// Destination projection information.
            //ProjectionInfo dest = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
            //// Call the projection utility.
            //DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, source, dest, 0, 1);


            //ProjectionInfo pStart = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone47N;
            //ProjectionInfo pEnd = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;


            //double lat = 8.654;
            //double lon = 38.123;

            //double[] xy = new double[2] { lat, lon };
            //double[] z = new double[1] { 1234.5 };

            //Reproject.ReprojectPoints(xy, z, pStart, pEnd, 0, 1);

            //Console.WriteLine(String.Format("New Lat/Lon = {0}/{1}", xy[0], xy[1]));

            var coor = App.Map.PixelToProj(new System.Drawing.Point(11219088, 1542359));

        }
    }
}
