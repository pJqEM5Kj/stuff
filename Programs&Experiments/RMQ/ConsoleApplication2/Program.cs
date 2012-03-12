using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            int IntervalsCount = 211000;
            int ArrayLength = 1000000;

            var array = new int[ArrayLength];

            var rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < ArrayLength; i++)
            {
                array[i] = rnd.Next(int.MaxValue / 2);
            }

            // test array
            //array = new int[] { 3, 8, 6, 4, 2, 5, 9, 0, 7, 1, };
            //ArrayLength = array.Length;

            var intervals = new Tuple<int, int>[IntervalsCount];
            for (int i = 0; i < IntervalsCount; i++)
            {
                int l = rnd.Next(0, ArrayLength);
                int r = rnd.Next(l, ArrayLength);

                intervals[i] = new Tuple<int, int>(l, r);
            }

            intervals[0] = new Tuple<int, int>(0, array.Length);

            var algos = new List<RMQ>
            {
                //new RMQ_DirectCalc(),
                new RMQ_SparseTable(),
                new RMQ_SegmentTree() { FromTop = true, },
                new RMQ_SegmentTree(),
            };

            var results = new List<int[]>();

            foreach (RMQ algo in algos)
            {
                var res = new int[IntervalsCount];
                Measure(algo, array, intervals, res);
                results.Add(res);

                Console.WriteLine();
                Console.WriteLine();
            }

            for (int i = 1; i < results.Count; i++)
            {
                if (!results[i].SequenceEqual(results[0]))
                {
                    Console.WriteLine("wrong result from : " + algos[i].ToString());

                    // for debug
                    //for(int j = 0; j < IntervalsCount; j++)
                    //{
                    //    if (results[i][j] != results[0][j])
                    //    {
                    //        int goodRes = results[0][j];
                    //        int badRes = algos[i].Execute(array, intervals[j].Item1, intervals[j].Item2);
                    //    }
                    //}
                }
            }
        }

        static void Measure(RMQ algo, int[] array, Tuple<int, int>[] intervals, int[] results)
        {
            TimeSpan prepareTime;

            var sw = Stopwatch.StartNew();
            algo.Prepare(array);
            prepareTime = sw.Elapsed;
            for (int i = 0; i < intervals.Length; i++)
            {
                results[i] = algo.Execute(array, intervals[i].Item1, intervals[i].Item2);
            }
            sw.Stop();

            Console.WriteLine("algo: " + algo.ToString());
            Console.WriteLine("prepare time: " + prepareTime.ToString());
            Console.WriteLine("all time: " + sw.Elapsed.ToString());
        }
    }


    abstract class RMQ
    {
        public virtual void Prepare(int[] array)
        {
            // nop
        }

        public abstract int Execute(int[] array, int l, int r);
    }

    class RMQ_DirectCalc : RMQ
    {
        public override int Execute(int[] array, int l, int r)
        {
            int min = array[l];

            for (int i = l + 1; i < r; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                }
            }

            return min;
        }

        public override string ToString()
        {
            return "RMQ_DirectCalc";
        }
    }

    class RMQ_SparseTable : RMQ
    {
        private int[][] PreparedInfo;


        public override void Prepare(int[] array)
        {
            int logL = (int)Math.Log(array.Length, 2) + 1;
            var preparedInfo = new int[logL][];

            preparedInfo[0] = array;

            for (int k = 1; k < logL; k++)
            {
                int n = (int)Math.Pow(2, k);

                int p = array.Length - n + 1;
                preparedInfo[k] = new int[p];

                int nn = (int)Math.Pow(2, k - 1);

                for (int i = 0; i < p; i++)
                {
                    preparedInfo[k][i] = Math.Min(preparedInfo[k - 1][i], preparedInfo[k - 1][i + nn]);
                }
            }

            PreparedInfo = preparedInfo;
        }

        public override int Execute(int[] array, int l, int r)
        {
            int length = r - l;
            if (length < 2)
            {
                return array[l];
            }

            int logL = (int)Math.Log(length, 2);
            int L = (int)Math.Pow(2, logL);

            int min1 = PreparedInfo[logL][l];
            int min2 = PreparedInfo[logL][r - L];

            return Math.Min(min1, min2);
        }

        public override string ToString()
        {
            return "RMQ_SparseTable";
        }
    }

    class RMQ_SegmentTree : RMQ
    {
        public bool FromTop;

        private Node[] PreparedInfo;


        public override void Prepare(int[] array)
        {
            int logL = (int)Math.Ceiling(Math.Log(array.Length, 2));
            int L = (int)Math.Pow(2, logL);
            var preparedInfo = new Node[2 * L - 1];

            for (int i = L - 1; i < preparedInfo.Length; i++)
            {
                int indx = i - (L - 1);

                preparedInfo[i] = new Node()
                {
                    IL = indx,
                    IR = indx + 1,
                    Min = (indx < array.Length) ? array[indx] : int.MaxValue,
                };
            }

            for (int i = preparedInfo.Length - 1; i > 0; i -= 2)
            {
                int indx = (i - 1) / 2;

                Node Rt = preparedInfo[i];
                Node Lf = preparedInfo[i - 1];

                preparedInfo[indx] = new Node() { IL = Lf.IL, IR = Rt.IR, Min = Math.Min(Lf.Min, Rt.Min), };
            }

            PreparedInfo = preparedInfo;
        }

        public override int Execute(int[] array, int l, int r)
        {
            int length = r - l;
            if (length < 2)
            {
                return array[l];
            }

            if (FromTop)
            {
                return Execute_FromTop(array, l, r);
            }
            else
            {
                return Execute_FromDown(array, l, r);
            }
        }

        public override string ToString()
        {
            return string.Format("RMQ_SegmentTree (FromTop = {0})", FromTop);
        }

        private int Execute_FromTop(int[] array, int l, int r)
        {
            var mins = new List<int>();

            var nodesIndxStack = new Stack<int>();
            nodesIndxStack.Push(0);

            while (nodesIndxStack.Any())
            {
                int nodeIndx = nodesIndxStack.Pop();
                Node node = PreparedInfo[nodeIndx];

                if (node.IL >= l && node.IR <= r)
                {
                    // node entirely inside segment
                    mins.Add(node.Min);
                    continue;
                }

                if (r <= node.IL || l >= node.IR)
                {
                    // node does not intersect with segment
                    continue;
                }

                // node intersects with segment
                int nodeIndx_L = 2 * nodeIndx + 1;
                int nodeIndx_R = 2 * nodeIndx + 2;
                nodesIndxStack.Push(nodeIndx_L);
                nodesIndxStack.Push(nodeIndx_R);
            }

            return mins.Min();
        }

        private int Execute_FromDown(int[] array, int l, int r)
        {
            var mins = new List<int>();

            int logL = (int)Math.Ceiling(Math.Log(array.Length, 2));
            int L = (int)Math.Pow(2, logL);

            int il = L - 1 + l;
            int ir = L - 1 + r - 1;

            while (il <= ir)
            {
                Node nl = PreparedInfo[il];
                Node nr = PreparedInfo[ir];

                if (il == 0 || ir == 0)
                {
                    return PreparedInfo[0].Min;
                }

                if (il % 2 == 0)
                {
                    mins.Add(nl.Min);
                    il++;
                }

                il = il / 2;

                if (ir % 2 == 1)
                {
                    mins.Add(nr.Min);
                    ir--;
                }

                ir = (ir - 1) / 2;
            }

            return mins.Min();
        }


        class Node
        {
            public int IL;
            public int IR;
            public int Min;

            public override string ToString()
            {
                return string.Format("L: {0}   R: {1}   Min: {2}", IL, IR, Min);
            }
        }
    }
}
