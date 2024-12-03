using System.IO;

var game = new HangMan();
game.Details();
game.RunGame();

Console.ReadKey();


class HangMan
{
    private ExtractingWords extractor;
    private SelectingAWord _selectingWord;
    private WordManager wordManager;
    private InputManager inputManager;
    private AttempManager attempManager;
    public readonly string SelectedWord;
    private List<string> words;

    public HangMan()
    {
        extractor = new ExtractingWords();
        words = new List<string>();
        inputManager = new InputManager();
        attempManager = new AttempManager();
        // Reading words and add to list
        words = extractor.ReadWordsFromFile(words);
        // Selecting the random word
        _selectingWord = new SelectingAWord(words);
        // Saving the word
        SelectedWord = _selectingWord.RandomWord;
        // Upload to wordManager
        wordManager = new WordManager(SelectedWord);
    }

    public void Details()
    {
        Console.WriteLine($"The word is {SelectedWord}");
    }
    public void RunGame()
    {
        int attemps = attempManager.RemainingAttempts();
        while (attemps > 0)
        {
            Console.WriteLine($"Remaining attemps: {attemps}");
            Console.WriteLine($"Word: {wordManager.UserWord}");
            Console.WriteLine("Write a letter:");
            string playerInput = Console.ReadLine();
            char charPlayerInput;
            if (InputManager.IsValidChar(playerInput))
            { 
                charPlayerInput = playerInput[0];
                attemps = attempManager.UpdateRemainingAttempts(inputManager.IsCharUsed(charPlayerInput), inputManager.IsCharInWord(charPlayerInput, wordManager.SecretWord));
                wordManager.RevealingWord(charPlayerInput);
            }
            continue;
        }
    }
}

public class ExtractingWords
{
    private StreamReader _sReader;
    private string _lineToRead;
    private readonly string _filePath;

    public ExtractingWords(string path = "C:\\Users\\alexp\\Desktop\\HangmanWords.txt")
    {
        _filePath = path;
        _sReader = new StreamReader(_filePath);
    }

    public List<string> ReadWordsFromFile(List<string> words)
    {
        _lineToRead = _sReader.ReadLine();
        while (_lineToRead != null)
        {
            words.Add(_lineToRead.Trim());
            _lineToRead = _sReader.ReadLine();
        }
        _sReader.Close();
        return words;
    }
}


public class SelectingAWord
{
    public readonly string RandomWord;

    public SelectingAWord(List<string> words)
    {
        var random = new Random();
        RandomWord = words[random.Next(0, words.Count)];
    }
}

public class WordManager
{
    public readonly string SecretWord;
    public string UserWord = "";

    public WordManager(string SelectedWord)
    {
        SecretWord = SelectedWord;
        foreach (char c in SecretWord) UserWord += '_';
    }

    public string RevealingWord(char userInputChar) 
    {
        List<int> positions = new List<int>();

        for (int i = 0; i < SecretWord.Length; i++)
        {
            if (SecretWord[i] == userInputChar) positions.Add(i);
        }

        char[] tempWordArr = UserWord.ToCharArray();

        if (positions.Count > 0) 
        {
            for (int i = 0; i < positions.Count; i++)
            {
                tempWordArr[positions[i]] = userInputChar;
            }
            UserWord = new string(tempWordArr);
        }
        return UserWord;
    }
}

public class InputManager
{

    private List<char> WrongLetters; 

    public InputManager()
    {
        WrongLetters = new List<char>();
    }

    public static bool IsValidChar(string input)
    {
        bool InputIsValid = Char.TryParse(input, out char inputChar);
        if (InputIsValid)
        {
            if (string.IsNullOrEmpty(input)) return false;
            else if (Char.IsLetter(inputChar)) return true;
            return false;
        }
        return false;
    }

    public bool IsCharUsed (char c)
    {
        if(WrongLetters.Contains(c)) return true;
        WrongLetters.Add(c);
        return false;
    }

    public bool IsCharInWord(char userInput, string word)
    {
        foreach (char c in word)
        {
            if (userInput == c) return true;
        }
        return false;
    }
}

public class AttempManager
{
    private int _maxAttemps;

    public AttempManager() => _maxAttemps = 5;

    public int RemainingAttempts() => _maxAttemps;

    public int UpdateRemainingAttempts(bool isDuplicateLetter, bool isCharInWord)
    {
        if (!isDuplicateLetter && !isCharInWord) --_maxAttemps;    
        return _maxAttemps;
    }
}
