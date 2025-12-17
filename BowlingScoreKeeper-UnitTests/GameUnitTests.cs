using BowlingScoreKeeper.Components.Pages;

namespace BowlingScoreKeeper_UnitTest;

[TestFixture]
public class GameUnitTests
{
	public Game game;

	[SetUp]
	public void Setup()
	{
		game = new Game();
	}

	[Test]
	public void TestThatThereAre10Frames()
	{
		Assert.That(game.Frames.Count, Is.EqualTo(10));
	}


	[Test]
	public void TestInitialScore()
	{
		Assert.That(game.Frames[0].Score, Is.Null);
	}

	[Test]
	public void TestAddingSingleRoll()
	{
		game.AddRoll(5);
		Assert.That(game.CurrentFrame.FirstRoll, Is.EqualTo(5));
	}

	[Test]
	public void TestCompletingFrame()
	{
		game.AddRoll(5);
		game.AddRoll(3);
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(1));
		Assert.That(game.Frames[0].Score, Is.EqualTo(8));
	}

	[Test]
	public void TestStrike()
	{
		game.AddRoll(10);
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(1));
		Assert.That(game.Frames[0].Score, Is.Null); // Score can't be calculated yet
	}

	[Test]
	public void TestSpare()
	{
		game.AddRoll(5);
		game.AddRoll(5);
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(1));
		Assert.That(game.Frames[0].Score, Is.Null); // Score can't be calculated yet
	}

	[Test]
	public void TestStrikeFollowedByOpenFrame()
	{
		game.AddRoll(10);  // Frame 1
		game.AddRoll(3);   // Frame 2
		Assert.That(game.Frames[0].Score, Is.Null);  // Can't calculate yet
		game.AddRoll(4);   // Frame 2
		Assert.That(game.Frames[0].Score, Is.EqualTo(17));  // Now we can calculate
		Assert.That(game.Frames[1].Score, Is.EqualTo(24));
	}

	[Test]
	public void TestTwoConsecutiveStrikes()
	{
		game.AddRoll(10);  // Frame 1
		game.AddRoll(10);  // Frame 2
		Assert.That(game.Frames[0].Score, Is.Null);  // Can't calculate yet
		game.AddRoll(5);   // Frame 3
		Assert.That(game.Frames[0].Score, Is.EqualTo(25));  // Now we can calculate Frame 1
		Assert.That(game.Frames[1].Score, Is.Null);  // Frame 2 still can't be calculated
	}

	[Test]
	public void TestThreeConsecutiveStrikes()
	{
		game.AddRoll(10);  // Frame 1
		game.AddRoll(10);  // Frame 2
		game.AddRoll(10);  // Frame 3
		Assert.That(game.Frames[0].Score, Is.EqualTo(30));  // Now we can calculate Frame 1
		Assert.That(game.Frames[1].Score, Is.Null);  // Frame 2 still can't be calculated
		Assert.That(game.Frames[2].Score, Is.Null);  // Frame 3 can't be calculated yet
	}

	[Test]
	public void TestSpareFollowedByStrike()
	{
		game.AddRoll(5);   // Frame 1
		game.AddRoll(5);   // Frame 1 (Spare)
		Assert.That(game.Frames[0].Score, Is.Null);  // Can't calculate yet
		game.AddRoll(10);  // Frame 2 (Strike)
		Assert.That(game.Frames[0].Score, Is.EqualTo(20));  // Now we can calculate Frame 1
		Assert.That(game.Frames[1].Score, Is.Null);  // Frame 2 can't be calculated yet
	}

	[Test]
	public void TestSpareFollowedByOpenFrame()
	{
		game.AddRoll(5);   // Frame 1
		game.AddRoll(5);   // Frame 1 (Spare)
		Assert.That(game.Frames[0].Score, Is.Null);  // Can't calculate yet
		game.AddRoll(3);   // Frame 2
		Assert.That(game.Frames[0].Score, Is.EqualTo(13));  // Now we can calculate Frame 1
		game.AddRoll(4);   // Frame 2
		Assert.That(game.Frames[1].Score, Is.EqualTo(20));  // Now we can calculate Frame 2
	}

	[Test]
	public void TestStrikeSpareSequence()
	{
		game.AddRoll(10);  // Frame 1 (Strike)
		game.AddRoll(5);   // Frame 2
		game.AddRoll(5);   // Frame 2 (Spare)
		Assert.That(game.Frames[0].Score, Is.EqualTo(20));  // Now we can calculate Frame 1
		Assert.That(game.Frames[1].Score, Is.Null);  // Frame 2 can't be calculated yet
		game.AddRoll(7);   // Frame 3
		Assert.That(game.Frames[1].Score, Is.EqualTo(37));  // Now we can calculate Frame 2
	}

	[Test]
	public void TestOpenFrameFollowedByStrike()
	{
		game.AddRoll(3);   // Frame 1
		game.AddRoll(4);   // Frame 1
		Assert.That(game.Frames[0].Score, Is.EqualTo(7));  // Can calculate immediately
		game.AddRoll(10);  // Frame 2 (Strike)
		Assert.That(game.Frames[1].Score, Is.Null);  // Frame 2 can't be calculated yet
	}

	[Test]
	public void TestStrikeFollowedByRegularFrame()
	{
		game.AddRoll(10); // Frame 1
		game.AddRoll(3);  // Frame 2
		game.AddRoll(4);  // Frame 2
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(2));
		Assert.That(game.Frames[0].Score, Is.EqualTo(17));
		Assert.That(game.Frames[1].Score, Is.EqualTo(24));
	}

	[Test]
	public void TestSpareFollowedByRegularRoll()
	{
		game.AddRoll(5);  // Frame 1
		game.AddRoll(5);  // Frame 1
		game.AddRoll(3);  // Frame 2
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(1));
		Assert.That(game.Frames[0].Score, Is.EqualTo(13));
	}

	[Test]
	public void TestPerfectGame()
	{
		for (int i = 0; i < 12; i++)
		{
			game.AddRoll(10);
		}
		Assert.That(game.Frames[9].Score, Is.EqualTo(300));
	}

	[Test]
	public void TestWorstGame()
	{
		for (int i = 0; i < 20; i++)
		{
			game.AddRoll(0);
		}
		Assert.That(game.Frames[9].Score, Is.EqualTo(0));
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
		Assert.That(game.Frames[9].Score, Is.EqualTo(18));
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
		Assert.That(game.Frames[9].Score, Is.EqualTo(13));
	}

	[Test]
	public void TestGameEndsAfterTenthFrame()
	{
		for (int i = 0; i < 20; i++)
		{
			game.AddRoll(1);
		}
		game.AddRoll(1); // This should not be added
		Assert.That(game.Frames[9].Score, Is.EqualTo(20));
		Assert.That(game.Frames[9].ThirdRoll, Is.Null);
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

		Assert.That(game.Frames[6].Score, Is.EqualTo(14));
		Assert.That(game.Frames[7].Score, Is.EqualTo(32));
		Assert.That(game.Frames[8].Score, Is.EqualTo(40));
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

		Assert.That(game.Frames[7].Score, Is.EqualTo(16));
		Assert.That(game.Frames[8].Score, Is.EqualTo(29));
		Assert.That(game.Frames[9].Score, Is.EqualTo(34));
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

		Assert.That(game.Frames[7].Score, Is.EqualTo(16));
		Assert.That(game.Frames[8].Score, Is.EqualTo(41));
		Assert.That(game.Frames[9].Score, Is.EqualTo(59));
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

		Assert.That(game.Frames[8].Score, Is.EqualTo(18));
		Assert.That(game.Frames[9].Score, Is.EqualTo(35));
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

		Assert.That(game.Frames[8].Score, Is.EqualTo(18));
		Assert.That(game.Frames[9].Score, Is.EqualTo(48));
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

		Assert.That(game.Frames[8].Score, Is.EqualTo(18));
		Assert.That(game.Frames[9].Score, Is.EqualTo(25));
		Assert.That(game.Frames[9].ThirdRoll, Is.Null);
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

		Assert.That(game.Frames[8].Score, Is.EqualTo(18));
		Assert.That(game.Frames[9].Score, Is.EqualTo(27));
		Assert.That(game.Frames[9].FirstRoll, Is.EqualTo(4));
		Assert.That(game.Frames[9].SecondRoll, Is.EqualTo(5));
		Assert.That(game.Frames[9].ThirdRoll, Is.Null);
		Assert.That(game.CurrentFrameIndex, Is.EqualTo(9));
	}
}