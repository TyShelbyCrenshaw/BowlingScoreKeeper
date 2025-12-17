using BowlingScoreKeeper.Components.Pages;

namespace BowlingScoreKeeper_UnitTest
{
	[TestFixture]
	public class Tests
	{
		public GameRefactored game;

		[SetUp]
		public void Setup()
		{
			game = new GameRefactored();
		}

		[Test]
		public void TestThatThereAre10Frames()
		{
			Assert.AreEqual(10, game.Frames.Count);
		}


		[Test]
		public void TestInitialScore()
		{
			Assert.AreEqual(null, game.Frames[0].Score);
		}

		[Test]
		public void TestAddingSingleRoll()
		{
			game.AddRoll(5);
			Assert.AreEqual(5, game.CurrentFrame.FirstRoll);
		}

		[Test]
		public void TestCompletingFrame()
		{
			game.AddRoll(5);
			game.AddRoll(3);
			Assert.AreEqual(1, game.CurrentFrameIndex);
			Assert.AreEqual(8, game.Frames[0].Score);
		}

		[Test]
		public void TestStrike()
		{
			game.AddRoll(10);
			Assert.AreEqual(1, game.CurrentFrameIndex);
			Assert.IsNull(game.Frames[0].Score); // Score can't be calculated yet
		}

		[Test]
		public void TestSpare()
		{
			game.AddRoll(5);
			game.AddRoll(5);
			Assert.AreEqual(1, game.CurrentFrameIndex);
			Assert.Null(game.Frames[0].Score); // Score can't be calculated yet
		}

		[Test]
		public void TestStrikeFollowedByOpenFrame()
		{
			game.AddRoll(10);  // Frame 1
			game.AddRoll(3);   // Frame 2
			Assert.IsNull(game.Frames[0].Score);  // Can't calculate yet
			game.AddRoll(4);   // Frame 2
			Assert.AreEqual(17, game.Frames[0].Score);  // Now we can calculate
			Assert.AreEqual(24, game.Frames[1].Score);
		}

		[Test]
		public void TestTwoConsecutiveStrikes()
		{
			game.AddRoll(10);  // Frame 1
			game.AddRoll(10);  // Frame 2
			Assert.IsNull(game.Frames[0].Score);  // Can't calculate yet
			game.AddRoll(5);   // Frame 3
			Assert.AreEqual(25, game.Frames[0].Score);  // Now we can calculate Frame 1
			Assert.IsNull(game.Frames[1].Score);  // Frame 2 still can't be calculated
		}

		[Test]
		public void TestThreeConsecutiveStrikes()
		{
			game.AddRoll(10);  // Frame 1
			game.AddRoll(10);  // Frame 2
			game.AddRoll(10);  // Frame 3
			Assert.AreEqual(30, game.Frames[0].Score);  // Now we can calculate Frame 1
			Assert.IsNull(game.Frames[1].Score);  // Frame 2 still can't be calculated
			Assert.IsNull(game.Frames[2].Score);  // Frame 3 can't be calculated yet
		}

		[Test]
		public void TestSpareFollowedByStrike()
		{
			game.AddRoll(5);   // Frame 1
			game.AddRoll(5);   // Frame 1 (Spare)
			Assert.IsNull(game.Frames[0].Score);  // Can't calculate yet
			game.AddRoll(10);  // Frame 2 (Strike)
			Assert.AreEqual(20, game.Frames[0].Score);  // Now we can calculate Frame 1
			Assert.IsNull(game.Frames[1].Score);  // Frame 2 can't be calculated yet
		}

		[Test]
		public void TestSpareFollowedByOpenFrame()
		{
			game.AddRoll(5);   // Frame 1
			game.AddRoll(5);   // Frame 1 (Spare)
			Assert.IsNull(game.Frames[0].Score);  // Can't calculate yet
			game.AddRoll(3);   // Frame 2
			Assert.AreEqual(13, game.Frames[0].Score);  // Now we can calculate Frame 1
			game.AddRoll(4);   // Frame 2
			Assert.AreEqual(20, game.Frames[1].Score);  // Now we can calculate Frame 2
		}

		[Test]
		public void TestStrikeSpareSequence()
		{
			game.AddRoll(10);  // Frame 1 (Strike)
			game.AddRoll(5);   // Frame 2
			game.AddRoll(5);   // Frame 2 (Spare)
			Assert.AreEqual(20, game.Frames[0].Score);  // Now we can calculate Frame 1
			Assert.IsNull(game.Frames[1].Score);  // Frame 2 can't be calculated yet
			game.AddRoll(7);   // Frame 3
			Assert.AreEqual(37, game.Frames[1].Score);  // Now we can calculate Frame 2
		}

		[Test]
		public void TestOpenFrameFollowedByStrike()
		{
			game.AddRoll(3);   // Frame 1
			game.AddRoll(4);   // Frame 1
			Assert.AreEqual(7, game.Frames[0].Score);  // Can calculate immediately
			game.AddRoll(10);  // Frame 2 (Strike)
			Assert.IsNull(game.Frames[1].Score);  // Frame 2 can't be calculated yet
		}

		[Test]
		public void TestStrikeFollowedByRegularFrame()
		{
			game.AddRoll(10); // Frame 1
			game.AddRoll(3);  // Frame 2
			game.AddRoll(4);  // Frame 2
			Assert.AreEqual(2, game.CurrentFrameIndex);
			Assert.AreEqual(17, game.Frames[0].Score);
			Assert.AreEqual(24, game.Frames[1].Score);
		}

