namespace SonicBoom.ManualTests;

public class Program
{
    public static void Main(string[] args)
    {
        // TestContinuousPlayback();
        TestStoppingLoopingOgg();
    }

    private static void TestStoppingLoopingOgg()
    {
        var player = new AudioPlayer();
        player.LoopPlayback = true;
        player.Load(Path.Join("TestData", "crunch.ogg"));
        player.Play();

        Thread.Sleep(1000);
        player.Stop();

        Thread.Sleep(5000);
    }

    private static void TestContinuousPlayback()
    {
        int iterationsLeft = 10;
        var player = new AudioPlayer();
        player.LoopPlayback = false;

        while (iterationsLeft-- > 0)
        {
            player.Load(Path.Join("TestData", "crunch.ogg"));
            player.Play();
            Thread.Sleep(1000);
        }
    }
}
