namespace BowlingScoreKeeper.Components.Pages
{
	public class GameAIRefactored : IGame
	{
		// Internal State
		private readonly LinkedList<GameFrame> _frames = new();
		private LinkedListNode<GameFrame> _currentFrameNode;

		// UI Contracts
		public event Action OnStateChanged;
		public List<GameFrame> Frames => _frames.ToList();

		// Calculated Property (No manual state management)
		public int CurrentFrameIndex => _currentFrameNode.Value.FrameIndex - 1;
		public GameFrame CurrentFrame => _currentFrameNode.Value;

		public GameAIRefactored()
		{
			// Initialize 10 frames
			for (int i = 1; i <= 10; i++)
			{
				_frames.AddLast(new GameFrame { FrameIndex = i });
			}
			// Point to the head
			_currentFrameNode = _frames.First!;
		}

		public void AddRoll(int pinsKnocked)
		{
			// 1. Guard: Don't allow rolls if game is over
			if (IsGameComplete()) return;

			// 2. Try to add the roll (Handles logic for 10th frame extra rolls)
			bool rollAccepted = TryRecordRoll(_currentFrameNode.Value, pinsKnocked);

			if (!rollAccepted) return;

			// 3. Update State
			RecalculateScores();
			UpdateCursorPosition();
			OnStateChanged?.Invoke();
		}

		private bool TryRecordRoll(GameFrame frame, int pins)
		{
			if (frame.FirstRoll == null)
			{
				frame.FirstRoll = pins;
				return true;
			}

			if (frame.SecondRoll == null)
			{
				frame.SecondRoll = pins;
				return true;
			}

			if (frame.ThirdRoll == null && frame.FrameIndex == 10)
			{
				// Rule: You only get a 3rd roll if you Struck or Spared
				if (frame.IsStrike || frame.IsSpare)
				{
					frame.ThirdRoll = pins;
					return true;
				}
			}

			return false;
		}

		private void UpdateCursorPosition()
		{
			// Never move past the last frame
			if (_currentFrameNode.Next == null) return;

			var frame = CurrentFrame;

			// Move if Strike (and not 10th) OR if frame is full (2 rolls)
			if (frame.IsStrike || frame.SecondRoll != null)
			{
				_currentFrameNode = _currentFrameNode.Next!;
			}
		}

		private bool IsGameComplete()
		{
			var lastFrame = _frames.Last!.Value;

			// If Strike/Spare, we need 3 rolls. Otherwise, we need 2.
			if (lastFrame.IsStrike || lastFrame.IsSpare)
				return lastFrame.ThirdRoll != null;

			return lastFrame.SecondRoll != null;
		}

		// --- SCORING ENGINE ---

		private void RecalculateScores()
		{
			int runningTotal = 0;
			bool canCalculate = true;

			var runner = _frames.First;

			while (runner != null)
			{
				var frame = runner.Value;

				// Optimization: If a previous frame was incomplete, we can't calculate future ones
				if (!canCalculate)
				{
					frame.Score = null;
					runner = runner.Next;
					continue;
				}

				int? framePoints = GetFramePoints(runner);

				if (framePoints != null)
				{
					runningTotal += framePoints.Value;
					frame.Score = runningTotal;
				}
				else
				{
					frame.Score = null;
					canCalculate = false;
				}

				runner = runner.Next;
			}
		}

		private int? GetFramePoints(LinkedListNode<GameFrame> node)
		{
			var frame = node.Value;

			if (frame.FrameIndex == 10)
			{
				return ScoreTenthFrame(frame);
			}

			if (frame.IsStrike)
			{
				return ScoreStrike(node);
			}

			if (frame.IsSpare)
			{
				return ScoreSpare(node);
			}

			return ScoreNormalFrame(frame);
		}

		private int? ScoreNormalFrame(GameFrame frame)
		{
			if (frame.FirstRoll != null && frame.SecondRoll != null)
			{
				return frame.FirstRoll.Value + frame.SecondRoll.Value;
			}
			return null;
		}

		private int? ScoreSpare(LinkedListNode<GameFrame> currentNode)
		{
			// Logic: 10 + Next Ball
			var nextNode = currentNode.Next;
			if (nextNode != null && nextNode.Value.FirstRoll != null)
			{
				return 10 + nextNode.Value.FirstRoll.Value;
			}
			return null;
		}

		private int? ScoreStrike(LinkedListNode<GameFrame> currentNode)
		{
			// Logic: 10 + Next 2 Balls
			var nextNode = currentNode.Next;

			// Cannot calculate if next frame doesn't exist
			if (nextNode == null || nextNode.Value.FirstRoll == null) return null;

			// Scenario A: Strike then Strike (Need to look 2 frames ahead)
			if (nextNode.Value.IsStrike && nextNode.Value.FrameIndex != 10)
			{
				var twoNodesDown = nextNode.Next;
				if (twoNodesDown != null && twoNodesDown.Value.FirstRoll != null)
				{
					return 10 + 10 + twoNodesDown.Value.FirstRoll.Value;
				}
				return null;
			}

			// Scenario B: Strike then Normal (or Strike then Frame 10)
			// We need the next frame's 1st AND 2nd roll
			if (nextNode.Value.SecondRoll != null)
			{
				return 10 + nextNode.Value.FirstRoll.Value + nextNode.Value.SecondRoll.Value;
			}

			return null;
		}

		private int? ScoreTenthFrame(GameFrame frame)
		{
			// Logic: Sum of all rolls if the frame is "complete"

			if (frame.IsStrike || frame.IsSpare)
			{
				// Must have 3 rolls
				if (frame.ThirdRoll != null)
					return frame.FirstRoll!.Value + frame.SecondRoll!.Value + frame.ThirdRoll.Value;
			}
			else
			{
				// Must have 2 rolls
				if (frame.SecondRoll != null)
					return frame.FirstRoll!.Value + frame.SecondRoll.Value;
			}

			return null;
		}
	}
}