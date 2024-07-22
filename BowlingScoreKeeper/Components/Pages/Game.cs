using NUnit.Framework;

namespace BowlingScoreKeeper.Components.Pages
{
	public class Game
	{
		public List<GameFrame> Frames { get; set; } = new List<GameFrame>();
		public int CurrentFrameIndex { get; set; } = 0;
        public int CurrentGameMaxPossible { get; set; } = 0;

        public event Action OnStateChanged;

        public Game()
		{
            for (int i = 0; i < 10; i++)
            {
                Frames.Add(new GameFrame()
                {
					FrameIndex = i + 1
				});
            }
        }

		public GameFrame CurrentFrame => Frames[CurrentFrameIndex];

        public void AddRoll(int pinsKnocked)
        {
            if(CurrentFrameIndex >= 10) 
            {
                return;
            }

			//what is our current frame
			GameFrame currentFrame = Frames[CurrentFrameIndex];

			//add the pinsKnock to the current frame
			if (currentFrame.FirstRoll == null)
            {
                currentFrame.FirstRoll = pinsKnocked;
            }
            else if(currentFrame.SecondRoll == null)
            {
                currentFrame.SecondRoll = pinsKnocked;
            }
            else if(CurrentFrameIndex == 9 && currentFrame.ThirdRoll == null)
            {
                currentFrame.ThirdRoll = pinsKnocked;
            }
            else
            {
                Assert.Fail("We are trying to add a roll to an out of bounds frame roll");
            }

            //calculate the score for all frames
            CalculateScore();

            //CalculateMaximumScore();

			//move frame forward
            //only move the frame if we got a strike or we are the second roll
			MoveToNextFrame();

			//invoke statechange update to notify components they need to update there values
			OnStateChanged?.Invoke();
		}

        private void MoveToNextFrame()
        {
            //If we are not the last frame
            //And we have fill out the frame
            if (Frames[CurrentFrameIndex] != Frames.Last() &&
                (Frames[CurrentFrameIndex].SecondRoll != null ||
				Frames[CurrentFrameIndex].IsStrike))
            {
                CurrentFrameIndex += 1;
            }
        }

        private void CalculateScore()
        {
            //keep a running total so you can pass that up the next frame
            int runningTotal = 0;

			//calculate each frame GameScoreUpToCurrentFrame
			for (int i = 0; i <= CurrentFrameIndex; i++)
            {
                GameFrame currentFrame = Frames[i];
				//get the values for my Frames[i] and set the GameScoreUpToCurrentFrame
				int currentFramePoints = (currentFrame.FirstRoll ?? 0) + (currentFrame.SecondRoll ?? 0);

				//if you have spares you need to look one roll forward and add that
				if(currentFrame.IsSpare)
                {
                    //you can get an out of bounds if this is the last frame
                    if(i < 9)
                    {
						GameFrame nextFrame = Frames[i + 1];
						currentFramePoints += nextFrame.FirstRoll ?? 0;
					}
                    else
                    {
                        //yes you can get here if we get a spare in the 10 frame
                        //we are in the 10th frame and we just need to add the 3rd shot
                        currentFramePoints += currentFrame.ThirdRoll ?? 0;
                    }
                }
				//if you have a stike you need to look two rolls forward and add that
                else if(currentFrame.IsStrike)
                {
                    //Frame 8
                    if (i < 8)
                    {
                        //we cannot go out of bounds
                        GameFrame nextFrame = Frames[i + 1];
						currentFramePoints += nextFrame.FirstRoll ?? 0;
						if(nextFrame.IsStrike)
                        {
                            GameFrame twoNextFrame = Frames[i + 2];
                            currentFramePoints += twoNextFrame.FirstRoll ?? 0;
                        }
                        else
                        {
							currentFramePoints += nextFrame.SecondRoll ?? 0;
                        }

					}
                    //Frame 9
                    else if(i < 9)
                    {
						//be carful we can go out of bounds now
						GameFrame nextFrame = Frames[i + 1];
                        currentFramePoints += nextFrame.FirstRoll ?? 0;
                        currentFramePoints += nextFrame.SecondRoll ?? 0;
					}
                    //Frame 10
                    else
                    {
                        //we are in the last Frame
                        currentFramePoints = (currentFrame.FirstRoll ?? 0) + (currentFrame.SecondRoll ?? 0) + (currentFrame.ThirdRoll ?? 0);
                    }
				}
                currentFrame.Score = runningTotal + currentFramePoints;
                runningTotal = currentFrame.Score ?? 0;
			}
		}
	}


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
