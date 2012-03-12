using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PointInPolygon
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<PolygonTest> tests = GetTests();

            int passTests = 0;
            int failedTests = 0;

            int indx = -1;

            foreach (PolygonTest test in tests)
            {
                indx++;

                test.TestResult = Convert(PolygonHelper.Contains(test.testPolygon, test.testPoint, test.testEps));

                if (test.TestResult != test.RightResult)
                {
                    PolygonHelper.Contains(test.testPolygon, test.testPoint, test.testEps);

                    failedTests++;
                    Console.WriteLine(string.Format("Test {0} failed.", indx + 1));
                }
                else
                {
                    passTests++;
                }
            }

            int testCount = indx + 1;

            if (failedTests == 0)
            {
                Console.WriteLine("All tests passed");
            }
            else
            {
                Console.WriteLine(string.Format("Failed test: {0} / {1}", failedTests, testCount));
            }
        }

        private static IEnumerable<PolygonTest> GetTests()
        {
            Point defaultPt = new Point(1, 1.3);
            Point defaultPt2 = defaultPt;

            double defaultEps = 10E-10;

            // 1)
            yield return new PolygonTest()
                {
                    testEps = defaultEps,
                    testPoint = defaultPt2,
                    RightResult = -1,
                    testPolygon = new List<Point>()
                        {
                        },
                };


            // 2)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = 0,
                testPolygon = new List<Point>() 
                    {
                        defaultPt,
                    },
            };


            // 3)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X, defaultPt.Y + 0.01),
                    },
            };


            // 4)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y + 4),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 4, defaultPt.Y),
                        new Point(defaultPt.X + 5, defaultPt.Y),
                        new Point(defaultPt.X + 6, defaultPt.Y),
                    },
            };


            // 5)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 4, defaultPt.Y),
                        new Point(defaultPt.X + 5, defaultPt.Y),
                    },
            };


            // 6)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = 0,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 4, defaultPt.Y),
                        new Point(defaultPt.X - 5, defaultPt.Y),
                    },
            };


            // 7)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y + 1),
                        new Point(defaultPt.X + 3, defaultPt.Y + 1),
                        new Point(defaultPt.X + 4, defaultPt.Y + 1),
                        new Point(defaultPt.X - 5, defaultPt.Y + 1),
                    },
            };


            // 8)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y - 1),
                        new Point(defaultPt.X + 3, defaultPt.Y - 1),
                        new Point(defaultPt.X + 4, defaultPt.Y - 1),
                        new Point(defaultPt.X - 5, defaultPt.Y - 1),
                    },
            };


            // 9)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 4, defaultPt.Y),
                        new Point(defaultPt.X + 5, defaultPt.Y + 0.5),
                        new Point(defaultPt.X + 1, defaultPt.Y + 2),
                    },
            };


            // 10)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 1, defaultPt.Y + 2),
                        new Point(defaultPt.X + 2, defaultPt.Y),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 4, defaultPt.Y),
                        new Point(defaultPt.X + 5, defaultPt.Y + 0.5),
                    },
            };


            // 11)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y + 2),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X + 2, defaultPt.Y - 5),
                    },
            };


            // 12)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = 1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X - 1, defaultPt.Y + 5),
                        new Point(defaultPt.X + 3, defaultPt.Y),
                        new Point(defaultPt.X - 1, defaultPt.Y - 5),
                    },
            };


            // 13)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y + 5),
                        new Point(defaultPt.X + 2, defaultPt.Y + 1),
                        new Point(defaultPt.X + 2, defaultPt.Y - 1),
                        new Point(defaultPt.X + 2, defaultPt.Y - 5),
                    },
            };


            // 14)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X + 2, defaultPt.Y + 5),
                        new Point(defaultPt.X + 2, defaultPt.Y),
                        new Point(defaultPt.X + 2, defaultPt.Y - 5),
                    },
            };


            // 15)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X - 2, defaultPt.Y + 5),
                        new Point(defaultPt.X - 2, defaultPt.Y + 1),
                        new Point(defaultPt.X - 2, defaultPt.Y - 5),
                    },
            };


            // 16)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = -1,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X - 2, defaultPt.Y + 5),
                        new Point(defaultPt.X - 2, defaultPt.Y),
                        new Point(defaultPt.X - 2, defaultPt.Y - 5),
                    },
            };


            // 17)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = 0,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X, defaultPt.Y + 5),
                        new Point(defaultPt.X, defaultPt.Y + 1),
                        new Point(defaultPt.X, defaultPt.Y - 5),
                    },
            };


            // 18)
            yield return new PolygonTest()
            {
                testEps = defaultEps,
                testPoint = defaultPt2,
                RightResult = 0,
                testPolygon = new List<Point>() 
                    {
                        new Point(defaultPt.X, defaultPt.Y + 5),
                        new Point(defaultPt.X, defaultPt.Y),
                        new Point(defaultPt.X, defaultPt.Y - 5),
                    },
            };
        }

        private static int Convert(PointInPolygonResult obj)
        {
            switch (obj)
            {
                case PointInPolygonResult.Outside:
                    return -1;

                case PointInPolygonResult.BorderPoint:
                case PointInPolygonResult.Border:
                    return 0;

                case PointInPolygonResult.Inside:
                    return 1;

                default:
                    throw new InvalidOperationException();
            }
        }

        private class PolygonTest
        {
            public IList<Point> testPolygon;
            public Point testPoint;
            public double testEps = 0;
            public int RightResult;
            public PointInPolygonResult RightResult2;
            public int TestResult;
        }
    }

    public enum PointInPolygonResult
    {
        Outside = 0,
        BorderPoint = 1,
        Border = 2,
        Inside = 3,
    }

    public class PolygonHelper
    {
        public static PointInPolygonResult Contains(IEnumerable<Point> polygon, Point p, double eps)
        {
            if (polygon == null)
            {
                throw new ArgumentNullException("polygon");
            }

            if (eps < 0)
            {
                throw new ArgumentOutOfRangeException("eps");
            }


            Point p0 = new Point();
            Point p1 = new Point();

            bool? top = null;
            bool eqL = false;
            bool eqR = false;

            int intersections = 0;

            foreach (Point pt in GetPolygonPointsLoopedBack(polygon, p.Y, eps))
            {
                double dX = p.X - pt.X;
                double dY = p.Y - pt.Y;


                #region Math.Abs(dY) < eps

                if (Math.Abs(dY) <= eps)
                {
                    if (Math.Abs(dX) <= eps)
                    {
                        // points are equal
                        return PointInPolygonResult.BorderPoint;
                    }

                    if (dX < 0)
                    {
                        // current point - right point with the same Y

                        if (eqL)
                        {
                            // previously there was left point with the same Y
                            return PointInPolygonResult.Border;
                        }

                        eqR = true;
                    }
                    else
                    {
                        // current point - left point with the same Y

                        if (eqR)
                        {
                            // previously there was right point with the same Y
                            return PointInPolygonResult.Border;
                        }

                        eqL = true;
                    }

                    continue;
                }

                #endregion


                if (!top.HasValue)
                {
                    top = (dY < 0);
                    p0 = p1 = pt;
                    continue;
                }

                p0 = p1;
                p1 = pt;

                if (DistanceToSegment(p0, p1, p) < eps)
                {
                    // point is on the border
                    return PointInPolygonResult.Border;
                }


                #region Main part

                if (top.Value)
                {
                    // we go from top
                    if (dY < 0)
                    {
                        // when p1 is higher - no intersection
                        // reset equal flags
                        eqL = eqR = false;
                        continue;
                    }
                    else
                    {
                        // p1 lower
                        top = false;
                    }
                }
                else
                {
                    // we go from bottom
                    if (dY > 0)
                    {
                        // when p1 lower - no intersection
                        // reset equal flags
                        eqL = eqR = false;
                        continue;
                    }
                    else
                    {
                        // p1 higher
                        top = true;
                    }
                }

                if (eqL)
                {
                    eqL = false;
                    continue;
                }

                if (eqR)
                {
                    eqR = false;
                    intersections++;
                    continue;
                }

                double x = GetCrossX(p0, p1, p);
                if (x > p.X)
                {
                    intersections++;
                }

                #endregion
            }

            if (intersections % 2 == 1)
            {
                return PointInPolygonResult.Inside;
            }
            else
            {
                return PointInPolygonResult.Outside;
            }
        }

        public static IEnumerable<Point> GetPolygonPointsLoopedBack(IEnumerable<Point> polygon, double y, double eps)
        {
            Point? firstPoint = null;
            Point? firstTopBottomPoint = null;

            foreach (Point p in polygon)
            {
                if (firstPoint == null)
                {
                    firstPoint = p;
                }

                if (firstTopBottomPoint == null && Math.Abs(y - p.Y) > eps)
                {
                    firstTopBottomPoint = p;
                }

                yield return p;
            }

            if (firstPoint == null)
            {
                yield break;
            }

            yield return firstPoint.Value;

            if (firstTopBottomPoint == null)
            {
                yield break;
            }

            double dist = (firstPoint.Value - firstTopBottomPoint.Value).Length;
            if (dist < eps)
            {
                yield break;
            }

            yield return firstTopBottomPoint.Value;
        }

        private static double DistanceToSegment(Point p0, Point p1, Point p)
        {
            Vector v = p1 - p0;
            Vector w = p - p0;

            double dotProduct = v.X * w.X + v.Y * w.Y;
            if (dotProduct <= 0)
            {
                return (p - p0).Length;
            }

            double squaredLength = v.X * v.X + v.Y * v.Y;
            if (dotProduct >= squaredLength)
            {
                return (p - p1).Length;
            }

            double progress = dotProduct / squaredLength;
            Point tmpP = p0 + progress * v;
            return (p - tmpP).Length;
        }

        private static double GetCrossX(Point p0, Point p1, Point p)
        {
            double p1Y_Shifted = p1.Y - p.Y;
            double p0Y_Shifted = p0.Y - p.Y;

            double C = (p0Y_Shifted * p1.X) - (p0.X * p1Y_Shifted);
            double A = p1Y_Shifted - p0Y_Shifted;
            double crossX = -C / A;
            return crossX;
        }
    }
}
