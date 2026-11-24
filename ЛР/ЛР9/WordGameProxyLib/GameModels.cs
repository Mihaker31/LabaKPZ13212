namespace WordGameProxyLib
{
    public enum UserRole
    {
        Guest,      // Гість: максимум 1 підказка, без права відкрити слово
        VipPlayer,  // VIP-гравець: до 3 підказок, без права відкрити слово
        Admin       // Адміністратор: повний доступ, включаючи перегляд загаданого слова
    }

    public record GuessResult(bool IsSuccess, bool ServedFromProxy, string Message);

    public record WordEntry(string Word, string Hint);
}
