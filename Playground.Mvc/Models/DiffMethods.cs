using System.Collections.Generic;
using System.Text;

namespace Playground.Mvc.Models
{
    public enum DiffStatus
    {
        None,
        Unchanged,
        Deleted,
        Added
    }

    public struct DiffResults2<T> where T : class
    {
        public T SourceItem;

        public DiffStatus SourceItemStatus;

        public T DestinationItem;

        public DiffStatus DestinationItemStatus;

        public DiffResults2(T source, DiffStatus srcStatus, T dest, DiffStatus destStatus)
        {
            SourceItem = source;
            SourceItemStatus = srcStatus;
            DestinationItem = dest;
            DestinationItemStatus = destStatus;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (SourceItemStatus == DiffStatus.Deleted)
            {
                sb.Append('-');
            }

            if (SourceItem == null)
            {
                sb.Append("-------");
            }

            else
            {
                sb.Append(SourceItem);
            }

            sb.Append('\t');
            sb.Append('\t');

            if (DestinationItemStatus == DiffStatus.Added)
            {
                sb.Append('+');
            }

            if (DestinationItem == null)
            {
                sb.Append("-------");
            }
            else
            {
                sb.Append(DestinationItem);
            }
            return sb.ToString();
        }
    }

    public struct DiffResults3<T> where T : class
    {
        public T SourceItem;

        public DiffStatus SourceItemStatus;

        public T DestinationItem1;

        public DiffStatus DestinationItem1Status;

        public T DestinationItem2;

        public DiffStatus DestinationItem2Status;

        public DiffResults3(T source, DiffStatus srcStatus, T dest1, DiffStatus destStatus1, T dest2, DiffStatus destStatus2)
        {
            SourceItem = source;
            SourceItemStatus = srcStatus;
            DestinationItem1 = dest1;
            DestinationItem1Status = destStatus1;
            DestinationItem2 = dest2;
            DestinationItem2Status = destStatus2;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }
    }

    public class DiffMove3
    {
        // ReSharper disable once InconsistentNaming
        public int x;
        // ReSharper disable once InconsistentNaming
        public int y;
        // ReSharper disable once InconsistentNaming
        public int z;
        // ReSharper disable once InconsistentNaming
        public int length;

        public DiffMove3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            length = int.MaxValue;
        }

