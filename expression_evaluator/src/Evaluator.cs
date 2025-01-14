public class Evaluator(string source) {
    private readonly string Source = source.Replace(" ", "");
    private int Current = 0;

    public double Parse() {
        return ParseTerm();
    }

    private double ParseTerm() {
        var left = ParseFactor();

        while (Match('+') || Match('-')) {
            char op = Source[Current - 1];
            var right = ParseFactor();
            left = op is '+' ? left + right : left - right;
        }

        return left;
    }

    private double ParseFactor() {
        var left = ParseExponent();

        while (Match('*') || Match('/')) {
            char op = Source[Current - 1];
            var right = ParseExponent();
            left = op is '*' ? left * right : left / right;
        }

        return left;
    }

    private double ParseExponent() {
        var left = ParseGroup();

        while (Match('^')) {
            var right = ParseGroup();
            left = Math.Pow(left, right);
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

        if (Match('-')) {
            return -ParseGroup();
        }

        return ParseNumeric();
    }



    private double ParseNumeric() {
        int start = Current;
        bool hasDecimal = false;

        while (Current < Source.Length) {
            if (char.IsDigit(Source[Current])) {
                Current++;
            } else if (Source[Current] is '.' && !hasDecimal) {
                hasDecimal = true;
                Current++;
            } else {
                break;
            }
        }

        var number = Source[start..Current];
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