		[Test]
		public void TestSpareFollowedByRegularRoll()
		{
			game.AddRoll(5);  // Frame 1
			game.AddRoll(5);  // Frame 1
			game.AddRoll(3);  // Frame 2
			Assert.AreEqual(1, game.CurrentFrameIndex);
			Assert.AreEqual(13, game.Frames[0].Score);
		}

		[Test]
		public void TestPerfectGame()
		{
			for (int i = 0; i < 12; i++)
			{
				game.AddRoll(10);
			}
			Assert.AreEqual(300, game.Frames[9].Score);
		}

		[Test]
		public void TestWorstGame()
		{
			for (int i = 0; i < 20; i++)
			{
				game.AddRoll(0);
			}
			Assert.AreEqual(0, game.Frames[9].Score);
		}

		[Test]
		public void TestTenthFrameStrike()
		{
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(0);
			}
			game.AddRoll(10); // 10th frame, first roll
			game.AddRoll(5);  // 10th frame, second roll
			game.AddRoll(3);  // 10th frame, third roll
			Assert.AreEqual(18, game.Frames[9].Score);
		}

		[Test]
		public void TestTenthFrameSpare()
		{
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(0);
			}
			game.AddRoll(5);  // 10th frame, first roll
			game.AddRoll(5);  // 10th frame, second roll
			game.AddRoll(3);  // 10th frame, third roll
			Assert.AreEqual(13, game.Frames[9].Score);
		}

		[Test]
		public void TestGameEndsAfterTenthFrame()
		{
			for (int i = 0; i < 20; i++)
			{
				game.AddRoll(1);
			}
			game.AddRoll(1); // This should not be added
			Assert.AreEqual(20, game.Frames[9].Score);
			Assert.IsNull(game.Frames[9].ThirdRoll);
		}

		[Test]
		public void TestStrikeInEighthFrame()
		{
			// Roll 7 frames of 1 pin each
			for (int i = 0; i < 14; i++)
			{
				game.AddRoll(1);
			}

			// Strike in 8th frame
			game.AddRoll(10);

			// 9th frame
			game.AddRoll(5);
			game.AddRoll(3);

			Assert.AreEqual(14, game.Frames[6].Score);
			Assert.AreEqual(32, game.Frames[7].Score);
			Assert.AreEqual(40, game.Frames[8].Score);
		}

		[Test]
		public void TestSpareInNinthFrame()
		{
			// Roll 8 frames of 1 pin each
			for (int i = 0; i < 16; i++)
			{
				game.AddRoll(1);
			}

			// Spare in 9th frame
			game.AddRoll(5);
			game.AddRoll(5);

			// 10th frame
			game.AddRoll(3);
			game.AddRoll(2);

			Assert.AreEqual(16, game.Frames[7].Score);
			Assert.AreEqual(29, game.Frames[8].Score);
			Assert.AreEqual(34, game.Frames[9].Score);
		}

		[Test]
		public void TestStrikeInNinthAndTenthFrames()
		{
			// Roll 8 frames of 1 pin each
			for (int i = 0; i < 16; i++)
			{
				game.AddRoll(1);
			}

			// Strike in 9th frame
			game.AddRoll(10);

			// Strike in 10th frame
			game.AddRoll(10);
			game.AddRoll(5);
			game.AddRoll(3);

			Assert.AreEqual(16, game.Frames[7].Score);
			Assert.AreEqual(41, game.Frames[8].Score);
			Assert.AreEqual(59, game.Frames[9].Score);
		}

		[Test]
		public void TestSpareInTenthFrame()
		{
			// Roll 9 frames of 1 pin each
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(1);
			}

			// Spare in 10th frame
			game.AddRoll(5);
			game.AddRoll(5);
			game.AddRoll(7);

			Assert.AreEqual(18, game.Frames[8].Score);
			Assert.AreEqual(35, game.Frames[9].Score);
		}

		[Test]
		public void TestThreeStrikesInTenthFrame()
		{
			// Roll 9 frames of 1 pin each
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(1);
			}

			// Three strikes in 10th frame
			game.AddRoll(10);
			game.AddRoll(10);
			game.AddRoll(10);

			Assert.AreEqual(18, game.Frames[8].Score);
			Assert.AreEqual(48, game.Frames[9].Score);
		}

		[Test]
		public void TestGameEndingWithOpenFrameInTenth()
		{
			// Roll 9 frames of 1 pin each
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(1);
			}

			// Open frame in 10th
			game.AddRoll(4);
			game.AddRoll(3);

			Assert.AreEqual(18, game.Frames[8].Score);
			Assert.AreEqual(25, game.Frames[9].Score);
			Assert.IsNull(game.Frames[9].ThirdRoll);
		}

		[Test]
		public void TestTenthFrameOpenFrameNoThirdRoll()
		{
			// Roll 9 frames of 1 pin each
			for (int i = 0; i < 18; i++)
			{
				game.AddRoll(1);
			}

			// 10th frame - open frame
			game.AddRoll(4);
			game.AddRoll(5);

			// Try to add a third roll (this should not be added)
			game.AddRoll(1);

			Assert.AreEqual(18, game.Frames[8].Score);
			Assert.AreEqual(27, game.Frames[9].Score);
			Assert.AreEqual(4, game.Frames[9].FirstRoll);
			Assert.AreEqual(5, game.Frames[9].SecondRoll);
			Assert.IsNull(game.Frames[9].ThirdRoll);
			Assert.AreEqual(9, game.CurrentFrameIndex);
		}
	}
}