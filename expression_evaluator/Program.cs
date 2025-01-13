internal class Program {
    static void Main() {
        var evaluator = new Evaluator("2 * 5 + 1 + 32 / 54.2 * 2 + 321");

        var result = evaluator.Parse();
        Console.WriteLine(result);
    }
}