internal class Program {
    static void Main() {
        var evaluator = new Evaluator("(2 + 5 - (4 * (-2 - (1)))) * (42 / ((2 / -3) * (32 - -2)) + 1)");

        var result = evaluator.Parse();
        Console.WriteLine(result);
    }
}