        public DiffMove3(int x, int y, int z, int length)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.length = length;
        }
    }

    public class DiffMethods
    {
        public static List<DiffResults2<T>> Diff<T>(List<T> list1, List<T> list2) where T : class
        {
            var diffResult = new List<DiffResults2<T>>();

            //First, we have to find the Longest Common Subsequence
            var lcsResult = LongestCommonSubsequence(list1, list2);

            //This is the main loop.  It's similar to the main loop we had discussed before, but it compares both input lists to the LCS result.
            int index1 = 0, index2 = 0, indexLcs = 0;
            while (index1 < list1.Count || index2 < list2.Count)
            {
                //My basic logic here is:
                //If both sides match the LCS, take an element from both sides.
                //Otherwise, if we have an element available on the left, and it doesn't match the LCS, take that element.
                //Otherwise, we must have an element available on the right that doesn't match the LCS, so take that element.

                bool hasElement1 = index1 < list1.Count;
                bool hasElement2 = index2 < list2.Count;
                bool hasElementLcs = indexLcs < lcsResult.Count;

                if (hasElement1 && hasElement2 && hasElementLcs
                    && list1[index1].Equals(lcsResult[indexLcs]) && list2[index2].Equals(lcsResult[indexLcs]))
                {
                    //Both sides match the LCS.  Take one element from each side, and increment all three indices.
                    diffResult.Add(new DiffResults2<T>(list1[index1], DiffStatus.Unchanged, list2[index2], DiffStatus.Unchanged));
                    index1++;
                    index2++;
                    indexLcs++;
                }
                else
                {
                    if (hasElement1 && (!hasElementLcs || !list1[index1].Equals(lcsResult[indexLcs])))
                    {
                        //We have an element on the left, and it doesn't match the LCS.  Take the element on the left, and increase the left index.
                        diffResult.Add(new DiffResults2<T>(list1[index1], DiffStatus.Deleted, null, DiffStatus.None));
                        index1++;
                    }
                    else
                    {
                        //We must have an element on the right that doesn't match the LCS.  Take the element on the right, and increase the right index.
                        diffResult.Add(new DiffResults2<T>(null, DiffStatus.None, list2[index2], DiffStatus.Added));
                        index2++;
                    }
                }
            }

            return diffResult;
        }

        public static List<T> LongestCommonSubsequence<T>(List<T> list1, List<T> list2) where T : class
        {
            //Apologies for the somewhat perverse variable type here.
            //This is a 2-dimensional array, and in each cell, we will store the result of one subproblem.
            //Each result is the longest common subsequence of that subproblem.
            List<T>[,] subproblemResults = new List<T>[list1.Count, list2.Count];

            //I'm going to loop through the 2D array, filling in the answers to subproblems as I go...
            //Note that it's very important that the indexes start at (0,0) and increase from there.
            //Each subproblem with higher indices will rely on the results of subproblems with lower indices.
            for (int index1 = 0; index1 < list1.Count; index1++)
            {
                for (int index2 = 0; index2 < list2.Count; index2++)
                {
                    //In each iteration, we are solving one subproblem.
                    List<T> subproblemResult;

                    if (list1[index1].Equals(list2[index2]))
                    {
                        //If these two elements are equal:
                        //Find what the LCS would be if both sides were one element shorter, and append the current element to the end of that LCS.

                        if (index1 == 0 || index2 == 0)
                        {
                            //In this case, there is not a smaller subproblem we can append our result to, so we'll just start with an empty list.
                            subproblemResult = new List<T>();
                        }
                        else
                        {
                            //Start by getting the answer to the subproblem where each list is one element shorter.
                            //I'm copying it to a new list so we can append to it without affecting the original.
                            subproblemResult = new List<T>(subproblemResults[index1 - 1, index2 - 1]);
                        }
                        //Append the current element.
                        subproblemResult.Add(list1[index1]);
                    }
                    else
                    {
                        //If these two elements are not equal:
                        //Check the two adjacent subproblems, and take whichever result is longer.

                        if (index1 == 0)
                        {
                            if (index2 == 0)
                            {
                                //If there aren't any smaller subproblems available, we'll start with an empty list.
                                subproblemResult = new List<T>();
                            }
                            else
                            {
                                //Only one smaller subproblem is available
                                subproblemResult = subproblemResults[index1, index2 - 1];
                            }
                        }
                        else
                        {
                            if (index2 == 0)
                            {
                                //Only one smaller subproblem is available
                                subproblemResult = subproblemResults[index1 - 1, index2];
                            }
                            else
                            {
                                //Take whichever of the two subproblem results is longer.
                                List<T> subproblem1 = subproblemResults[index1 - 1, index2];
                                List<T> subproblem2 = subproblemResults[index1, index2 - 1];
                                if (subproblem1.Count > subproblem2.Count)
                                {
                                    subproblemResult = subproblem1;
                                }
                                else
                                {
                                    subproblemResult = subproblem2;
                                }
                            }
                        }
                    }

                    //Store the answer to this subproblem in the 2D array, so we can use it in later subproblems.
                    subproblemResults[index1, index2] = subproblemResult;
                }
            }

            //Return the result of the last subproblem
            return subproblemResults[list1.Count - 1, list2.Count - 1];
        }

        public static List<DiffResults3<T>> Diff<T>(List<T> list1, List<T> list2, List<T> list3) where T : class
        {
            //The way I imagine solving this problem is that we are trying to find the shortest path through a 3D grid, and only certain moves are allowed.

            //Every time we move 1 square, that represents adding another row to the DIFF result.
            //If we move straight in one direction, then we add a row in which only one column is not blank.
            //If we move diagonally, in two directions at once, then we add a row in which two columns are not blank.  Both non-blank items must match.
            //If we move diagonally, in all three directions at once, then we add a row in which there are no blanks.  All three items must match.

            //At every square in the 3D grid, we are going to mark down two things:
            //#1: What is the shortest possible distance we could move from here to the end?
            //    I'm storing this in moves[x,y,z].length
            //#2: Which square should we move to from here, in order to follow the shortest path?
            //    I'm storing this in moves[x,y,z].x, moves[x,y,z].y, and moves[x,y,z].z

            //I am actually starting at the end and working backwards.  I do this because I'm going to have to retrace the path later in the reverse direction.
            //That will allow me to add the rows to the DIFF in the correct order, which is a little nicer when using List.Add()

            DiffMove3[,,] moves = new DiffMove3[list1.Count + 1, list2.Count + 1, list3.Count + 1];

            int x, y, z;

            //This set of 3 nested loops is to figure out the optimal path through the 3D grid.
            for (x = list1.Count; x >= 0; x--)
            {
                for (y = list2.Count; y >= 0; y--)
                {
                    for (z = list3.Count; z >= 0; z--)
                    {
                        bool hasX = x < list1.Count;
                        bool hasY = y < list2.Count;
                        bool hasZ = z < list3.Count;
                        T tx = hasX ? list1[x] : null;
                        T ty = hasY ? list2[y] : null;
                        T tz = hasZ ? list3[z] : null;
                        bool xy = hasX && hasY && tx.Equals(ty);
                        bool yz = hasY && hasZ && ty.Equals(tz);
                        bool zx = hasZ && hasX && tz.Equals(tx);

                        //Consider all of the possible moves that could be taken from here.
                        //Only allow moves that don't go off the "board"
                        List<DiffMove3> possibleMoves = new List<DiffMove3>();

                        if (xy && yz)
                        {
                            possibleMoves.Add(new DiffMove3(x + 1, y + 1, z + 1));
                        }
                        if (xy)
                        {
                            possibleMoves.Add(new DiffMove3(x + 1, y + 1, z));
                        }
                        if (yz)
                        {
                            possibleMoves.Add(new DiffMove3(x, y + 1, z + 1));
                        }
                        if (zx)
                        {
                            possibleMoves.Add(new DiffMove3(x + 1, y, z + 1));
                        }
                        if (hasX)
                        {
                            possibleMoves.Add(new DiffMove3(x + 1, y, z));
                        }
                        if (hasY)
                        {
                            possibleMoves.Add(new DiffMove3(x, y + 1, z));
                        }
                        if (hasZ)
                        {
                            possibleMoves.Add(new DiffMove3(x, y, z + 1));
                        }

                        if (possibleMoves.Count == 0)
                        {
                            //We must be in the starting square
                            //Start the length at 0
                            //Have this square move outside the limits for x/y/z, so that the while loop later on will terminate when it reaches this
                            moves[x, y, z] = new DiffMove3(x + 1, y + 1, z + 1, 0);
                        }
                        else
                        {
                            //Figure out which of the possible moves gets us closest to the end, and record that move as the one we want
                            DiffMove3 shortest = null;
                            foreach (DiffMove3 move in possibleMoves)
                            {
                                move.length = moves[move.x, move.y, move.z].length + 1;
                                if (shortest == null || move.length < shortest.length)
                                {
                                    shortest = move;
                                }
                            }
                            moves[x, y, z] = shortest;
                        }
                    }
                }
            }

            //Now we should have the shortest path through that 3D array.
            //Use that to construct the Diff by retracing our steps "backwards" (which is now actually "forwards", since we moved backwards before)
            List<DiffResults3<T>> diffResult = new List<DiffResults3<T>>();
            x = 0;
            y = 0;
            z = 0;
            while (x < list1.Count || y < list2.Count || z < list3.Count)
            {
                int mx = moves[x, y, z].x;
                int my = moves[x, y, z].y;
                int mz = moves[x, y, z].z;

                if (mx > x)
                {
                    if (my > y)
                    {
                        if (mz > z)
                        {
                            //Take x, y, & z
                            diffResult.Add(new DiffResults3<T>(list1[x], DiffStatus.Unchanged, list2[y], DiffStatus.Unchanged, list3[z], DiffStatus.Unchanged));
                        }
                        else
                        {
                            //Take x & y
                            diffResult.Add(new DiffResults3<T>(list1[x], DiffStatus.Deleted, list2[y], DiffStatus.Deleted, null, DiffStatus.None));
                        }
                    }
                    else
                    {
                        if (mz > z)
                        {
                            //Take x & z
                            diffResult.Add(new DiffResults3<T>(list1[x], DiffStatus.Deleted, null, DiffStatus.None, list3[z], DiffStatus.Added));
                        }
                        else
                        {
                            //Take x
                            diffResult.Add(new DiffResults3<T>(list1[x], DiffStatus.Deleted, null, DiffStatus.None, null, DiffStatus.None));
                        }
                    }
                }
                else
                {
                    if (my > y)
                    {
                        if (mz > z)
                        {
                            //Take y & z
                            diffResult.Add(new DiffResults3<T>(null, DiffStatus.None, list2[y], DiffStatus.Added, list3[z], DiffStatus.Added));
                        }
                        else
                        {
                            //Take y
                            diffResult.Add(new DiffResults3<T>(null, DiffStatus.None, list2[y], DiffStatus.Deleted, null, DiffStatus.None));
                        }
                    }
                    else
                    {
                        if (mz > z)
                        {
                            //Take z
                            diffResult.Add(new DiffResults3<T>(null, DiffStatus.None, null, DiffStatus.None, list3[z], DiffStatus.Added));
                        }
                        else
                        {
                            //Impossible!
                        }
                    }
                }

                x = mx;
                y = my;
                z = mz;
            }

            return diffResult;
        }
    }
}