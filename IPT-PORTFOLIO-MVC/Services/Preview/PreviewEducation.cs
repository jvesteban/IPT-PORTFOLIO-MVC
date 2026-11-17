namespace IPT_PORTFOLIO_MVC.Services.Preview;

public sealed record PreviewEducation(string Level, IReadOnlyList<PreviewEducationEntry> Entries)
{
    public static readonly string[] Levels = ["Elementary", "Junior High", "Senior High", "College"];

    public static IReadOnlyList<string> Grades(string level) => level switch
    {
        "Elementary" => ["Grade 1", "Grade 2", "Grade 3", "Grade 4", "Grade 5", "Grade 6"],
        "Junior High" => ["Grade 7", "Grade 8", "Grade 9", "Grade 10"],
        "Senior High" => ["Grade 11", "Grade 12"],
        "College" => ["1st year", "2nd year", "3rd year", "4th year"],
        _ => []
    };
}
