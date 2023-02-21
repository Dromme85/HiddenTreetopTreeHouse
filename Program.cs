
namespace HiddenTreetopTreeHouse
{
	static class Program
	{
		public static GridHandler gh { get; private set; }

		static void Main()
		{
			gh = new("../../../treegrid.txt");

			PrintResult();
		}

		/// <summary>
		/// Simple visualization of the tree grid,
		/// displaying the height of visible trees
		/// and hiding trees that's not visible.
		/// </summary>
		static void PrintResult()
		{
			int visibleTrees = gh.CalculateVisibleTrees();
			gh.CalculateScenicScores();
			long highestScenicScore = 1;
			int[] highScorePos = new int[2];

			for (int y = 0; y < gh.Grid.GetLength(0); y++)
			{
				for (int x = 0; x < gh.Grid.GetLength(1); x++)
				{
					if (gh.Grid[x, y].Visible)
					{
						Console.Write(gh.Grid[x, y].Data + " ");
					}
					else Console.Write("  ");

					if (gh.Grid[x, y].ScenicScore > highestScenicScore)
					{
						highestScenicScore = gh.Grid[x, y].ScenicScore;
						highScorePos = new int[2] { x, y };
					}
				}
				Console.WriteLine();
			}
			var s = gh.Grid[highScorePos[0], highScorePos[1]].Scores;

			Console.WriteLine($"\nThere are {visibleTrees} visible trees out of {gh.Grid.Length}.");
			Console.WriteLine($"\nThe spot with the highest score is {highestScenicScore} ({s[0]} * {s[1]} * {s[2]} * {s[3]})" +
				$" and can be found at {highScorePos[0]}, {highScorePos[1]}.");
		}
	}

}