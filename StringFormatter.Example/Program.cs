using StringFormatter.Core.Services;

var result = StringFormatterService.Shared.Format("I {Bebra} I", new Cho());
Console.WriteLine(result);
Console.ReadLine();

class Cho
{
    public int Bebra { get; set; } = 10;
}