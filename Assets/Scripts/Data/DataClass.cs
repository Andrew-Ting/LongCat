public class DataClass 
{
    // w/ respect to forward of cat
    public enum Directions
    {
        Forward,
        Right,
        Behind,
        Left
    }
    // direction camera is facing in game
    public enum ViewDirection 
    {
        North = 0,
        South = 2,
        East = 1,
        West = 3
    }
    public enum MenuCameraViews
    {
        homeView,
        levelView,
        noView1,
        noView2
     }
     // direction camera is facing in game
    public enum BlockState
    {
        HasNotFallen,
        HasFallen,
        ToBeDeleted
    }
    public enum PowerUp
    {
        Grow,
        Shrink
    }
}
