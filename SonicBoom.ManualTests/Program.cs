namespace SonicBoom.ManualTests;

public class Program
{
    public static void Main(string[] args)
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
