namespace WordGameProxyLib
{
    // Реальний суб'єкт (RealSubject): містить словник і основну логіку гри 'Вгадай слово'
    public class RealWordGameService : IWordGameService
    {
        private readonly Dictionary<string, List<WordEntry>> _dictionary;
        private string _secretWord = string.Empty;
        private string _secretHint = string.Empty;
        private readonly HashSet<char> _openedLetters = new HashSet<char>();
        private readonly HashSet<char> _usedLetters = new HashSet<char>();

        public string Category { get; private set; } = string.Empty;
        public int AttemptsLeft { get; private set; } = 6;
        public int MaxAttempts => 6;
        public bool IsGameOver { get; private set; } = true;
        public bool IsWon { get; private set; } = false;
        public IReadOnlyCollection<char> UsedLetters => _usedLetters;

        public string MaskedWord
        {
            get
            {
                if (string.IsNullOrEmpty(_secretWord))
                    return string.Empty;

                return string.Join(" ", _secretWord.Select(c => _openedLetters.Contains(c) ? c : '_'));
            }
        }

        public static readonly string[] AvailableCategories =
        {
            "Програмування",
            "Тварини",
            "Міста України",
            "Космос"
        };

        public RealWordGameService()
        {
            _dictionary = new Dictionary<string, List<WordEntry>>
            {
                ["Програмування"] = new List<WordEntry>
                {
                    new WordEntry("ПРОКСІ", "Структурний патерн, що контролює доступ до іншого об'єкта"),
                    new WordEntry("ІНТЕРФЕЙС", "Контракт методів і властивостей без реалізації"),
                    new WordEntry("КОМПІЛЯТОР", "Перекладає вихідний код у машинний або проміжний код"),
                    new WordEntry("АЛГОРИТМ", "Чітка послідовність дій для розв'язання задачі"),
                    new WordEntry("ПОЛІМОРФІЗМ", "Здатність об'єктів мати різну реалізацію одного методу")
                },
                ["Тварини"] = new List<WordEntry>
                {
                    new WordEntry("ХАМЕЛЕОН", "Рептилія, яка змінює колір шкіри для маскування"),
                    new WordEntry("ПІНГВІН", "Морський птах, який не літає, але чудово пірнає"),
                    new WordEntry("ДЕЛЬФІН", "Розумний морський ссавець, що використовує ехолокацію"),
                    new WordEntry("ЛЕОПАРД", "Плямистий хижак із родини котячих"),
                    new WordEntry("ФЛАМІНГО", "Високий птах із рожевим оперенням")
                },
                ["Міста України"] = new List<WordEntry>
                {
                    new WordEntry("ЛЬВІВ", "Місто Лева, відоме своєю кавою та старою архітектурою"),
                    new WordEntry("ОДЕСА", "Перлина біля Чорного моря з Потьомкінськими сходами"),
                    new WordEntry("ПОЛТАВА", "Місто галушок на березі річки Ворскли"),
                    new WordEntry("ЧЕРНІВЦІ", "Місто на Буковині зі знаменитим університетом-резиденцією"),
                    new WordEntry("ХАРКІВ", "Велике студентське місто з майданом Свободи")
                },
                ["Космос"] = new List<WordEntry>
                {
                    new WordEntry("ГАЛАКТИКА", "Величезна система зір, газу та пилу, як-от Чумацький Шлях"),
                    new WordEntry("АСТЕРОЇД", "Кам'янисте небесне тіло, що обертається навколо Сонця"),
                    new WordEntry("ТЕЛЕСКОП", "Оптичний прилад для спостереження за зорями"),
                    new WordEntry("ОРБІТА", "Траєкторія руху планети або супутника"),
                    new WordEntry("КОМЕТА", "Крижане небесне тіло з яскравим хвостом")
                }
            };
        }

        public void StartNewGame(string category)
        {
            if (!_dictionary.TryGetValue(category, out var list))
            {
                category = "Програмування";
                list = _dictionary[category];
            }

            Category = category;
            var entry = list[Random.Shared.Next(list.Count)];
            _secretWord = entry.Word.ToUpperInvariant();
            _secretHint = entry.Hint;

            _openedLetters.Clear();
            _usedLetters.Clear();
            AttemptsLeft = MaxAttempts;
            IsGameOver = false;
            IsWon = false;
        }

        public GuessResult GuessLetter(char letter)
        {
            char upper = char.ToUpperInvariant(letter);
            _usedLetters.Add(upper);

            if (_secretWord.Contains(upper))
            {
                _openedLetters.Add(upper);
                if (_secretWord.All(c => _openedLetters.Contains(c)))
                {
                    IsWon = true;
                    IsGameOver = true;
                    return new GuessResult(true, false, $"Вітаємо! Літера '{upper}' є у слові — ви відгадали слово «{_secretWord}»!");
                }
                return new GuessResult(true, false, $"Чудово! Літера '{upper}' є у загаданому слові.");
            }
            else
            {
                AttemptsLeft = Math.Max(0, AttemptsLeft - 1);
                if (AttemptsLeft == 0)
                {
                    IsGameOver = true;
                    IsWon = false;
                    return new GuessResult(false, false, $"Спроби закінчилися! Загадане слово було: «{_secretWord}».");
                }
                return new GuessResult(false, false, $"Літери '{upper}' немає у слові. Залишилось спроб: {AttemptsLeft}.");
            }
        }

        public GuessResult GuessWord(string word)
        {
            string normalized = word.Trim().ToUpperInvariant();
            if (normalized == _secretWord)
            {
                foreach (char c in _secretWord)
                    _openedLetters.Add(c);
                IsWon = true;
                IsGameOver = true;
                return new GuessResult(true, false, $"Блискуче! Ви вгадали слово «{_secretWord}» цілком!");
            }

            AttemptsLeft = Math.Max(0, AttemptsLeft - 2);
            if (AttemptsLeft == 0)
            {
                IsGameOver = true;
                IsWon = false;
                return new GuessResult(false, false, $"Невірно! Спроби вичерпано. Загадане слово: «{_secretWord}».");
            }
            return new GuessResult(false, false, $"Слово «{normalized}» невірне (-2 спроби). Залишилось спроб: {AttemptsLeft}.");
        }

        public string GetHint()
        {
            return _secretHint;
        }

        public string RevealSecretWord()
        {
            return _secretWord;
        }
    }
}
