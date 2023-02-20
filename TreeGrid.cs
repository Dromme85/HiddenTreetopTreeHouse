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
	[StructLayout(LayoutKind.Sequential)]
	struct TreeGrid
	{
		public byte Data;
		public bool Visible;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public bool[] Direction;

		public TreeGrid()
		{
			Data = 0;
			Visible = false;
			Direction = new bool[4];
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
				// if any errors, assign some test data.
				gs = new string[] { "30373", "25512", "65332", "33549", "35390" };
			}

			Grid = new TreeGrid[gs[0].Length, gs.Length];
			for (int x = 0; x < gs.Length; x++)
			{
				for (int y = 0; y < gs[x].Length; y++)
				{
					Grid[x, y] = new TreeGrid();
				}
			}

			for (int x = 0; x < gs.Length; x++)
			{
				for (int y = 0; y < gs[x].Length; y++)
				{
					byte.TryParse(gs[x][y].ToString(), out byte d);
					Grid[x, y].Data = d;
					if (x == 0 || x == gs.Length - 1 || y == 0 || y == gs[gs.Length - 1].Length - 1)
					{
						Grid[x, y].Visible = true;
						if (x == 0) Grid[x, y].Direction[0] = true;
						if (x == gs.Length - 1) Grid[x, y].Direction[2] = true;
						if (y == 0) Grid[x, y].Direction[3] = true;
						if (y == gs[gs.Length - 1].Length - 1) Grid[x, y].Direction[1] = true;
					}
				}
			}
		}

		public void PrintResult()
		{
			var visibleTrees = CalculateVisibleTrees();

			for (int x = 0; x < Grid.GetLength(0); x++)
			{
				for (int y = 0; y < Grid.GetLength(1); y++)
				{
					if (Grid[x, y].Visible)
						Console.Write(Grid[x, y].Data + " ");
					else Console.Write("  ");
				}
				Console.WriteLine();
			}

			Console.WriteLine($"There are {visibleTrees} visible trees out of {Grid.Length}.");
		}

		public int CalculateVisibleTrees()
		{
			int val = Grid.GetLength(0) * 2 + Grid.GetLength(1) * 2 - 4;

			for (int x = 1; x < Grid.GetLength(0) - 1; x++)
			{
				for (int y = 1; y < Grid.GetLength(1) - 1; y++)
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

		private bool CheckDirection(int d, int x, int y, int o = 0)
		{
			o++;

			switch (d)
			{
				case 0: // Up
					if (Grid[x, y].Data > Grid[x - o, y].Data)
					{
						if (x - o == 0) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;
				case 1: // Right
					if (Grid[x, y].Data > Grid[x, y + o].Data)
					{
						if (y + o == Grid.GetLength(1) - 1) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;
				case 2: // Down
					if (Grid[x, y].Data > Grid[x + o, y].Data)
					{
						if (x + o == Grid.GetLength(0) - 1) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;
				case 3: // Left
					if (Grid[x, y].Data > Grid[x, y - o].Data)
					{
						if (y - o == 0) return true;
						return CheckDirection(d, x, y, o);
					}
					return false;
				default: return false;
			}
		}
	}
}
