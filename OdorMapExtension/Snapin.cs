﻿using System;
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
        IFeatureSet _myPoints = new FeatureSet(FeatureType.Point);
        IFeatureSet _myLines = new FeatureSet(FeatureType.Line);
        //IFeatureSet _myLines = new FeatureSet(FeatureType.Line);
        public override void Activate()
        {
            const string MenuKey = "kOdorMap";
            //const string SubMenuKey = "kOdorMapSub1";

            App.HeaderControl.Add(new RootItem(MenuKey, Properties.Settings.Default.odormapMenu));
            #region comment
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, Properties.Settings.Default.odormapSub1, method1));
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
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Draw Line", DrawLine));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Draw Line 2", DrawLine2));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Coord Click", btnCoord_Click));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "calculating the length of line string", linestringCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "create new polygon and calculating the area", PolygonCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "create multi points", MpsCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "create a multilinestring", MlsCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "create a multipolygon", MpgCS));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "generate polygon that contains a hole ", HolesCS));
            //App.HeaderControl.Add(new MenuContainerItem(MenuKey, SubMenuKey, Properties.Settings.Default.odormapSub));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, SubMenuKey, "odor map 3", OnMenuClickEventHandler));

            #endregion
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Create Attributes", CreateAttributes));

            App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Create base grid", createBaseGrid));
            //App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Apply Color Scheme", btnApplyColorScheme_Click));
            App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Add Line", AddLine));
            App.HeaderControl.Add(new SimpleActionItem(MenuKey, "Move Line", MoveLine));




            base.Activate();
        }

        private void MoveLine(object sender, EventArgs e)
        {
            Random rnd = new Random();


            foreach (IFeature feature in _myLines.Features)
            {
                // Coordinates can be updated geographically like
                // feature.Coordinates[0].X += (rnd.NextDouble() - .5);
                // feature.Coordinates[0].Y += (rnd.NextDouble() - .5);

                //for (int i = 0; i < 10; i++)
                //{
                    // Or controled in pixels with the help of the map
                    System.Drawing.Point pixelLocation =App.Map.ProjToPixel(feature.Coordinates[0]);

                    // Control movement in terms of pixels
                    int dx = Convert.ToInt32((rnd.NextDouble() - .5) * 50); // shift left or right 5 pixels
                    int dy = Convert.ToInt32((rnd.NextDouble() - .5) * 50); // shift up or down 5 pixels
                    pixelLocation.X = pixelLocation.X + dx;
                    pixelLocation.Y = pixelLocation.Y + dy;

                    // Convert the pixel motions back to geographic motions.
                    //feature.Coordinates[i] = App.Map.PixelToProj(pixelLocation);
                    feature.Coordinates[0] = App.Map.PixelToProj(pixelLocation);
                //}
            }

            // Refresh the cached representation because we moved points around.
            App.Map.MapFrame.Invalidate();
            App.Map.Invalidate();
        }

        private void AddLine(object sender, EventArgs e)
        {
            // Create the featureset if one hasn't been created yet.
            if (_myLines == null) _myLines= new FeatureSet(FeatureType.Line);
            _myLines.Projection = App.Map.Projection;

            // Assume background layers have been added, and get the current map extents.

            double xmin = App.Map.ViewExtents.MinX;
            double xmax = App.Map.ViewExtents.MaxX;
            double ymin = App.Map.ViewExtents.MinY;
            double ymax = App.Map.ViewExtents.MaxY;

            // Randomly generate 10 points that are in the map extent
            Coordinate[] coords = new Coordinate[10];
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                double x = xmin + rnd.NextDouble() * (xmax - xmin);
                double y = ymin + rnd.NextDouble() * (ymax - ymin);
                coords[i] = new Coordinate(x, y);

            }

            _myLines.Features.Add(new LineString(coords));

            // Add a layer to the map, and we know it is a point layer so cast it specifically.
            IMapLineLayer LineLayer = App.Map.Layers.Add(_myLines) as IMapLineLayer;

            // Control what the points look like through a symbolizer (or pointLayer.Symbology for categories)
            if (LineLayer != null)
            {
                LineLayer.LegendText = "MovingLines";
                LineLayer.Symbolizer = new LineSymbolizer(Color.Blue, 3);
            }
        }

        private void btnApplyColorScheme_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            Shapefile sf = new Shapefile();
            openFileDialog1.Filter = "Shapefiles|*.shp";



            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sf = Shapefile.OpenFile(openFileDialog1.FileName, null);


                App.Map.AddLayer(sf.Filename);
                App.Map.Refresh();
            }
        }

        private void createBaseGrid(object sender, EventArgs e)
        {
            Coordinate[] coord = new Coordinate[4];
            double startx = 11252200;
            double starty = 1418500;
            const int width = 100;

            FeatureSet fs = new FeatureSet();
            IFeature feature;
            // Add Some Columns
            fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Value", typeof(double)));

            Random rnd = new Random();
            for (int i = 0; i < 110; i++)
            {
                for (int j = 0; j < 110; j++)
                {
                    coord[0] = new Coordinate((i * width) + startx, (j * width) + starty);
                    coord[1] = new Coordinate((i * width) + startx, (j * width) + starty + width);
                    coord[2] = new Coordinate((i * width) + startx + width, (j * width) + starty + width);
                    coord[3] = new Coordinate((i * width) + startx + width, (j * width) + starty + 0);
                    Polygon pg = new Polygon(coord);
                    feature = fs.AddFeature(pg);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["ID"] = j + i * 50;
                    feature.DataRow["Text"] = "Hello World" + (j + i * 50).ToString();
                    feature.DataRow["Value"] = (rnd.NextDouble() * 360) ;
                    feature.DataRow.EndEdit();
                }
            }
            // Add a layer to the map, and we know it is a point layer so cast it specifically.
            IMapPolygonLayer polygonLayer = App.Map.Layers.Add(fs) as IMapPolygonLayer;
            // Control what the points look like through a symbolizer (or pointLayer.Symbology for categories)
            if (polygonLayer != null)
            {
                polygonLayer.LegendText = "grid point";
                //polygonLayer.Symbolizer = new PointSymbolizer(Color.Blue, DotSpatial. );
            }
        }

        //http://dotspatial.codeplex.com/wikipage?title=RandomPoints&referringTitle=Desktop_SampleCode
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


        private void MpgCS(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Polygon[] pg = new Polygon[50];
            for (int i = 0; i < 50; i++)
            {
                Coordinate center = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
                Coordinate[] coord = new Coordinate[36];
                for (int ii = 0; ii < 36; ii++)
                {
                    coord[ii] = new Coordinate(center.X + Math.Cos((ii * 10) * Math.PI / 10), center.Y + (ii * 10) * Math.PI / 10);
                }
                coord[35] = new Coordinate(coord[0].X, coord[0].Y);
                pg[i] = new Polygon(coord);
            }
            MultiPolygon mpg = new MultiPolygon(pg);

            FeatureSet fs = new FeatureSet(mpg.FeatureType);
            fs.Features.Add(mpg);
            fs.SaveAs("C:\\Temp\\mpg.shp", true);
        }

        private void MlsCS(object sender, EventArgs e)
        {
            Random rnd = new Random();
            MultiLineString Mls = new MultiLineString();
            LineString[] ls = new LineString[40];
            for (int ii = 0; ii < 40; ii++)
            {
                Coordinate[] coord = new Coordinate[36];
                for (int i = 0; i < 36; i++)
                {
                    coord[i] = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
                }
                ls[ii] = new LineString(coord);
            }
            Mls = new MultiLineString(ls);

            FeatureSet fs = new FeatureSet(Mls.FeatureType);
            fs.Features.Add(Mls);
            fs.SaveAs("C:\\Temp\\Mls.shp", true);
        }

        private void HolesCS(object sender, EventArgs e)
        {
            //Defines a new coordinate array
            Coordinate[] coords = new Coordinate[20];
            //Defines a new random number generator
            Random rnd = new Random();
            //defines a randomly generated center for teh polygon
            Coordinate center = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
            for (int i = 0; i < 19; i++)
            {
                //generates random coordinates and adds those coordinates to the array
                coords[i] = new Coordinate(center.X + Math.Cos((i * 10) * Math.PI / 10), center.Y + (i * 10) * Math.PI / 10);
            }
            //sets the last coordinate equal to the first, this 'closes' the polygon
            coords[19] = new Coordinate(coords[0].X, coords[0].Y);
            //defines a new LingRing from the coordinates
            LinearRing Ring = new LinearRing(coords);
            //Repeates the process, but generates a LinearRing with a smaller area, this will be the hole in the polgyon
            Coordinate[] coordshole = new Coordinate[20];
            for (int i = 0; i < 20; i++)
            {
                coordshole[i] = new Coordinate(center.X + Math.Cos((i * 10) * Math.PI / 20), center.Y + (i * 10) * Math.PI / 20);
            }
            coordshole[19] = new Coordinate(coordshole[0].X, coordshole[0].Y);
            LinearRing Hole = new LinearRing(coordshole);
            //This steps addes the hole LinerRing to a ILinearRing Array
            //A Polgyon can contain multiple holes, thus a Array of Hole is required
            ILinearRing[] Holes = new ILinearRing[1];
            Holes[0] = Hole;
            //This passes the Ring, the polygon shell, and the Holes Array, the holes
            Polygon pg = new Polygon(Ring, Holes);

            //Feature f = new Feature();
            FeatureSet fs = new FeatureSet(pg.FeatureType);
            //f = new Feature(pg);
            fs.Features.Add(pg);
            fs.SaveAs("C:\\Temp\\hole.shp", true);

        }

        private void MpsCS(object sender, EventArgs e)
        {
            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);
            Coordinate[] c = new Coordinate[36];
            Random rnd = new Random();
            for (int i = 0; i < 36; i++)
            {
                c[i] = new Coordinate((rnd.NextDouble() + 360) - 180, (rnd.NextDouble() * 180) - 90);
            }
            MultiPoint Mps = new MultiPoint(c);

            f = new Feature(Mps);
            fs.Features.Add(f);
            fs.SaveAs("C:\\Temp\\mps.shp", true);
        }

        private void PolygonCS(object sender, EventArgs e)
        {
            //creates a new coordinate array
            Coordinate[] coords = new Coordinate[10];
            //creates a random point variable
            Random rnd = new Random();
            //Creates the center coordiante for the new polygon
            Coordinate center = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
            //a for loop that generates a new random X and Y value and feeds those values into the coordinate array
            for (int i = 0; i < 10; i++)
            {
                coords[i] = new Coordinate(center.X + Math.Cos((i * 2) * Math.PI / 18), center.Y + (i * 2) * Math.PI / 18);
            }
            //creates a new polygon from the coordinate array
            coords[9] = new Coordinate(coords[0].X, coords[0].Y);
            Polygon pg = new Polygon(coords);
            //new variable for the area of the polgyon
            Double area;
            area = pg.Area;
            //displays the area of the polygon
            MessageBox.Show("The Area of the polygon is: " + area);
        }

        private void linestringCS(object sender, EventArgs e)
        {
            //creates a new coordinate array
            Coordinate[] coords = new Coordinate[36];
            //creates a random point variable
            Random rnd = new Random();
            //a for loop that generates a new random X and Y value and feeds those values into the coordinate array
            for (int i = 0; i < 36; i++)
            {
                coords[i] = new Coordinate((rnd.NextDouble() * 360) - 180, (rnd.NextDouble() * 180) - 90);
            }
            //creates a new linstring from the array of coordinates
            LineString ls = new LineString(coords);
            //new variable for the length of the linstring
            Double length;
            length = ls.Length;
            //Displays the length of the linstring
            MessageBox.Show("The length of the linstring is: " + length);
        }

        private void btnCoord_Click(object sender, EventArgs e)
        {
            //creates a new coordinate 
            Coordinate c = new Coordinate(2.4, 2.4);
            //passes the coordinate to a new point
            DotSpatial.Topology.Point p = new DotSpatial.Topology.Point(c);
            //displayes the new point's x and y coordiantes
            MessageBox.Show("Point p is: x= " + p.X + " & y= " + p.Y);
        }

        private void DrawLine2(object sender, EventArgs e)
        {


        }

        void DrawLine(double x1, double y1, double x2, double y2, int pixelWidth, Color color)
        {
            // TODO write function draw line

        }

        private void DrawLine(object sender, EventArgs e)
        {
            double xmin = App.Map.Extent.MinX;
            double xmax = App.Map.Extent.MaxX;
            double ymin = App.Map.Extent.MinY;
            double ymax = App.Map.Extent.MaxY;

            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);

            // TODO change to List<Coordinate>... 
            Coordinate[] coord = new Coordinate[2];
            coord[0] = new Coordinate(xmin, ymin);
            coord[1] = new Coordinate(xmin, ymax);
            LineString ls = new LineString(coord);
            f = new Feature(ls);
            fs.Features.Add(f);

            coord[0] = new Coordinate(xmin / 2, ymin);
            coord[1] = new Coordinate(xmin / 2, ymax);
            ls = new LineString(coord);
            f = new Feature(ls);
            fs.Features.Add(f);

            coord[0] = new Coordinate(xmin, ymax);
            coord[1] = new Coordinate(xmax, ymax);
            ls = new LineString(coord);
            f = new Feature(ls);
            fs.Features.Add(f);

            coord[0] = new Coordinate(xmin, ymax / 2);
            coord[1] = new Coordinate(xmax, ymax / 2);
            ls = new LineString(coord);
            f = new Feature(ls);
            fs.Features.Add(f);

            //App.Map.AddFeatureLayer 


            fs.SaveAs("C:\\Temp\\LineTest.shp", true);
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

            vertices.Add(new Coordinate(11219035, 1542354));
            vertices.Add(new Coordinate(11219035, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 0));
            Polygon geom = new Polygon(vertices);

            fs.AddFeature(geom);

            // add 16.01.18
            // add the geometry to the featureset. 
            IFeature feature = fs.AddFeature(geom);


            // now the resulting features knows what columns it has
            // add values for the columns
            feature.DataRow.BeginEdit();
            feature.DataRow["ID"] = 1;
            feature.DataRow["Text"] = "Hello World";
            feature.DataRow.EndEdit();


            vertices.Clear();
            vertices.Add(new Coordinate(11219035 + 100, 1542354));
            vertices.Add(new Coordinate(11219035 + 100, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 200, 1542354 + 100));
            vertices.Add(new Coordinate(11219035 + 200, 1542354 + 0));
            geom = new Polygon(vertices);

            feature = fs.AddFeature(geom);
            // now the resulting features knows what columns it has
            // add values for the columns
            feature.DataRow.BeginEdit();
            feature.DataRow["ID"] = 2;
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

            fs.SaveAs("C:\\Temp\\Lines.shp", true);
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
            var coor = App.Map.PixelToProj(new System.Drawing.Point(11219088, 1542359));
        }
    }
}
