public class Evaluator {
    private readonly string Source;
    private int Current = 0;

    public Evaluator(string source) {
        Source = source.Replace(" ", "");
    }

    public double Parse() {
        return ParseTerm();
    }

    private double ParseTerm() {
        var left = ParseFactor();

        while (Match('+') || Match('-')) {
            char op = Source[Current - 1];
            var right = ParseFactor();
            left = op == '+' ? left + right : left - right;
        }

        return left;
    }

    private double ParseFactor() {
        var left = ParseNumeric();

        while (Match('*') || Match('/')) {
            char op = Source[Current - 1];
            var right = ParseNumeric();
            left = op == '*' ? left * right : left / right;
        }

        return left;
    }

    private double ParseGroup() {
        if (Match('(')) {
            var val = ParseTerm();
            if (!Match(')')) {
                throw new Exception("expected ')'");
            }

            return val;
        }

        return ParseNumeric();
    }

    private double ParseNumeric() {
        int start = Current;
        bool hasDecimal = false;

        while (Current < Source.Length) {
            char c = Source[Current];

            if (char.IsDigit(c)) {
                Current++;
            } else if (c is '.' && !hasDecimal) {
                hasDecimal = true;
                Current++;
            } else {
                break;
            }
        }

        string number = Source[start..Current];
        return double.Parse(number);
    }

    private bool Match(char c) {
        if (Current >= Source.Length) return false;

        if (Source[Current] == c) {
            Current++;
            return true;
        }

        return false;
    }
}