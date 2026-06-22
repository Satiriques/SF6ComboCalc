using SF6ComboCalculator;
using SF6ComboCalculator.Combo;
using Con = System.Console;

namespace SF6ComboCalculator.ConsoleApp;

/// <summary>
/// A small line-editor REPL for building combos. As the user types it re-renders the
/// damage table live, offers ghost-text of the best matching move, and completes the
/// move under the cursor with Tab (cycling through matches on repeated Tab).
/// </summary>
internal sealed class InteractiveSession
{
    private const string Prompt = "combo> ";
    private static readonly char[] Separators = { '>', ',' };

    private readonly string _character;
    private readonly ComboParser _parser;
    private readonly IReadOnlyList<string> _suggestions;
    private readonly int _level;
    private readonly int _stocks;

    private string _buffer = string.Empty;
    private int _cursor;
    private int _anchorTop;
    private int _prevLineCount;

    // Tab-completion cycling state (reset whenever a non-Tab key is pressed).
    private bool _completing;
    private string _compHead = string.Empty;
    private string _compTail = string.Empty;
    private List<string> _compMatches = new();
    private int _compIndex;

    public InteractiveSession(string character, int level, int stocks, string seed)
    {
        _character = character;
        _level = level;
        _stocks = stocks;
        _parser = ComboParser.From(character);
        _suggestions = _parser.GetMoveSuggestions();
        _buffer = seed;
        _cursor = seed.Length;
    }

    public int Run()
    {
        if (Con.IsInputRedirected)
        {
            Con.Error.WriteLine("Interactive mode requires a real terminal (stdin is redirected).");
            return 1;
        }

        Con.WriteLine($"Interactive combo builder for '{_character}'.");
        Con.WriteLine("Type a combo; Tab completes/cycles moves, Enter clears, Esc or Ctrl+C exits.");
        Con.WriteLine();

        _anchorTop = Con.CursorTop;

        Render();
        while (true)
        {
            var key = Con.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape ||
                (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.C))
            {
                break;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                HandleTab(forward: !key.Modifiers.HasFlag(ConsoleModifiers.Shift));
                Render();
                continue;
            }

            // Any non-Tab key ends a completion-cycle session.
            _completing = false;

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    _buffer = string.Empty;
                    _cursor = 0;
                    break;
                case ConsoleKey.Backspace:
                    if (_cursor > 0)
                    {
                        _buffer = _buffer.Remove(_cursor - 1, 1);
                        _cursor--;
                    }
                    break;
                case ConsoleKey.Delete:
                    if (_cursor < _buffer.Length)
                        _buffer = _buffer.Remove(_cursor, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    if (_cursor > 0) _cursor--;
                    break;
                case ConsoleKey.RightArrow:
                    // At end of line with a ghost showing, RightArrow accepts the ghost.
                    if (_cursor == _buffer.Length)
                    {
                        var ghost = GhostText();
                        if (ghost.Length > 0)
                        {
                            _buffer += ghost;
                            _cursor = _buffer.Length;
                        }
                    }
                    else
                    {
                        _cursor++;
                    }
                    break;
                case ConsoleKey.Home:
                    _cursor = 0;
                    break;
                case ConsoleKey.End:
                    _cursor = _buffer.Length;
                    break;
                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        _buffer = _buffer.Insert(_cursor, key.KeyChar.ToString());
                        _cursor++;
                    }
                    break;
            }

