internal class Program {
    static void Main() {
        var evaluator = new Evaluator("(2 + 5^(2 * 2) - (4 * (-2 - (1)))) * (42^-4 / ((2 / -3^-2) * (32 - -2)) + 1)");

        var result = evaluator.Parse();
        Console.WriteLine(result);
    }
}