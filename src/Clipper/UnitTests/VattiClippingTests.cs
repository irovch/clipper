using System.Collections.Generic;
using System.Linq;
using Clipper;
using Clipper.Custom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class VattiClippingTests
    {
        private const double Scale = GeometryHelper.PolygonScaleConstant;
        private const double ScaleInverse = GeometryHelper.PolygonScaleInverseConstant;
        private const double AreaScale = GeometryHelper.PolygonAreaScaleConstant;
        private const double AreaScaleInverse = GeometryHelper.PolygonAreaScaleInverseConstant;

        [TestInitialize]
        public void InitializeTest()
        {
        }

        [TestMethod]
        public void TestPolygonSimplifySelfIntersecting()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(00.0, 00.0),
                        Scaled(10.0, 10.0),
                        Scaled(10.0, 00.0),
                        Scaled(00.0, 10.0)
                    }));

            const double expectedArea = 50.0;

            // The self intersecting polygon has equal and opposite areas, so resultant area will be zero.
            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse));

            // Simplify the polygon
            var solution = new PolygonPath();
            ClippingHelper.SimplifyPolygon(subject, solution);

            // The simplified non self intersecting polygon has valid area.
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - expectedArea));

            // Self intersecting simplified into two polygons.
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(00.0, 00.0),
                Scaled(05.0, 05.0),
                Scaled(00.0, 10.0));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(10.0, 00.0),
                Scaled(10.0, 10.0),
                Scaled(05.0, 05.0));
        }

        [TestMethod]
        public void TestUnion1()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0.0, 0.0),
                        Scaled(1.0, 0.0),
                        Scaled(1.0, 1.0),
                        Scaled(0.0, 1.0)
                    }));

            var clip = new PolygonPath(
                    new Polygon(
                    new[]
                    {
                        Scaled(1.0, 0.0),
                        Scaled(2.0, 0.0),
                        Scaled(2.0, 1.0),
                        Scaled(1.0, 1.0)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 2.0));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(0.0, 0.0),
                Scaled(2.0, 0.0),
                Scaled(2.0, 1.0),
                Scaled(0.0, 1.0));
        }

        [TestMethod]
        public void TestUnion2()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(00, 00),
                        Scaled(10, 00),
                        Scaled(10, 10),
                        Scaled(00, 10)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(10, 00),
                        Scaled(20, 00),
                        Scaled(20, 10),
                        Scaled(10, 10)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 100.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 200.0));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(00, 00),
                Scaled(20, 00),
                Scaled(20, 10),
                Scaled(00, 10));
        }

        [TestMethod]
        public void TestUnion3()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(00, 00),
                        Scaled(10, 00),
                        Scaled(10, 10),
                        Scaled(00, 10)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(05, 00),
                        Scaled(15, 00),
                        Scaled(15, 10),
                        Scaled(05, 10)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 100.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 150.0));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(00, 00),
                Scaled(15, 00),
                Scaled(15, 10),
                Scaled(00, 10));
        }

        [TestMethod]
        public void TestUnion4()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+0.0, +0.0),
                        Scaled(+5.0, +0.0),
                        Scaled(+5.0, +5.0),
                        Scaled(+0.0, +5.0)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+2.5, -2.5),
                        Scaled(+7.5, -2.5),
                        Scaled(+7.5, +2.5),
                        Scaled(+2.5, +2.5)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 25.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 25.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            // Area is (2 * 50 * 50) - (1 * 25 * 25)
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - ((2 * 5.0 * 5.0) - (2.5 * 2.5))));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+2.5, -2.5),
                Scaled(+7.5, -2.5),
                Scaled(+7.5, +2.5),
                Scaled(+5.0, +2.5),
                Scaled(+5.0, +5.0),
                Scaled(+0.0, +5.0),
                Scaled(+0.0, +0.0),
                Scaled(+2.5, +0.0));
        }

        [TestMethod]
        public void TestUnion5()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0000, 0000),
                        Scaled(1000, 0000),
                        Scaled(1000, 1000),
                        Scaled(0000, 1000)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(2000, 0000),
                        Scaled(3000, 0000),
                        Scaled(3000, 1000),
                        Scaled(2000, 1000)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 2000000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(2000, 0000),
                Scaled(3000, 0000),
                Scaled(3000, 1000),
                Scaled(2000, 1000));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(0000, 0000),
                Scaled(1000, 0000),
                Scaled(1000, 1000),
                Scaled(0000, 1000));
        }

        [TestMethod]
        public void TestUnion6()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0000, 0000),
                        Scaled(1000, 0000),
                        Scaled(1000, 1000),
                        Scaled(0000, 1000)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, null, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(0000, 0000),
                Scaled(1000, 0000),
                Scaled(1000, 1000),
                Scaled(0000, 1000));
        }

        [TestMethod]
        public void TestUnion7()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0.0, 0.0),
                        Scaled(1.0, 0.0),
                        Scaled(1.0, 1.0),
                        Scaled(0.0, 1.0)
                    }));

            var clip = new PolygonPath(new Polygon(
                    new[]
                    {
                        Scaled(2.0, 0.0),
                        Scaled(3.0, 0.0),
                        Scaled(3.0, 1.0),
                        Scaled(2.0, 1.0)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.AreEqual(2, solution.Count);
            var area = solution.Area * AreaScaleInverse;
            Assert.IsTrue(GeometryHelper.NearZero(area - 2.0));

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(2.0, 0.0),
                Scaled(3.0, 0.0),
                Scaled(3.0, 1.0),
                Scaled(2.0, 1.0));
        }

        [TestMethod]
        public void TestIntersection1()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+00, +00),
                        Scaled(+50, +00),
                        Scaled(+50, +50),
                        Scaled(+00, +50)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+25, -25),
                        Scaled(+75, -25),
                        Scaled(+75, +25),
                        Scaled(+25, +25)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 2500.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2500.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            // Area is (25 * 25)
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - (25 * 25)));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+25, +00),
                Scaled(+50, +00),
                Scaled(+50, +25),
                Scaled(+25, +25));
        }

        [TestMethod]
        public void TestIntersection2()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(00, 00),
                        Scaled(10, 00),
                        Scaled(10, 10),
                        Scaled(00, 10)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(05, 00),
                        Scaled(15, 00),
                        Scaled(15, 10),
                        Scaled(05, 10)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 100.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 50.0));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(05, 00),
                Scaled(10, 00),
                Scaled(10, 10),
                Scaled(05, 10));
        }

        [TestMethod]
        public void TestIntersectionNonOverlapping()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0, 0),
                        Scaled(1, 0),
                        Scaled(1, 1),
                        Scaled(0, 1)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(2, 0),
                        Scaled(3, 0),
                        Scaled(3, 1),
                        Scaled(2, 1)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area - AreaScale));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area - AreaScale));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area));
            Assert.AreEqual(0, solution.Count);
        }

        [TestMethod]
        public void TestXor()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+00, +00),
                        Scaled(+50, +00),
                        Scaled(+50, +50),
                        Scaled(+00, +50)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+25, -25),
                        Scaled(+75, -25),
                        Scaled(+75, +25),
                        Scaled(+25, +25)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 2500.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2500.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));

            // Area is:
            // 1: the two original polygon path areas (2 * 50 * 50)
            // 2: less the overlap area from one of the original polygon paths (1 * 25 * 25)
            // 3: less the solution area (which has a reverse winding order and therefore a negative area)
            // (2 * 50 * 50) - (1 * 25 * 25) - (25 * 25)
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - ((2 * 50 * 50) - (1 * 25 * 25) - (25 * 25))));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+25, -25),
                Scaled(+75, -25),
                Scaled(+75, +25),
                Scaled(+50, +25),
                Scaled(+50, +50),
                Scaled(+00, +50),
                Scaled(+00, +00),
                Scaled(+25, +00));

            polygon = solution[1];
            polygon.Reverse();
            polygon.OrderBottomLeftFirst();
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+25, +00),
                Scaled(+50, +00),
                Scaled(+50, +25),
                Scaled(+25, +25));
        }

        [TestMethod]
        public void TestXorNonOverlapping()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0000, 0000),
                        Scaled(1000, 0000),
                        Scaled(1000, 1000),
                        Scaled(0000, 1000)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(2000, 0000),
                        Scaled(3000, 0000),
                        Scaled(3000, 1000),
                        Scaled(2000, 1000)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 2000000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(2000, 0000),
                Scaled(3000, 0000),
                Scaled(3000, 1000),
                Scaled(2000, 1000));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(0000, 0000),
                Scaled(1000, 0000),
                Scaled(1000, 1000),
                Scaled(0000, 1000));
        }

        [TestMethod]
        public void TestDifference()
        {
            var subject = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+00, +00),
                    Scaled(+50, +00),
                    Scaled(+50, +50),
                    Scaled(+00, +50)
                }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+25, -25),
                    Scaled(+75, -25),
                    Scaled(+75, +25),
                    Scaled(+25, +25)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 2500.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2500.0));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            // Area is:
            // 1: the original source polygon path area (50 * 50)
            // 2: less the overlap area from the operation polygon (1 * 25 * 25)
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - ((50 * 50) - (25 * 25))));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+00, +00),
                Scaled(+25, +00),
                Scaled(+25, +25),
                Scaled(+50, +25),
                Scaled(+50, +50),
                Scaled(+00, +50));
        }

        [TestMethod]
        public void TestDifferenceNonOverlapping()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(0000, 0000),
                        Scaled(1000, 0000),
                        Scaled(1000, 1000),
                        Scaled(0000, 1000)
                    }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(2000, 0000),
                    Scaled(3000, 0000),
                    Scaled(3000, 1000),
                    Scaled(2000, 1000)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(0000, 0000),
                Scaled(1000, 0000),
                Scaled(1000, 1000),
                Scaled(0000, 1000));
        }

        [TestMethod]
        public void TestContains1()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+00, +00),
                        Scaled(+10, +00),
                        Scaled(+10, +10),
                        Scaled(+00, +10)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+02, +02),
                        Scaled(+08, +02),
                        Scaled(+08, +08),
                        Scaled(+02, +08)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 36.0));

            Assert.IsTrue(ClippingHelper.Contains(subject, clip));
        }

        [TestMethod]
        public void TestContains2()
        {
            var subject = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+00, +00),
                    Scaled(+10, +00),
                    Scaled(+10, +10),
                    Scaled(+00, +10)
                }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+12, +12),
                    Scaled(+18, +12),
                    Scaled(+18, +18),
                    Scaled(+12, +18)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 36.0));

            Assert.IsFalse(ClippingHelper.Contains(subject, clip));
        }

        [TestMethod]
        public void TestContains3()
        {
            var subject = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+00, +00),
                    Scaled(+10, +00),
                    Scaled(+10, +10),
                    Scaled(+00, +10)
                }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+00, +00),
                    Scaled(+10, +00),
                    Scaled(+10, +10),
                    Scaled(+00, +10)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 100.0));

            Assert.IsTrue(ClippingHelper.Contains(subject, clip));
        }

        [TestMethod]
        public void TestContains4()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+00, +00),
                        Scaled(+10, +00),
                        Scaled(+10, +10),
                        Scaled(+00, +10)
                    }));

            var clip = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+00, +00),
                        Scaled(+10, +00),
                        Scaled(+11, +10),
                        Scaled(+00, +10)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 100.0));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 105.0));

            Assert.IsFalse(ClippingHelper.Contains(subject, clip));
        }

        [TestMethod]
        public void TestSimplify1()
        {
            var path = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+00.00, +10.00),
                    Scaled(-05.00, -10.00),
                    Scaled(+10.00, +05.00),
                    Scaled(-10.00, +05.00),
                    Scaled(+05.00, -10.00)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(ClippingHelper.SimplifyPolygon(path, solution));

            Assert.AreEqual(5, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-01.25, +05.00),
                Scaled(+01.25, +05.00),
                Scaled(+00.00, +10.00));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-03.00, -02.00),
                Scaled(-01.25, +05.00),
                Scaled(-10.00, +05.00));

            polygon = solution[2];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+05.00, -10.00),
                Scaled(+03.00, -02.00),
                Scaled(+00.00, -05.00));

            polygon = solution[3];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+03.00, -02.00),
                Scaled(+10.00, +05.00),
                Scaled(+01.25, +05.00));

            polygon = solution[4];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(-05.00, -10.00),
                Scaled(+00.00, -05.00),
                Scaled(-03.00, -02.00));
        }

        [TestMethod]
        public void TestSingleEdgeClip()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+0000, +0000),
                        Scaled(+1000, +0000),
                        Scaled(+1000, +1000),
                        Scaled(+0000, +1000)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, null, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(1, solution.Count);

            // First polygon is subject
            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.IsTrue(GeometryHelper.NearZero(polygon.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +1000),
                Scaled(+0000, +1000),
                Scaled(+0000, +0000),
                Scaled(+1000, +0000));
        }

        [TestMethod]
        public void TestUnionNoClipPolygon()
        {
            var subject = new PolygonPath(
                new Polygon(
                    new[]
                    {
                        Scaled(+0000, -0500),
                        Scaled(+0000, -0500),
                        Scaled(+0500, -0500),
                        Scaled(+0500, +0000),
                        Scaled(+0500, +0500),
                        Scaled(+0000, +0500),
                        Scaled(-0500, +0500),
                        Scaled(-0500, +0500),
                        Scaled(-0500, +0000),
                        Scaled(-0500, -0500),
                        Scaled(+0000, -0500),
                        Scaled(+0000, -0500)
                    }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, null, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));
        }

        [TestMethod]
        public void TestDiamond()
        {
            var subject = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+0100, +0000),
                    Scaled(-0250, +0250),
                    Scaled(-0500, +0000),
                    Scaled(-0250, -0250)
                }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(-0100, +0000),
                    Scaled(+0250, -0250),
                    Scaled(+0500, +0000),
                    Scaled(+0250, +0250)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 150000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 150000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 285714.2857));
            Assert.AreEqual(1, solution.Count);
            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+500, +000),
                Scaled(+250, +250),
                Scaled(+000, +071.4285714),
                Scaled(-250, +250),
                Scaled(-500, +000),
                Scaled(-250, -250),
                Scaled(+000, -071.4285714),
                Scaled(+250, -250));

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 14285.71428));
            Assert.AreEqual(1, solution.Count);
            polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+100, +000),
                Scaled(+000, +071.4285714),
                Scaled(-100, +000),
                Scaled(+000, -071.4285714));

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 135714.28571));
            Assert.AreEqual(1, solution.Count);
            polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+000, -071.4285714),
                Scaled(-100, +000),
                Scaled(+000, +071.4285714),
                Scaled(-250, +250),
                Scaled(-500, +000),
                Scaled(-250, -250));

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 271428.57142));
            Assert.AreEqual(2, solution.Count);
            polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+500, +000),
                Scaled(+250, +250),
                Scaled(+000, +071.4285714),
                Scaled(+100, +000),
                Scaled(+000, -071.4285714),
                Scaled(+250, -250));
            polygon = solution[1];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+000, -071.4285714),
                Scaled(-100, +000),
                Scaled(+000, +071.4285714),
                Scaled(-250, +250),
                Scaled(-500, +000),
                Scaled(-250, -250));
        }

        [TestMethod]
        public void TestDiamondUnionSingleTouchVertex()
        {
            var subject = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(+1000, +0500),
                    Scaled(+0500, +1000),
                    Scaled(-1000, +0500),
                    Scaled(+0000, +0000)
                }));

            var clip = new PolygonPath(new Polygon(
                new[]
                {
                    Scaled(-0500, -1000),
                    Scaled(+1000, -0500),
                    Scaled(+0000, +0000),
                    Scaled(-1000, -0500)
                }));

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 2000000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0500),
                Scaled(+0500, +1000),
                Scaled(-1000, +0500),
                Scaled(+0000, +0000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, -0500),
                Scaled(+0000, +0000),
                Scaled(-1000, -0500),
                Scaled(-0500, -1000));

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area));
            Assert.AreEqual(0, solution.Count);

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1000000));
            Assert.AreEqual(1, solution.Count);

            polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0500),
                Scaled(+0500, +1000),
                Scaled(-1000, +0500),
                Scaled(+0000, +0000));

            solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));
            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 2000000));
            Assert.AreEqual(2, solution.Count);
            polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0500),
                Scaled(+0500, +1000),
                Scaled(-1000, +0500),
                Scaled(+0000, +0000));

            polygon = solution[1];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, -0500),
                Scaled(+0000, +0000),
                Scaled(-1000, -0500),
                Scaled(-0500, -1000));
        }

        [TestMethod]
        public void TestMergeWithHole1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var merge1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var merge2 = new Polygon(
                new[]
                {
                    Scaled(+0250, +0250),
                    Scaled(+0250, -0250),
                    Scaled(-0250, -0250),
                    Scaled(-0250, +0250)
                });

            var subject = new PolygonPath(subject1);
            var clip = new PolygonPath(new[] { merge1, merge2 });

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 750000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 750000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0250, -0250),
                Scaled(-0250, +0250),
                Scaled(+0250, +0250),
                Scaled(+0250, -0250));
        }

        [TestMethod]
        public void TestMergeWithHole2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(+0000, +1000),
                    Scaled(+0000, +0000),
                    Scaled(+0900, +0000),
                    Scaled(+0900, +1000)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0200),
                    Scaled(+0200, +0200),
                    Scaled(+0200, +0800),
                    Scaled(+0400, +0800)
                });

            var merge1 = new Polygon(
                new[]
                {
                    Scaled(+1000, +1000),
                    Scaled(+0100, +1000),
                    Scaled(+0100, +0000),
                    Scaled(+1000, +0000)
                });

            var merge2 = new Polygon(
                new[]
                {
                    Scaled(+0600, +0200),
                    Scaled(+0600, +0800),
                    Scaled(+0800, +0800),
                    Scaled(+0800, +0200)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(new[] { merge1, merge2 });

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (900 * 1000 - 200 * 600)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - (900 * 1000 - 200 * 600)));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - (800 * 1000 - 200 * 600 - 200 * 600)));
            Assert.AreEqual(3, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0900, +1000),
                Scaled(+0100, +1000),
                Scaled(+0100, +0000),
                Scaled(+0900, +0000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0600, +0200),
                Scaled(+0600, +0800),
                Scaled(+0800, +0800),
                Scaled(+0800, +0200));

            polygon = solution[2];
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0200, +0200),
                Scaled(+0200, +0800),
                Scaled(+0400, +0800),
                Scaled(+0400, +0200));
        }

        [TestMethod]
        public void TestWithHoleUnion()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+0000, +0000),
                    Scaled(+0000, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0000)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1270000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0000),
                Scaled(+0500, +0000),
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0000, -0500),
                Scaled(+0000, -1000),
                Scaled(+1000, -1000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, +0000),
                Scaled(+0000, +0000),
                Scaled(+0000, -0400));
        }

        [TestMethod]
        public void TestWith2HoleUnion1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(-1000, +1000),
                    Scaled(-1000, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +1000)
                });

            var clip2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(new[] { clip1, clip2 });

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - (4000000 - 640000)));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 3360000));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +1000),
                Scaled(-1000, +1000),
                Scaled(-1000, -1000),
                Scaled(+1000, -1000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestWith2HoleUnion2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var clip2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(new[] { clip1, clip2 });

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - (1000000 - 640000)));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestWithHoleUnionSmall()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0.05, +0.05),
                    Scaled(-0.05, -0.05),
                    Scaled(+0.05, -0.05),
                    Scaled(+0.05, +0.05)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0.04, +0.04),
                    Scaled(+0.04, -0.04),
                    Scaled(-0.04, -0.04),
                    Scaled(-0.04, +0.04)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+0.00, +0.00),
                    Scaled(+0.00, -0.10),
                    Scaled(+0.10, -0.10),
                    Scaled(+0.10, +0.00)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (0.0100 - 0.0064)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 0.0100));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 0.0127));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0.10, +0.00),
                Scaled(+0.05, +0.00),
                Scaled(+0.05, +0.05),
                Scaled(-0.05, +0.05),
                Scaled(-0.05, -0.05),
                Scaled(+0.00, -0.05),
                Scaled(+0.00, -0.10),
                Scaled(+0.10, -0.10));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0.04, -0.04),
                Scaled(-0.04, +0.04),
                Scaled(+0.04, +0.04),
                Scaled(+0.04, +0.00),
                Scaled(+0.00, +0.00),
                Scaled(+0.00, -0.04));
        }

        [TestMethod]
        public void TestWithHoleSubtractLargeAndSmall1()
        {
            // 5000 metres (as mm)
            const int large = 5000 * 1000;

            // 40 microns (as mm)
            const double small = 40 * 0.001;

            var subjectPolygon = new Polygon(
                new[]
                {
                    Scaled(-large, +large),
                    Scaled(-large, -large),
                    Scaled(+large, -large),
                    Scaled(+large, +large)
                });

            var clipPolygon = new Polygon(
                new[]
                {
                    Scaled(-small, +small),
                    Scaled(-small, -small),
                    Scaled(+small, -small),
                    Scaled(+small, +small)
                });

            var subject = new PolygonPath(subjectPolygon);
            var clip = new PolygonPath(clipPolygon);

            const double subjectArea = 2.0 * large * 2.0 * large;
            const double clipArea = 2.0 * small * 2.0 * small;
            const double solutionArea = subjectArea - clipArea;

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - subjectArea));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - clipArea));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - solutionArea));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.IsTrue(GeometryHelper.NearZero(polygon.Area * AreaScaleInverse - subjectArea));
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+large, +large),
                Scaled(-large, +large),
                Scaled(-large, -large),
                Scaled(+large, -large));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.IsTrue(GeometryHelper.NearZero(polygon.Area * AreaScaleInverse + clipArea));
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-small, -small),
                Scaled(-small, +small),
                Scaled(+small, +small),
                Scaled(+small, -small));
        }

        [TestMethod]
        public void TestWithHoleSubtractLargeAndSmall2()
        {
            // 5000 metres (as mm)
            const double large = 5000 * 1000;

            // 40 microns (as mm)
            const double small = 40 * 0.001;

            var subjectPolygon = new Polygon(
                new[]
                {
                    Scaled(     0,      0),
                    Scaled(+large,      0),
                    Scaled(+large, +large),
                    Scaled(     0, +large)
                });

            var clipPolygon = new Polygon(
                new[]
                {
                    Scaled(-small, +small),
                    Scaled(-small, -small),
                    Scaled(+small, -small),
                    Scaled(+small, +small)
                });

            var subject = new PolygonPath(subjectPolygon);
            var clip = new PolygonPath(clipPolygon);

            const double subjectArea = large * large;
            const double clipArea = 2.0 * small * 2.0 * small;
            const double solutionArea = subjectArea - clipArea / 4.0;

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - subjectArea));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - clipArea));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - solutionArea));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.IsTrue(GeometryHelper.NearZero(polygon.Area * AreaScaleInverse - subjectArea));
            Assert.AreEqual(6, polygon.Count);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            Assert.AreEqual(+large * Scale, polygon[0].X); Assert.AreEqual(+large * Scale, polygon[0].Y);
            Assert.AreEqual(0 * Scale, polygon[1].X); Assert.AreEqual(+large * Scale, polygon[1].Y);
            Assert.AreEqual(0 * Scale, polygon[2].X); Assert.AreEqual(+small * Scale, polygon[2].Y);
            Assert.AreEqual(+small * Scale, polygon[3].X); Assert.AreEqual(+small * Scale, polygon[3].Y);
            Assert.AreEqual(+small * Scale, polygon[4].X); Assert.AreEqual(0 * Scale, polygon[4].Y);
            Assert.AreEqual(+large * Scale, polygon[5].X); Assert.AreEqual(0 * Scale, polygon[5].Y);
        }

        [TestMethod]
        public void TestWithHoleUnionLargeAndSmall()
        {
            // 5000 metres (as mm)
            const double large = 5000 * 1000;

            // 40 microns (as mm)
            const double small = 40 * 0.001;

            var subjectPolygon = new Polygon(
                new[]
                {
                    Scaled(     0,      0),
                    Scaled(+large,      0),
                    Scaled(+large, +large),
                    Scaled(     0, +large)
                });

            var clipPolygon = new Polygon(
                new[]
                {
                    Scaled(-small, +small),
                    Scaled(-small, -small),
                    Scaled(+small, -small),
                    Scaled(+small, +small)
                });

            var subject = new PolygonPath(subjectPolygon);
            var clip = new PolygonPath(clipPolygon);

            const double subjectArea = large * large;
            const double clipArea = 2.0 * small * 2.0 * small;
            const double solutionArea = subjectArea + 3.0 * clipArea / 4.0;

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - subjectArea));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - clipArea));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - solutionArea));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.IsTrue(GeometryHelper.NearZero(polygon.Area * AreaScaleInverse - solutionArea));
            Assert.AreEqual(8, polygon.Count);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            Assert.AreEqual(+small, polygonPoints[0].X); Assert.AreEqual(0, polygonPoints[0].Y);
            Assert.AreEqual(+large, polygonPoints[1].X); Assert.AreEqual(0, polygonPoints[1].Y);
            Assert.AreEqual(+large, polygonPoints[2].X); Assert.AreEqual(+large, polygonPoints[2].Y);
            Assert.AreEqual(0, polygonPoints[3].X); Assert.AreEqual(+large, polygonPoints[3].Y);
            Assert.AreEqual(0, polygonPoints[4].X); Assert.AreEqual(+small, polygonPoints[4].Y);
            Assert.AreEqual(-small, polygonPoints[5].X); Assert.AreEqual(+small, polygonPoints[5].Y);
            Assert.AreEqual(-small, polygonPoints[6].X); Assert.AreEqual(-small, polygonPoints[6].Y);
            Assert.AreEqual(+small, polygonPoints[7].X); Assert.AreEqual(-small, polygonPoints[7].Y);
        }

        [TestMethod]
        public void TestWithHoleUnion10MetreX10Metre()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-050000, +050000),
                    Scaled(-050000, -050000),
                    Scaled(+050000, -050000),
                    Scaled(+050000, +050000)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+040000, +040000),
                    Scaled(+040000, -040000),
                    Scaled(-040000, -040000),
                    Scaled(-040000, +040000)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+000000, +000000),
                    Scaled(+000000, -100000),
                    Scaled(+100000, -100000),
                    Scaled(+100000, +000000)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            const double subject1Area = 100000.0 * 100000.0;
            const double subject2Area = 080000.0 * 080000.0;
            const double clip1Area = 100000.0 * 100000.0;

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (subject1Area - subject2Area)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - clip1Area));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 12699999999.999998));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+100000, +000000),
                Scaled(+050000, +000000),
                Scaled(+050000, +050000),
                Scaled(-050000, +050000),
                Scaled(-050000, -050000),
                Scaled(+000000, -050000),
                Scaled(+000000, -100000),
                Scaled(+100000, -100000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-40000, -40000),
                Scaled(-40000, +40000),
                Scaled(+40000, +40000),
                Scaled(+40000, +00000),
                Scaled(+00000, +00000),
                Scaled(+00000, -40000));
        }

        [TestMethod]
        public void TestWithHoleIntersection1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+0000, +0000),
                    Scaled(+0000, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0000)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 90000));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0000),
                Scaled(+0400, +0000),
                Scaled(+0400, -0400),
                Scaled(+0000, -0400),
                Scaled(+0000, -0500),
                Scaled(+0500, -0500));
        }

        [TestMethod]
        public void TestWithHoleIntersection2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(-0450, +0450),
                    Scaled(-0450, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0450)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2102500));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 262500));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0450),
                Scaled(-0450, +0450),
                Scaled(-0450, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestWithHoleSubtract1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+0000, +0000),
                    Scaled(+0000, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0000)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 270000));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0000, -0400),
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, +0000),
                Scaled(+0500, +0000),
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0000, -0500));
        }

        [TestMethod]
        public void TestWithHoleSubtract2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(-0450, +0450),
                    Scaled(-0450, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0450)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2102500));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Difference, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 97500));
            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0450, +0450),
                Scaled(+0500, +0450),
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(-0450, -0500));
        }

        [TestMethod]
        public void TestWithHoleXor1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+0000, +0000),
                    Scaled(+0000, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0000)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 1000000));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1180000));
            Assert.AreEqual(3, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0000),
                Scaled(+0500, +0000),
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0000, -0500),
                Scaled(+0000, -1000),
                Scaled(+1000, -1000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0000, -0500),
                Scaled(+0000, -0400),
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, +0000),
                Scaled(+0500, +0000),
                Scaled(+0500, -0500));

            polygon = solution[2];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0400, +0000),
                Scaled(+0000, +0000),
                Scaled(+0000, -0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestWithHoleXor2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(-0450, +0450),
                    Scaled(-0450, -1000),
                    Scaled(+1000, -1000),
                    Scaled(+1000, +0450)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 2102500));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Xor, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 1937500));
            Assert.AreEqual(3, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+1000, +0450),
                Scaled(+0500, +0450),
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(-0450, -0500),
                Scaled(-0450, -1000),
                Scaled(+1000, -1000));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0450, -0500),
                Scaled(-0450, +0450),
                Scaled(+0500, +0450),
                Scaled(+0500, -0500));

            polygon = solution[2];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0400, +0400),
                Scaled(-0400, +0400),
                Scaled(-0400, -0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestSelfIntersectingWithHoleUnion()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(+03, +03),
                    Scaled(+08, +08),
                    Scaled(+06, +08),
                    Scaled(+06, +03),
                    Scaled(+02, +07),
                    Scaled(+01, +04)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+02, +04),
                    Scaled(+02, +05),
                    Scaled(+03, +05),
                    Scaled(+03, +04)
                });

            var clip1 = new Polygon(
                new[]
                {
                    Scaled(+01, +07),
                    Scaled(+01, +03),
                    Scaled(+08, +03),
                    Scaled(+08, +08),
                    Scaled(+01, +08)
                });

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - 6));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area * AreaScaleInverse - 35));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - 10.5));
            Assert.AreEqual(3, solution.Count);
            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+6.0, +6.0),
                Scaled(+8.0, +8.0),
                Scaled(+6.0, +8.0),
                Scaled(+6.0, +6.0),
                Scaled(+4.5, +4.5),
                Scaled(+6.0, +3.0));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+04.5, +04.5),
                Scaled(+02.0, +07.0),
                Scaled(+01.0, +04.0),
                Scaled(+03.0, +03.0));

            polygon = solution[2];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+02.0, +04.0),
                Scaled(+02.0, +05.0),
                Scaled(+03.0, +05.0),
                Scaled(+03.0, +04.0));
        }

        [TestMethod]
        public void TestPolygonSimplify()
        {
            var polygon = new Polygon(
                 new[]
                 {
                    Scaled(+00000000, -05000000),
                    Scaled(+00000000, -05000000),
                    Scaled(+05000000, -05000000),
                    Scaled(+05000000, +00000000),
                    Scaled(+05000000, +05000000),
                    Scaled(+00000000, +05000000),
                    Scaled(-05000000, +05000000),
                    Scaled(-05000000, +05000000),
                    Scaled(-05000000, +00000000),
                    Scaled(-05000000, -05000000),
                    Scaled(-05000000, -05000000),
                    Scaled(+00000000, -05000000),
                    Scaled(+00000000, -05000000)
                 });

            Assert.AreEqual(100000000000000, polygon.Area * AreaScaleInverse);

            polygon.Simplify();
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(100000000000000, polygon.Area * AreaScaleInverse);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+05000000, -05000000),
                Scaled(+05000000, +05000000),
                Scaled(-05000000, +05000000),
                Scaled(-05000000, -05000000));
        }

        [TestMethod]
        public void TestEmptyUnion1()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(+0400, +0400),
                    Scaled(+0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, +0400)
                });

            var clip1 = new Polygon();

            var subject = new PolygonPath(new[] { subject1, subject2 });
            var clip = new PolygonPath(clip1);

            Assert.IsTrue(GeometryHelper.NearZero(subject.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.IsTrue(GeometryHelper.NearZero(clip.Area));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area * AreaScaleInverse - (1000000 - 640000)));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestEmptyUnion2()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(-0400, +0400),
                    Scaled(-0400, -0400),
                    Scaled(+0400, -0400),
                    Scaled(+0400, +0400)
                });

            Assert.IsTrue(GeometryHelper.NearZero(subject1.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(subject2.Area * AreaScaleInverse - 640000));

            var subject = new PolygonPath(
                new[]
                {
                    subject1,
                    subject2
                });

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, null, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area - (subject1.Area - subject2.Area)));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }

        [TestMethod]
        public void TestEmptyUnion3()
        {
            var subject1 = new Polygon(
                new[]
                {
                    Scaled(-0500, +0500),
                    Scaled(-0500, +0500),
                    Scaled(-0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, -0500),
                    Scaled(+0500, +0500)
                });

            var subject2 = new Polygon(
                new[]
                {
                    Scaled(-0400, +0400),
                    Scaled(-0400, -0400),
                    Scaled(-0400, -0400),
                    Scaled(+0300, -0400),
                    Scaled(-0300, -0400),
                    Scaled(+0400, -0400),
                    Scaled(+0400, -0400),
                    Scaled(+0400, -0400),
                    Scaled(+0400, +0400)
                });

            Assert.IsTrue(GeometryHelper.NearZero(subject1.Area * AreaScaleInverse - 1000000));
            Assert.IsTrue(GeometryHelper.NearZero(subject2.Area * AreaScaleInverse - 640000));

            var subject = new PolygonPath(
                new[]
                {
                    subject1,
                    subject2
                });

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, null, solution));

            Assert.IsTrue(GeometryHelper.NearZero(solution.Area - (subject1.Area - subject2.Area)));
            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            var polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.CounterClockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(+0500, +0500),
                Scaled(-0500, +0500),
                Scaled(-0500, -0500),
                Scaled(+0500, -0500));

            polygon = solution[1];
            polygonPoints = FromScaledPolygon(polygon);
            Assert.AreEqual(PolygonOrientation.Clockwise, polygon.Orientation);
            Assert.AreEqual(PolygonOrientation.Clockwise, GeometryHelper.GetOrientation(polygonPoints));
            AssertEqual(
                polygon,
                Scaled(-0400, -0400),
                Scaled(-0400, +0400),
                Scaled(+0400, +0400),
                Scaled(+0400, -0400));
        }


        [TestMethod]
        public void MiscShapeTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+01.0, +02.0),
                    Scaled(+03.0, +02.0),
                    Scaled(+04.0, +01.0),
                    Scaled(+05.0, +02.0),
                    Scaled(+07.0, +02.0),
                    Scaled(+06.0, +03.0),
                    Scaled(+07.0, +04.0),
                    Scaled(+05.0, +04.0),
                    Scaled(+04.0, +05.0),
                    Scaled(+03.0, +04.0),
                    Scaled(+01.0, +04.0),
                    Scaled(+02.0, +03.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+01.0, +01.0),
                    Scaled(+03.0, +00.0),
                    Scaled(+05.0, +00.0),
                    Scaled(+07.0, +01.0),
                    Scaled(+08.0, +03.0),
                    Scaled(+08.0, +05.0),
                    Scaled(+07.0, +07.0),
                    Scaled(+05.0, +08.0),
                    Scaled(+03.0, +08.0),
                    Scaled(+01.0, +07.0),
                    Scaled(+00.0, +05.0),
                    Scaled(+00.0, +03.0)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Union, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            solution[0].OrderBottomLeftFirst();
            AssertEqual(
                solution[0],
                Scaled(+03.0, +00.0),
                Scaled(+05.0, +00.0),
                Scaled(+07.0, +01.0),
                Scaled(+08.0, +03.0),
                Scaled(+08.0, +05.0),
                Scaled(+07.0, +07.0),
                Scaled(+05.0, +08.0),
                Scaled(+03.0, +08.0),
                Scaled(+01.0, +07.0),
                Scaled(+00.0, +05.0),
                Scaled(+00.0, +03.0),
                Scaled(+01.0, +01.0));

            solution = new PolygonPath();

            // Orientation of polygons should not matter, switch to clockwise.
            subject.ReversePolygonOrientations();
            clip.ReversePolygonOrientations();

            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            solution[0].OrderBottomLeftFirst();
            AssertEqual(
                solution[0],
                Scaled(+04.0, +01.0),
                Scaled(+05.0, +02.0),
                Scaled(+07.0, +02.0),
                Scaled(+06.0, +03.0),
                Scaled(+07.0, +04.0),
                Scaled(+05.0, +04.0),
                Scaled(+04.0, +05.0),
                Scaled(+03.0, +04.0),
                Scaled(+01.0, +04.0),
                Scaled(+02.0, +03.0),
                Scaled(+01.0, +02.0),
                Scaled(+03.0, +02.0));
        }

        [TestMethod]
        public void BasicClipTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+08.0, +05.0),
                    Scaled(+12.0, +02.0),
                    Scaled(+12.0, +08.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+00.0, +00.0),
                    Scaled(+10.0, +00.0),
                    Scaled(+10.0, +10.0),
                    Scaled(+00.0, +10.0)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+10.0, +06.5),
                Scaled(+08.0, +05.0),
                Scaled(+10.0, +03.5));

            solution = new PolygonPath();

            // Orientation of polygons should not matter, switch to clockwise.
            subject.ReversePolygonOrientations();
            clip.ReversePolygonOrientations();

            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+10.0, +06.5),
                Scaled(+08.0, +05.0),
                Scaled(+10.0, +03.5));
        }

        [TestMethod]
        public void WithDuplicateAnColinearClipTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+08.0, +05.0),
                    Scaled(+08.0, +05.0),
                    Scaled(+12.0, +02.0),
                    Scaled(+12.0, +08.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+05.0, +00.0),
                Scaled(+10.0, +00.0),
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+10.0, +06.5),
                Scaled(+08.0, +05.0),
                Scaled(+10.0, +03.5));
        }

        [TestMethod]
        public void SharedEdgeTestClipTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+00.0, +01.0),
                    Scaled(+02.0, +02.0),
                    Scaled(+00.0, +03.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+00.0, +00.0),
                    Scaled(+05.0, +00.0),
                    Scaled(+05.0, +05.0),
                    Scaled(+00.0, +05.0)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+02.0, +02.0),
                Scaled(+00.0, +03.0),
                Scaled(+00.0, +01.0));
        }

        [TestMethod]
        public void SharedEdgeAndVertexTestClipTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+05.0, +05.0),
                    Scaled(+05.0, +00.0),
                    Scaled(+02.0, +03.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+00.0, +00.0),
                    Scaled(+05.0, +00.0),
                    Scaled(+05.0, +05.0),
                    Scaled(+00.0, +05.0)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+05.0, +05.0),
                Scaled(+02.0, +03.0),
                Scaled(+05.0, +00.0));
        }

        [TestMethod]
        public void NoClipTest()
        {
            var subject = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+10.1, +05.0),
                    Scaled(+12.0, +02.0),
                    Scaled(+12.0, +08.0)
                }));

            var clip = new PolygonPath(
                new Polygon(new[]
                {
                    Scaled(+00.0, +00.0),
                    Scaled(+10.0, +00.0),
                    Scaled(+10.0, +10.0),
                    Scaled(+00.0, +10.0)
                }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(0, solution.Count);
        }

        [TestMethod]
        public void AllClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+03.0, +05.0),
                Scaled(+08.0, +02.0),
                Scaled(+08.0, +08.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0),
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+08.0, +08.0),
                Scaled(+03.0, +05.0),
                Scaled(+08.0, +02.0));
        }

        [TestMethod]
        public void SamePolygonClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0),
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0),
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0),
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0));
        }

        [TestMethod]
        public void OpenSubject1ClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +60.0),
                Scaled(+00.0, +50.0),
                Scaled(+30.0, +20.0),
                Scaled(+60.0, +50.0),
                Scaled(+60.0, +60.0)
            })
            {
                IsClosed = false
            });

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+10.0, +50.0),
                Scaled(+10.0, +00.0),
                Scaled(+50.0, +00.0),
                Scaled(+50.0, +50.0)
            }));

            var tree = new PolygonTree();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));

            var solution = PolygonPath.FromTree(tree);

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];

            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            AssertEqual(
                polygon,
                Scaled(+10.0, +40.0),
                Scaled(+30.0, +20.0),
                Scaled(+50.0, +40.0));
        }

        [TestMethod]
        public void OpenSubject2ClipTest()
        {
            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +60.0),
                Scaled(+00.0, +50.0),
                Scaled(+30.0, +20.0),
                Scaled(+60.0, +50.0),
                Scaled(+60.0, +60.0)
            }));

            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+10.0, +50.0),
                Scaled(+10.0, +00.0),
                Scaled(+50.0, +00.0),
                Scaled(+50.0, +50.0)
            })
            {
                IsClosed = false
            });

            var tree = new PolygonTree();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));
            var solution = PolygonPath.FromTree(tree);

            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            AssertEqual(
                polygon,
                Scaled(+10.0, +40.0),
                Scaled(+10.0, +50.0));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            AssertEqual(
                polygon,
                Scaled(+50.0, +40.0),
                Scaled(+50.0, +50.0));
        }

        [TestMethod]
        public void OpenSubject3ClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, -10.0),
                Scaled(+00.0, +00.0),
                Scaled(+30.0, +30.0),
                Scaled(+60.0, +00.0),
                Scaled(+60.0, -10.0)
            })
            {
                IsClosed = false
            });

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+10.0, +50.0),
                Scaled(+10.0, +00.0),
                Scaled(+50.0, +00.0),
                Scaled(+50.0, +50.0)
            }));

            var tree = new PolygonTree();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));

            var solution = PolygonPath.FromTree(tree);

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            Assert.AreEqual(PolygonOrientation.CounterClockwise, polygon.Orientation);
            AssertEqual(
                polygon,
                Scaled(+50.0, +10.0),
                Scaled(+30.0, +30.0),
                Scaled(+10.0, +10.0));
        }

        [TestMethod]
        public void ClipWithLineTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+10.0, +50.0),
                Scaled(+10.0, +00.0),
                Scaled(+50.0, +00.0),
                Scaled(+50.0, +50.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +20.0),
                Scaled(+00.0, +30.0),
                Scaled(+60.0, +30.0),
                Scaled(+60.0, +20.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();

            AssertEqual(
                polygon,
                Scaled(+10.0, +20.0),
                Scaled(+50.0, +20.0),
                Scaled(+50.0, +30.0),
                Scaled(+10.0, +30.0));
        }

        [TestMethod]
        public void OverlappingPolygonClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+12.0, +00.0),
                Scaled(+12.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0),
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+10.0, +10.0),
                Scaled(+00.0, +10.0),
                Scaled(+00.0, +00.0),
                Scaled(+10.0, +00.0));
        }

        [TestMethod]
        public void ConcaveClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(-020, +020),
                Scaled(+020, +020),
                Scaled(-005, +050),
                Scaled(+020, +080),
                Scaled(-020, +080)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+000, +000),
                Scaled(+100, +000),
                Scaled(+100, +100),
                Scaled(+000, +100)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(2, solution.Count);
    
            AssertEqual(
                solution[0],
                Scaled(+020, +080),
                Scaled(+000, +080),
                Scaled(+000, +056));
            
            AssertEqual(
                solution[1],
                Scaled(+000, +044),
                Scaled(+000, +020),
                Scaled(+020, +020));
        }

        [TestMethod]
        public void MultiIntersection1ClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+020, +020),
                Scaled(+120, +020),
                Scaled(+120, +060),
                Scaled(+020, +060)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+000, +000),
                Scaled(+040, +000),
                Scaled(+050, +040),
                Scaled(+060, +000),
                Scaled(+070, +000),
                Scaled(+080, +040),
                Scaled(+090, +000),
                Scaled(+100, +000),
                Scaled(+100, +100),
                Scaled(+000, +100)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            AssertEqual(
                solution[0],
                Scaled(+100, +060),
                Scaled(+020, +060),
                Scaled(+020, +020),
                Scaled(+045, +020),
                Scaled(+050, +040),
                Scaled(+055, +020),
                Scaled(+075, +020),
                Scaled(+080, +040),
                Scaled(+085, +020),
                Scaled(+100, +020));
        }

        [TestMethod]
        public void MultiIntersection2ClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+13.0, +02.0),
                Scaled(+15.0, +03.0),
                Scaled(+15.0, +06.0),
                Scaled(+16.0, +09.0),
                Scaled(+13.0, +13.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+19.0, +08.0),
                Scaled(+10.0, +01.0),
                Scaled(+10.0, +03.0),
                Scaled(+03.0, +03.0),
                Scaled(+03.0, +15.0),
                Scaled(+17.0, +10.0),
                Scaled(+08.0, +10.0),
                Scaled(+08.0, +08.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(2, solution.Count);

            AssertEqual(
                solution[0],
                Scaled(+15.25, +10.0),
                Scaled(+14.6097561, +10.8536585),
                Scaled(+13.0, +11.42857145),
                Scaled(+13.0 , +10.0));

            AssertEqual(
                solution[1],
                Scaled(+15.0, +4.8888889),
                Scaled(+15.0, +06.0),
                Scaled(+15.6666667, +08.0),
                Scaled(+13.0, +08.0),
                Scaled(+13.0, +3.3333333));
        }

        [TestMethod]
        public void MultiIntersection3ClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+06.0, +11.0),
                Scaled(+22.0, +11.0),
                Scaled(+22.0, +01.0),
                Scaled(+12.0, +01.0),
                Scaled(+12.0, +06.0),
                Scaled(+06.0, +06.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+19.0, +08.0),
                Scaled(+10.0, +01.0),
                Scaled(+10.0, +03.0),
                Scaled(+03.0, +03.0),
                Scaled(+03.0, +15.0),
                Scaled(+17.0, +10.0),
                Scaled(+08.0, +10.0),
                Scaled(+08.0, +08.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+19.0, +08.0),
                Scaled(+08.0, +08.0),
                Scaled(+08.0, +10.0),
                Scaled(+17.0, +10.0),
                Scaled(+14.2, +11.0),
                Scaled(+06.0, +11.0),
                Scaled(+06.0, +06.0),
                Scaled(+12.0, +06.0),
                Scaled(+12.0, +2.5555556));
        }

        [TestMethod]
        public void SingleMidVertexClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+03.0, +06.0),
                Scaled(+05.0, +03.0),
                Scaled(+01.0, +03.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +06.0),
                Scaled(+06.0, +06.0),
                Scaled(+06.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(0, solution.Count);
        }

        [TestMethod]
        public void SingleCornerVertexClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+06.0, +10.0),
                Scaled(+06.0, +13.0),
                Scaled(+08.0, +13.0),
                Scaled(+08.0, +10.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +06.0),
                Scaled(+06.0, +06.0),
                Scaled(+06.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(0, solution.Count);
        }

        [TestMethod]
        public void SharedEdgeVertexClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+06.0, +09.0),
                Scaled(+09.0, +09.0),
                Scaled(+09.0, +07.0),
                Scaled(+06.0, +07.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +06.0),
                Scaled(+06.0, +06.0),
                Scaled(+06.0, +10.0),
                Scaled(+00.0, +10.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(0, solution.Count);
        }

        [TestMethod]
        public void NoSubjectInteriorVertexClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +04.0),
                Scaled(+04.0, +04.0),
                Scaled(+04.0, +00.0),
                Scaled(+00.0, +00.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(-02.0, +01.0),
                Scaled(-02.0, +03.0),
                Scaled(+06.0, +03.0),
                Scaled(+06.0, +01.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+04.0, +03.0),
                Scaled(+00.0, +03.0),
                Scaled(+00.0, +01.0),
                Scaled(+04.0, +01.0));
        }

        [TestMethod]
        public void SelfIntersectingSubjectClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+06.0, +02.0),
                Scaled(+08.0, +04.0),
                Scaled(+10.0, +04.0),
                Scaled(+11.0, +03.0),
                Scaled(+11.0, +01.0),
                Scaled(+13.0, +01.0),
                Scaled(+13.0, +06.0),
                Scaled(+10.0, +08.0),
                Scaled(+09.0, +08.0),
                Scaled(+09.0, +03.0),
                Scaled(+08.0, +05.0),
                Scaled(+02.0, +03.0),
                Scaled(+02.0, +08.0),
                Scaled(+07.0, +08.0),
                Scaled(+05.0, +06.0),
                Scaled(+06.0, +04.0),
                Scaled(+03.0, +04.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+14.0, +08.0),
                Scaled(+14.0, +11.0),
                Scaled(+03.0, +11.0),
                Scaled(+03.0, +09.0),
                Scaled(+08.0, +07.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(2, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+09.0, +07.1666667),
                Scaled(+10.8, +07.4666667),
                Scaled(+10.0, +08.0),
                Scaled(+09.0, +08.0));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+06.5714286, +07.5714286),
                Scaled(+07.0, +08.0),
                Scaled(+05.5, +08.0));
        }

        [TestMethod]
        public void BothSelfIntersectingSubjectClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +00.0),
                Scaled(+04.0, +04.0),
                Scaled(+04.0, +14.0),
                Scaled(+07.0, +14.0),
                Scaled(+07.0, +16.0),
                Scaled(+00.0, +16.0),
                Scaled(+00.0, +13.0),
                Scaled(+07.0, +07.0),
                Scaled(+14.0, +14.0),
                Scaled(+11.0, +14.0),
                Scaled(+11.0, +00.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+07.0, +09.0),
                Scaled(+12.0, +09.0),
                Scaled(+07.0, +14.0),
                Scaled(+07.0, +13.0),
                Scaled(+10.0, +13.0),
                Scaled(+10.0, +15.0),
                Scaled(+15.0, +15.0),
                Scaled(+15.0, +13.0),
                Scaled(+11.0, +13.0),
                Scaled(+08.0, +16.0),
                Scaled(+17.0, +16.0),
                Scaled(+17.0, +12.0),
                Scaled(+11.0, +12.0),
                Scaled(+11.0, +11.0),
                Scaled(+12.0, +12.0),
                Scaled(+13.0, +11.0),
                Scaled(+19.0, +11.0),
                Scaled(+19.0, +17.0),
                Scaled(+06.0, +17.0),
                Scaled(+06.0, +12.0),
                Scaled(+07.0, +12.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(4, solution.Count);

            // Order by Y 
            solution = new PolygonPath(solution.OrderBy(poly => poly.Min(point => point.Y)));

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+09.0, +09.0),
                Scaled(+11.0, +09.0),
                Scaled(+11.0, +10.0),
                Scaled(+10.5, +10.5));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+11.0, +11.0),
                Scaled(+12.0, +12.0),
                Scaled(+11.0, +12.0));

            polygon = solution[2];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+11.0, +13.0),
                Scaled(+13.0, +13.0),
                Scaled(+14.0, +14.0),
                Scaled(+11.0, +14.0));

            polygon = solution[3];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+06.0, +14.0),
                Scaled(+07.0, +14.0),
                Scaled(+07.0, +16.0),
                Scaled(+06.0, +16.0));
        }

        [TestMethod]
        public void IntermediateHorizontal1Test()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00000000, +00000000),
                Scaled(+01000000, +01000000),
                Scaled(+03000000, +01000000),
                Scaled(+04000000, +00000000),
                Scaled(+04000000, +02000000),
                Scaled(+00000000, +02000000)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00500000, +00000000),
                Scaled(+03500000, +00000000),
                Scaled(+03500000, +03000000),
                Scaled(+00500000, +03000000)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+00500000, +00500000),
                Scaled(+01000000, +01000000),
                Scaled(+03000000, +01000000),
                Scaled(+03500000, +00500000),
                Scaled(+03500000, +02000000),
                Scaled(+00500000, +02000000));
        }

        [TestMethod]
        public void IntermediateHorizontal2Test()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00000000, +00000000),
                Scaled(+01000000, +01000000),
                Scaled(+03000000, +01000000),
                Scaled(+04000000, +02000000),
                Scaled(+00000000, +02000000)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00500000, +00000000),
                Scaled(+03500000, +00000000),
                Scaled(+03500000, +03000000),
                Scaled(+00500000, +03000000)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+00500000, +00500000),
                Scaled(+01000000, +01000000),
                Scaled(+03000000, +01000000),
                Scaled(+03500000, +01500000),
                Scaled(+03500000, +02000000),
                Scaled(+00500000, +02000000));
        }

        [TestMethod]
        public void MultipleWindTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00, +00),
                Scaled(+06, +00),
                Scaled(+06, +04),
                Scaled(+05, +03),
                Scaled(+04, +04),
                Scaled(+03, +03),
                Scaled(+02, +04),
                Scaled(+01, +03),
                Scaled(+00, +04)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+02, +00),
                Scaled(+04, +00),
                Scaled(+04, +04),
                Scaled(+02, +04)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+02, +00),
                Scaled(+04, +00),
                Scaled(+04, +04),
                Scaled(+03, +03),
                Scaled(+02, +04));
        }

        [TestMethod]
        public void DiamondSquareClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+02.0, +02.0),
                Scaled(+12.0, +02.0),
                Scaled(+12.0, +12.0),
                Scaled(+02.0, +12.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +07.0),
                Scaled(+07.0, +00.0),
                Scaled(+14.0, +07.0),
                Scaled(+07.0, +14.0)
            }));

            var solution = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, solution));

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            AssertEqual(
                polygon,
                Scaled(+05.0, +02.0),
                Scaled(+09.0, +02.0),
                Scaled(+12.0, +05.0),
                Scaled(+12.0, +09.0),
                Scaled(+09.0, +12.0),
                Scaled(+05.0, +12.0),
                Scaled(+02.0, +09.0),
                Scaled(+02.0, +05.0));
        }

        [TestMethod]
        public void ThreeDiamondsClipTest()
        {
            var subject1 = new Polygon(new[]
            {
                Scaled(+05.0, +05.0),
                Scaled(+10.0, +00.0),
                Scaled(+15.0, +05.0),
                Scaled(+10.0, +10.0)
            });

            var subject2 = new Polygon(new[]
            {
                Scaled(+07.0, +05.0),
                Scaled(+10.0, +02.0),
                Scaled(+13.0, +05.0),
                Scaled(+10.0, +08.0)
            });

            var subject3 = new Polygon(new[]
            {
                Scaled(+09.0, +05.0),
                Scaled(+10.0, +04.0),
                Scaled(+11.0, +05.0),
                Scaled(+10.0, +06.0)
            });

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+05.0, +00.0),
                Scaled(+15.0, +00.0),
                Scaled(+15.0, +10.0),
                Scaled(+05.0, +10.0)
            }));

            var subject = new PolygonPath(
                new[]
                {
                    subject1, subject2, subject3
                });

            var tree = new PolygonTree();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));

            var solution = PolygonPath.FromTree(tree);

            Assert.AreEqual(3, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(polygon.Orientation, PolygonOrientation.CounterClockwise);
            AssertEqual(
                polygon,
                Scaled(+10.0, +00.0),
                Scaled(+15.0, +05.0),
                Scaled(+10.0, +10.0),
                Scaled(+05.0, +05.0));

            polygon = solution[1];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(polygon.Orientation, PolygonOrientation.Clockwise);
            AssertEqual(
                polygon,
                Scaled(+10.0, +02.0),
                Scaled(+07.0, +05.0),
                Scaled(+10.0, +08.0),
                Scaled(+13.0, +05.0));

            polygon = solution[2];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(polygon.Orientation, PolygonOrientation.CounterClockwise);
            AssertEqual(
                polygon,
                Scaled(+10.0, +04.0),
                Scaled(+11.0, +05.0),
                Scaled(+10.0, +06.0),
                Scaled(+09.0, +05.0));
        }

        [TestMethod]
        public void BothOpenClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+10.0, +06.0),
                Scaled(+05.0, +06.0),
                Scaled(+00.0, +00.0)
            })
            {
                IsClosed = false
            });

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+00.0, +03.0),
                Scaled(+09.0, +03.0),
                Scaled(+09.0, +05.0)
            }));

            var tree = new PolygonTree();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));

            var solution = PolygonPath.FromTree(tree);

            // The solution should be a polygon with a single line segment.
            Assert.AreEqual(1, solution.Count);
            AssertEqual(
                solution[0],
                Scaled(+3.0681818, +3.6818182),
                Scaled(+2.5, +3.0));
        }

        [TestMethod]
        public void ShareHorizontalRightBoundClipTest()
        {
            var subject = new PolygonPath(new Polygon(new[]
            {
                Scaled(+07.0, +07.0),
                Scaled(+11.0, +00.0),
                Scaled(+11.0, +14.0),
                Scaled(+14.0, +14.0)
            }));

            var clip = new PolygonPath(new Polygon(new[]
            {
                Scaled(+06.0, +01.0),
                Scaled(+15.0, +01.0),
                Scaled(+15.0, +13.0),
                Scaled(+06.0, +13.0)
            }));

            var tree = new PolygonPath();
            Assert.IsTrue(new Clipper.Clipper().Execute(ClipOperation.Intersection, subject, clip, tree));

            var solution = new PolygonPath(tree);

            Assert.AreEqual(1, solution.Count);

            var polygon = solution[0];
            polygon.OrderBottomLeftFirst();
            Assert.AreEqual(polygon.Orientation, PolygonOrientation.CounterClockwise);
            AssertEqual(
                polygon,
                Scaled(+10.42857145, +01.0),
                Scaled(+11.0, +01.0),
                Scaled(+11.0, +11.0),
                Scaled(+13.0, +13.0),
                Scaled(+11.0, +13.0),
                Scaled(+11.0, +11.0),
                Scaled(+07.0, +07.0));
        }

        private static IList<DoublePoint> FromScaledPolygon(Polygon polygon)
        {
            return polygon
                .Select(p => new DoublePoint(
                    p.X * ScaleInverse,
                    p.Y * ScaleInverse))
                .ToList();
        }

        private static PointL Scaled(double x, double y)
        {
            return new PointL((long) (x * Scale), (long) (y * Scale));
        }

        private static void AssertEqual(Polygon polygon, params PointL[] expected)
        {
            Assert.AreEqual(expected.Length, polygon.Count);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(GeometryHelper.NearZero((expected[i] - polygon[i]).Length));
            }
        }
    }
}
