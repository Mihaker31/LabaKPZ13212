namespace WordGameProxyLib
{
    // Спільний інтерфейс Суб'єкта (Subject) для реальної гри та Proxy
    public interface IWordGameService
    {
        string Category { get; }
        string MaskedWord { get; }
        int AttemptsLeft { get; }
        int MaxAttempts { get; }
        bool IsGameOver { get; }
        bool IsWon { get; }
        IReadOnlyCollection<char> UsedLetters { get; }

        void StartNewGame(string category);
        GuessResult GuessLetter(char letter);
        GuessResult GuessWord(string word);
        string GetHint();
        string RevealSecretWord();
    }
}
