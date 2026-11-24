namespace WordGameProxyLib
{
    // Замісник (Proxy):
    // 1) Віртуальний проксі — відкладена (Lazy) ініціалізація RealWordGameService лише при старті гри;
    // 2) Захисний проксі — контроль доступу за ролями (Guest / VipPlayer / Admin) та валідація вводу;
    // 3) Кешуючий проксі — кешування перевірених літер і підказок без повторних звернень до RealSubject;
    // 4) Логуючий проксі — фіксація кожного перехопленого та делегованого виклику.
    public class WordGameProxy : IWordGameService
    {
        private RealWordGameService? _realService;
        private readonly Dictionary<char, GuessResult> _letterCache = new Dictionary<char, GuessResult>();
        private string? _cachedHint;
        private int _hintsUsedInRound;

        private readonly List<string> _actionLog = new List<string>();

        public UserRole CurrentRole { get; set; } = UserRole.Guest;
        public bool IsRealServiceInitialized => _realService != null;

        public int TotalCalls { get; private set; }
        public int ForwardedCalls { get; private set; }
        public int CachedOrBlockedCalls { get; private set; }

        public IReadOnlyList<string> ActionLog => _actionLog;

        public string Category => _realService?.Category ?? "Не обрано";
        public string MaskedWord => _realService?.MaskedWord ?? "_ _ _ _ _";
        public int AttemptsLeft => _realService?.AttemptsLeft ?? 6;
        public int MaxAttempts => _realService?.MaxAttempts ?? 6;
        public bool IsGameOver => _realService?.IsGameOver ?? true;
        public bool IsWon => _realService?.IsWon ?? false;
        public IReadOnlyCollection<char> UsedLetters => _realService?.UsedLetters ?? Array.Empty<char>();

        public WordGameProxy()
        {
            AddLog("[INIT] Створено WordGameProxy. RealWordGameService ще НЕ створено (Lazy Init).");
        }

        private void EnsureInitialized()
        {
            if (_realService == null)
            {
                _realService = new RealWordGameService();
                AddLog("[LAZY INIT] Перший запуск -> створено RealWordGameService і завантажено словник.");
            }
        }

        public void StartNewGame(string category)
        {
            TotalCalls++;
            EnsureInitialized();

            _letterCache.Clear();
            _cachedHint = null;
            _hintsUsedInRound = 0;

            ForwardedCalls++;
            AddLog($"[FORWARD] StartNewGame('{category}') -> RealWordGameService (кеш очищено).");
            _realService!.StartNewGame(category);
        }

        public GuessResult GuessLetter(char letter)
        {
            TotalCalls++;

            if (!IsRealServiceInitialized || IsGameOver)
            {
                CachedOrBlockedCalls++;
                string msg = "Гра не активна! Спочатку натисніть 'Нова гра'.";
                AddLog($"[BLOCKED] GuessLetter('{letter}') перехоплено Proxy: гра завершена або не почата.");
                return new GuessResult(false, true, msg);
            }

            char upper = char.ToUpperInvariant(letter);
            if (!char.IsLetter(upper))
            {
                CachedOrBlockedCalls++;
                string msg = $"Символ '{letter}' не є літерою. Запит заблоковано Proxy.";
                AddLog($"[BLOCKED] GuessLetter('{letter}'): невалідний символ.");
                return new GuessResult(false, true, msg);
            }

            // Кешування повторного вибору тієї самої літери
            if (_letterCache.TryGetValue(upper, out var cached))
            {
                CachedOrBlockedCalls++;
                AddLog($"[CACHE HIT] Літера '{upper}' вже перевірялась — результат повернуто з кешу Proxy!");
                return new GuessResult(cached.IsSuccess, true, $"[З кешу Proxy] Літеру '{upper}' уже перевірено раніше (спроба не знімається).");
            }

            ForwardedCalls++;
            AddLog($"[FORWARD] GuessLetter('{upper}') -> делеговано до RealWordGameService.");
            var result = _realService!.GuessLetter(upper);
            _letterCache[upper] = result;
            return result;
        }

        public GuessResult GuessWord(string word)
        {
            TotalCalls++;

            if (!IsRealServiceInitialized || IsGameOver)
            {
                CachedOrBlockedCalls++;
                AddLog("[BLOCKED] GuessWord(): гра не активна.");
                return new GuessResult(false, true, "Гра не активна! Почніть нову гру.");
            }

            if (string.IsNullOrWhiteSpace(word) || word.Trim().Length < 2)
            {
                CachedOrBlockedCalls++;
                AddLog($"[BLOCKED] GuessWord('{word}'): занадто коротке слово, відхилено Proxy.");
                return new GuessResult(false, true, "Введіть слово мінімум із 2 літер (відфільтровано Proxy).");
            }

            ForwardedCalls++;
            AddLog($"[FORWARD] GuessWord('{word.Trim().ToUpperInvariant()}') -> RealWordGameService.");
            return _realService!.GuessWord(word);
        }

        public string GetHint()
        {
            TotalCalls++;

            if (!IsRealServiceInitialized || IsGameOver)
            {
                CachedOrBlockedCalls++;
                AddLog("[BLOCKED] GetHint(): гра ще не розпочата або вже завершена.");
                return "Спочатку почніть нову гру.";
            }

            // Якщо підказка вже завантажена — віддаємо з кешу Proxy без звернення до RealSubject
            if (_cachedHint != null)
            {
                CachedOrBlockedCalls++;
                AddLog("[CACHE HIT] GetHint() -> повернено з кешу Proxy (ліміт не витрачається).");
                return $"[Кеш Proxy] {_cachedHint}";
            }

            // Захисний проксі: контроль прав ролі
            int maxHints = CurrentRole switch
            {
                UserRole.Guest => 1,
                UserRole.VipPlayer => 3,
                UserRole.Admin => 99,
                _ => 1
            };

            if (_hintsUsedInRound >= maxHints)
            {
                CachedOrBlockedCalls++;
                AddLog($"[ACCESS DENIED] GetHint() для ролі {CurrentRole}: вичерпано ліміт ({maxHints}).");
                return $"Доступ обмежено Proxy: для ролі '{CurrentRole}' ліміт підказок ({maxHints}) вичерпано.";
            }

            _hintsUsedInRound++;
            ForwardedCalls++;
            AddLog($"[FORWARD] GetHint() (роль: {CurrentRole}) -> запит до RealWordGameService.");
            _cachedHint = _realService!.GetHint();
            return _cachedHint;
        }

        public string RevealSecretWord()
        {
            TotalCalls++;

            if (!IsRealServiceInitialized)
            {
                CachedOrBlockedCalls++;
                AddLog("[BLOCKED] RevealSecretWord(): RealWordGameService ще не ініціалізовано.");
                return "Гра ще не запускалась.";
            }

            // Захисний проксі: підглядати секретне слово під час гри може тільки Admin
            if (!IsGameOver && CurrentRole != UserRole.Admin)
            {
                CachedOrBlockedCalls++;
                AddLog($"[ACCESS DENIED] RevealSecretWord() заблоковано! Роль '{CurrentRole}' не має прав Admin.");
                return $"ДОСТУП ЗАБОРОНЕНО (Protection Proxy): роль '{CurrentRole}' не може підглядати слово! Перемкніть роль на Admin.";
            }

            ForwardedCalls++;
            AddLog($"[FORWARD] RevealSecretWord() дозволено для ролі {CurrentRole} -> RealWordGameService.");
            return $"Загадане слово: «{_realService!.RevealSecretWord()}»";
        }

        public void ClearLog()
        {
            _actionLog.Clear();
            AddLog("[LOG] Журнал Proxy очищено.");
        }

        private void AddLog(string message)
        {
            _actionLog.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}
