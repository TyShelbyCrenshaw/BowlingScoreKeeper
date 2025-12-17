
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
			//is the game over?
			if (_currentFrameNode == null || IsGameComplete())
			{
				return;
			}

			if (!IsValidRoll(pinsKnocked))
			{
				return; //silently reject invalid rolls
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
			while(node != null && node.Value.FrameIndex <= CurrentFrame.FrameIndex)
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

		private bool IsValidRoll(int pinsKnocked)
		{
			if(pinsKnocked < 0 || pinsKnocked > 10)
			{
				return false;
			}
			if(CurrentFrameIndex < 9)
			{
				if(CurrentFrame.FirstRoll == null)
				{
					return true;
				}
				else
				{
					if (10 < (CurrentFrame.FirstRoll ?? 0) + pinsKnocked)
					{
						return false;
					}
				}

			}
			if(CurrentFrameIndex == 9)
			{
				//if we are the first roll we are valid
				if(CurrentFrame.FirstRoll == null)
				{
					return true;
				}

				//if we are the second roll we have see did we get a strike if we did we can score up to 10
				//else we can only score up to 10 for the first and second
				if(CurrentFrame.SecondRoll == null)
				{
					if(CurrentFrame.IsStrike)
					{
						return true;
					}
					else
					{
						if((CurrentFrame.FirstRoll ?? 0) + pinsKnocked > 10)
						{
							return false;
						}
						return true;
					}
				}

				//if we have a first and second score we must have gotten either a strike or a spare.
				//if it was a was a spare we can get a we can have up to 10 points.
				//if it was a strike we need to see if we got 10 points on the second frame if we did we can get 10 point
				//else we can only get 10 - second frame points
				//if we are on the third roll and we have not gotten 10 points yet we are invalid
				if(CurrentFrame.ThirdRoll == null)
				{
					if(CurrentFrame.IsSpare)
					{
						return true;
					}
					else if(CurrentFrame.IsStrike)
					{
						if((CurrentFrame.SecondRoll ?? 0) == 10)
						{
							return true;
						}
						else
						{
							if((CurrentFrame.SecondRoll ?? 0) + pinsKnocked > 10)
							{
								return false;
							}
							return true;
						}
					}
					return false;
				}
			}
			return true;
		}
	}
}