
namespace BowlingScoreKeeper.Components.Pages
{
	public class GameRefactored : IGame
	{
		private readonly LinkedList<GameFrame> _frames = new();

		private LinkedListNode<GameFrame> _currentFrameNode;

		public event Action OnStateChanged;

		//ui expects a list so we convert it for the frontend
		public List<GameFrame> Frames => _frames.ToList();

		public int CurrentFrameIndex => _currentFrameNode.Value.FrameIndex - 1;

		public GameFrame CurrentFrame => _currentFrameNode.Value;

		public GameRefactored()
		{
			for(int i = 0; i < 10; i++)
			{
				_currentFrameNode = new LinkedListNode<GameFrame>(new GameFrame(){ FrameIndex = i + 1 });
				_frames.AddLast(_currentFrameNode);
			}
			_currentFrameNode = _frames.First;
		}

		public void AddRoll(int pinsKnocked)
		{
			//Is the game over?
			if (_currentFrameNode == null || IsGameComplete())
			{
				return;
			}

			GameFrame frame = _currentFrameNode.Value;
			if(frame.FirstRoll == null)
			{
				frame.FirstRoll = pinsKnocked;
			}
			else if(frame.SecondRoll == null)
			{
				frame.SecondRoll = pinsKnocked;
			}
			else if(frame.ThirdRoll == null && frame.FrameIndex == 10)
			{
				frame.ThirdRoll = pinsKnocked;
			}
			else
			{
				throw new Exception("We are trying to add pins to a frame that does not exist.");
			}

			CalculateScore();
            
			MoveToNextFrame();

			OnStateChanged?.Invoke();
		}

		private void MoveToNextFrame()
		{
			if(_currentFrameNode.Next == null)
			{
				return;
			}

			if(CurrentFrame.IsStrike || CurrentFrame.SecondRoll != null)
			{
				_currentFrameNode = _currentFrameNode.Next;
			}
		}

		private void CalculateScore()
		{
			int runningTotal = 0;
            LinkedListNode<GameFrame>? node = _frames.First;
			while(node != null && node.Value.FrameIndex <= _currentFrameNode.Value.FrameIndex)
			{
				int framePoints = 0;
				bool frameFinalized = false;
				GameFrame frame = node.Value;
				LinkedListNode<GameFrame>? nextNode = node.Next;
				LinkedListNode<GameFrame>? nextNextNode = null;
				if(nextNode != null)
				{
					nextNextNode = nextNode.Next;
				}

				if(frame.IsStrike)
				{
					//get next two rolls
					if(frame.FrameIndex == 10)
					{
						framePoints = (frame.FirstRoll ?? 0) + (frame.SecondRoll ?? 0) + (frame.ThirdRoll ?? 0);
						frameFinalized = frame.ThirdRoll != null;
					}
					else if(nextNode != null)
					{
						if(nextNode.Value.IsStrike && nextNextNode != null)
						{
							framePoints = 20 + (nextNextNode.Value.FirstRoll ?? 0);
							frameFinalized = nextNextNode.Value.FirstRoll != null;
						}
						else
						{
							framePoints = 10 + (nextNode.Value.FirstRoll ?? 0) + (nextNode.Value.SecondRoll ?? 0);
							frameFinalized = nextNode.Value.SecondRoll != null;
						}
					}
				}
				else if(frame.IsSpare)
				{
					if(frame.FrameIndex == 10)
					{
						framePoints = 10 + (frame.ThirdRoll ?? 0);
						frameFinalized = frame.ThirdRoll != null;
					}
					if(nextNode != null)
					{
						framePoints = 10 + (nextNode.Value.FirstRoll ?? 0);
						frameFinalized = nextNode.Value.FirstRoll != null;
					}
				}
				else
				{
					framePoints = (frame.FirstRoll ?? 0) + (frame.SecondRoll ?? 0);
					frameFinalized = frame.SecondRoll != null;
				}

				if(frameFinalized)
				{
					frame.Score = runningTotal + framePoints;
					runningTotal = frame.Score ?? 0;
				}

				node = node.Next;
			}
		}

		private bool IsGameComplete()
		{
			if (CurrentFrameIndex < 9)
			{
				return false;
			}

			GameFrame tenthFrame = Frames[9];

			if (tenthFrame.IsStrike || tenthFrame.IsSpare)
			{
				return tenthFrame.ThirdRoll != null;
			}

			return tenthFrame.SecondRoll != null;
		}
	}
}