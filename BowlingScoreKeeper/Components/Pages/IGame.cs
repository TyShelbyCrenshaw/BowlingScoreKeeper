namespace BowlingScoreKeeper.Components.Pages
{
	public interface IGame
	{
		List<GameFrame> Frames { get; }
		int CurrentFrameIndex { get; }
		GameFrame CurrentFrame { get; }

		event Action OnStateChanged;

		void AddRoll(int pinsKnocked);
	}
}
