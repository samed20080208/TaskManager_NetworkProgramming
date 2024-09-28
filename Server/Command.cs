namespace Server;

public class Command
{
    public const string ProsesList = "ProssesList";
    public const string Kill = "Kill";
    public const string Start = "Start";
    public string? Text { get; set; }
    public string? Param { get; set; }
}

