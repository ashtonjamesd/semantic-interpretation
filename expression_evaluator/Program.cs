internal class Program {
    static void Main() {
        var evaluator = new Evaluator("(-2 + (-4)) * 4");

        var result = evaluator.Parse();
        Console.WriteLine(result);
    }
}