            Render();
        }

        // Leave the cursor below the rendered region on exit.
        SafeSetCursor(0, _anchorTop + _prevLineCount);
        Con.WriteLine();
        return 0;
    }

    // ---- completion -------------------------------------------------------

    /// <summary>The (start, prefix) of the move token the cursor is currently in.</summary>
    private (int start, string prefix) CurrentToken()
    {
        var start = 0;
        for (var i = 0; i < _cursor; i++)
        {
            if (Array.IndexOf(Separators, _buffer[i]) >= 0)
                start = i + 1;
        }

        var prefix = _buffer.Substring(start, _cursor - start);
        return (start, prefix);
    }

    private List<string> MatchesFor(string prefix)
    {
        var trimmed = prefix.TrimStart();
        return _suggestions
            .Where(s => s.StartsWith(trimmed, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    /// <summary>Ghost text shown after the cursor (remainder of the best match).</summary>
    private string GhostText()
    {
        if (_cursor != _buffer.Length) return string.Empty;
        var (_, prefix) = CurrentToken();
        var trimmed = prefix.TrimStart();
        if (trimmed.Length == 0) return string.Empty;

        var match = MatchesFor(prefix)
            .FirstOrDefault(s => s.Length > trimmed.Length &&
                                 s.StartsWith(trimmed, StringComparison.InvariantCultureIgnoreCase));
        if (match is null) return string.Empty;
        return match.Substring(trimmed.Length);
    }

    private void HandleTab(bool forward)
    {
        if (_completing && _compMatches.Count > 0)
        {
            _compIndex = (_compIndex + (forward ? 1 : -1) + _compMatches.Count) % _compMatches.Count;
            ApplyCompletion();
            return;
        }

        var (start, prefix) = CurrentToken();
        var leadingWs = prefix.Length - prefix.TrimStart().Length;
        var matches = MatchesFor(prefix);
        if (matches.Count == 0) return;

        _completing = true;
        _compHead = _buffer.Substring(0, start + leadingWs);
        _compTail = _buffer.Substring(_cursor);
        _compMatches = matches;
        _compIndex = 0;
        ApplyCompletion();
    }

    private void ApplyCompletion()
    {
        var word = _compMatches[_compIndex];
        _buffer = _compHead + word + _compTail;
        _cursor = _compHead.Length + word.Length;
    }

    // ---- rendering --------------------------------------------------------

    private void Render()
    {
        var ghost = GhostText();
        var body = BuildBody();

        Con.CursorVisible = false;
        var width = Math.Max(1, Con.WindowWidth);
        var lineCount = 1 + body.Count;

        // Input line (line 0): prompt + buffer in normal colour, ghost in dim grey.
        SafeSetCursor(0, _anchorTop);
        var head = Prompt + _buffer;
        if (head.Length > width - 1) head = head.Substring(0, width - 1);
        Con.Write(head);
        var remaining = width - 1 - head.Length;
        if (ghost.Length > 0 && remaining > 0)
        {
            var shown = ghost.Length > remaining ? ghost.Substring(0, remaining) : ghost;
            var prev = Con.ForegroundColor;
            Con.ForegroundColor = ConsoleColor.DarkGray;
            Con.Write(shown);
            Con.ForegroundColor = prev;
            remaining -= shown.Length;
        }
        if (remaining > 0) Con.Write(new string(' ', remaining));

        // Body (table / messages).
        for (var i = 0; i < body.Count; i++)
        {
            SafeSetCursor(0, _anchorTop + 1 + i);
            var line = body[i];
            if (line.Length > width - 1) line = line.Substring(0, width - 1);
            Con.Write(line.PadRight(width - 1));
        }

        // Clear lines that existed in the previous render but not this one.
        for (var i = lineCount; i < _prevLineCount; i++)
        {
            SafeSetCursor(0, _anchorTop + i);
            Con.Write(new string(' ', width - 1));
        }

        _prevLineCount = lineCount;

        // Place the real cursor inside the input line, after the prompt.
        SafeSetCursor(Math.Min(Prompt.Length + _cursor, width - 1), _anchorTop);
        Con.CursorVisible = true;
    }

    private List<string> BuildBody()
    {
        var lines = new List<string> { string.Empty };

        if (string.IsNullOrWhiteSpace(_buffer))
        {
            lines.Add("(type a combo to see the breakdown)");
            return lines;
        }

        try
        {
            var result = _parser.Parse(_buffer, new CharacterStates { Level = _level, NumberOfStocks = _stocks });
            lines.AddRange(TableRenderer.Render(_character, _buffer, _level, _stocks, result));
        }
        catch (Exception ex)
        {
            lines.Add($"! {ex.Message}");
        }

        return lines;
    }

    private static void SafeSetCursor(int left, int top)
    {
        top = Math.Clamp(top, 0, Math.Max(0, Con.BufferHeight - 1));
        left = Math.Clamp(left, 0, Math.Max(0, Con.BufferWidth - 1));
        Con.SetCursorPosition(left, top);
    }
}
