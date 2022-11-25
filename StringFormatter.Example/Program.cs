using StringFormatter.Core.Services;

var result = StringFormatterService.Shared.Format("I {{{Bebra}}} = {{{Bebra}}} {Array[0]} {Array[0]} I", new Cho());
Console.WriteLine(result);
Console.ReadLine();

class Cho
{
    public int Bebra { get; set; } = 10;

    public int[] Array { get; set; } = new int[] { 10 };
}