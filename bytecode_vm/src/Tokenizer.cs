public class Tokenizer {
    private readonly string Source;
    private readonly List<Token> Tokens = new();
    private int Current = 0;

    public Tokenizer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        while (!IsEnd()) {
            

            Current++;
        }

        return Tokens;
    }

    private bool IsEnd() {
        return Current >= Source.Length;
    }
}