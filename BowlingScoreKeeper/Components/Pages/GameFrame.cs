namespace BowlingScoreKeeper.Components.Pages
{
	public class GameFrame
	{
		public int FrameIndex { get; set; }
		public int? FirstRoll { get; set; }
		public int? SecondRoll { get; set; }
		public int? ThirdRoll { get; set; }
		public int? Score { get; set; }

		public bool IsStrike => FirstRoll == 10;
		public bool IsSpare => !IsStrike && (FirstRoll + SecondRoll == 10);
	}
}