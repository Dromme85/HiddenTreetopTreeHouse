using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace HiddenTreetopTreeHouse
{
	// The extra attributes is a workaround to be able to declare size of an array
	// https://stackoverflow.com/questions/4123314/how-i-can-declare-arrays-in-struct
	[StructLayout(LayoutKind.Sequential)]
	struct TreeGrid
	{
		public byte Data;
		public bool Visible;
		public long ScenicScore;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public bool[] Direction;
		public int[] Scores;

		public TreeGrid()
		{
			Data = 0;
			Visible = false;
			ScenicScore = 0;
			Direction = new bool[4];
			Scores = new int[4];
		}
	}

	internal class GridHandler
	{
		public TreeGrid[,] Grid;

		public GridHandler(string fileName)
		{
			string[] gs;

			try
			{
				gs = File.ReadAllLines(fileName);
			}
			catch
			{
				// If any errors while reading the file, use some test data.
				gs = new string[]
				{
					"30373", 
					"25512",
					"65332", 
					"33549", 
					"35390" 
				};
			}

			Grid = new TreeGrid[gs[0].Length, gs.Length];
			for (int y = 0; y < gs.Length; y++)
			{
				for (int x = 0; x < gs[y].Length; x++)
				{
					Grid[x, y] = new TreeGrid();
				}
			}

			for (int y = 0; y < gs.Length; y++)
			{
				for (int x = 0; x < gs[y].Length; x++)
				{
					byte.TryParse(gs[y][x].ToString(), out byte d);
					Grid[x, y].Data = d;

					// We already know the edge is visible,
					// lets set it to visible and what direction that is visible
					if (y == 0 || y == gs.Length - 1 || x == 0 || x == gs[gs.Length - 1].Length - 1)
					{
						Grid[x, y].Visible = true;
						if (y == 0) Grid[x, y].Direction[0] = true;
						if (y == gs.Length - 1) Grid[x, y].Direction[2] = true;
						if (x == 0) Grid[x, y].Direction[3] = true;
						if (x == gs[gs.Length - 1].Length - 1) Grid[x, y].Direction[1] = true;
					}
				}
			}
		}

		/// <summary>
		/// Loops through the grid and checks visibility of the trees.
		/// </summary>
		/// <returns>The number of trees visible</returns>
		public int CalculateVisibleTrees()
		{
			int val = Grid.GetLength(0) * 2 + Grid.GetLength(1) * 2 - 4;

			for (int y = 1; y < Grid.GetLength(0) - 1; y++)
			{
				for (int x = 1; x < Grid.GetLength(1) - 1; x++)
				{
					Grid[x, y].Direction[0] = CheckDirection(0, x, y);
					Grid[x, y].Direction[1] = CheckDirection(1, x, y);
					Grid[x, y].Direction[2] = CheckDirection(2, x, y);
					Grid[x, y].Direction[3] = CheckDirection(3, x, y);

					if (Grid[x, y].Direction.Any(d => d))
					{
						Grid[x, y].Visible = true;
						val++;
					}
				}
			}

			return val;
		}

		/// <summary>
		/// Recursive method to check visibility of a tree in a given direction.
		/// </summary>
		/// <param name="d">Direction (0 = up, 1 = right, 2 = down, 3 = left)</param>
		/// <param name="x">Horisontal position</param>
		/// <param name="y">Vertical position</param>
		/// <param name="o">Offset position from x, y</param>
		/// <returns>True if tree on x, y is visible</returns>
		private bool CheckDirection(int d, int x, int y, int o = 0)
		{
			o++;

			switch (d)
			{
				case 0: // Up
					if (Grid[x, y].Data > Grid[x, y - o].Data)
					{
						if (y - o == 0) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;

				case 1: // Right
					if (Grid[x, y].Data > Grid[x + o, y].Data)
					{
						if (x + o == Grid.GetLength(1) - 1) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;

				case 2: // Down
					if (Grid[x, y].Data > Grid[x, y + o].Data)
					{
						if (y + o == Grid.GetLength(0) - 1) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;

				case 3: // Left
					if (Grid[x, y].Data > Grid[x - o, y].Data)
					{
						if (x - o == 0) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;

				default: return false;
			}
		}

		/// <summary>
		/// Loops through the grid and calculates the scenic score of the trees.
		/// </summary>
		public void CalculateScenicScores()
		{
			// Lets assume the edge will not have the highest scenic score and skip those trees.
			for (int y = 1; y < Grid.GetLength(0) - 1; y++)
			{
				for (int x = 1; x < Grid.GetLength(1) - 1; x++)
				{
					// Only visible trees need to be calculated.
					if (Grid[x, y].Visible)
					{
						Grid[x, y].Scores[0] = CheckDirectionalScore(0, x, y);
						Grid[x, y].Scores[1] = CheckDirectionalScore(1, x, y);
						Grid[x, y].Scores[2] = CheckDirectionalScore(2, x, y);
						Grid[x, y].Scores[3] = CheckDirectionalScore(3, x, y);

						long product = 1;
						foreach (int value in Grid[x, y].Scores)
						{
							if (value > 0) product *= value;
						}

						Grid[x, y].ScenicScore = product;
					}
				}
			}
		}

		/// <summary>
		/// Recursive method to calculate the scenic score of a tree in a given direction.
		/// </summary>
		/// <param name="d">Direction (0 = up, 1 = right, 2 = down, 3 = left)</param>
		/// <param name="x">Horisontal position</param>
		/// <param name="y">Vertical position</param>
		/// <param name="o">Offset position from x, y</param>
		/// <returns>The score of the given direction</returns>
		private int CheckDirectionalScore(int d, int x, int y, int o = 0)
		{
			o++;

			switch (d)
			{
				case 0: // Up
					Grid[x, y].Scores[d]++;
					if (Grid[x, y].Data > Grid[x, y - o].Data
						&& y - o > 0)
						CheckDirectionalScore(d, x, y, o);
					return Grid[x, y].Scores[d];

				case 1: // Right
					Grid[x, y].Scores[d]++;
					if (Grid[x, y].Data > Grid[x + o, y].Data
						&& x + o < Grid.GetLength(0) - 1)
						return CheckDirectionalScore(d, x, y, o);
					return Grid[x, y].Scores[d];

				case 2: // Down
					Grid[x, y].Scores[d]++;
					if (Grid[x, y].Data > Grid[x, y + o].Data
						&& y + o < Grid.GetLength(1) - 1)
						return CheckDirectionalScore(d, x, y, o);
					return Grid[x, y].Scores[d];

				case 3: // Left
					Grid[x, y].Scores[d]++;
					if (Grid[x, y].Data > Grid[x - o, y].Data
						&& x - o > 0)
						return CheckDirectionalScore(d, x, y, o);
					return Grid[x, y].Scores[d];

				default: return 0;
			}
		}
	}
}
