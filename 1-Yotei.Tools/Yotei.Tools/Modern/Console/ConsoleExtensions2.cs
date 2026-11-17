using Color = System.ConsoleColor;
namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled,.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
           string? source)
           => EditLineEx(false, source, [], Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, Color forecolor)
            => EditLineEx(false, source, [], Timeout.InfiniteTimeSpan, forecolor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, Color forecolor, Color backcolor)
            => EditLineEx(false, source, [], Timeout.InfiniteTimeSpan, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled,.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public static string? EditLine(
           string? source, IEnumerable<string> candidates)
           => EditLineEx(false, source, candidates, Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, IEnumerable<string> candidates, Color forecolor)
            => EditLineEx(false, source, candidates, Timeout.InfiniteTimeSpan, forecolor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, IEnumerable<string> candidates,
            Color forecolor, Color backcolor)
            => EditLineEx(false, source, candidates, Timeout.InfiniteTimeSpan, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout)
            => EditLineEx(false, source, [], timeout, Console.ForegroundColor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout, Color forecolor)
            => EditLineEx(false, source, [], timeout, forecolor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout, Color forecolor, Color backcolor)
            => EditLineEx(false, source, [], timeout, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLine(
           string? source, IEnumerable<string> candidates, TimeSpan timeout)
           => EditLineEx(false, source, candidates, timeout, Console.ForegroundColor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, IEnumerable<string> candidates, TimeSpan timeout, Color forecolor)
            => EditLineEx(false, source, candidates, timeout, forecolor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, IEnumerable<string> candidates,
            TimeSpan timeout, Color forecolor, Color backcolor)
            => EditLineEx(false, source, candidates, timeout, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled,.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
           bool debug, string? source)
           => EditLineEx(debug, source, [], Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, Color forecolor)
            => EditLineEx(debug, source, [], Timeout.InfiniteTimeSpan, forecolor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, Color forecolor, Color backcolor)
            => EditLineEx(debug, source, [], Timeout.InfiniteTimeSpan, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled,.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public static string? EditLineEx(
           bool debug,
           string? source, IEnumerable<string> candidates)
           => EditLineEx(debug, source, candidates, Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, IEnumerable<string> candidates, Color forecolor)
            => EditLineEx(debug, source, candidates, Timeout.InfiniteTimeSpan, forecolor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled.
        /// Otherwise, returns the editted value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, IEnumerable<string> candidates,
            Color forecolor, Color backcolor)
            => EditLineEx(debug, source, candidates, Timeout.InfiniteTimeSpan, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, TimeSpan timeout)
            => EditLineEx(debug, source, [], timeout, Console.ForegroundColor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, TimeSpan timeout, Color forecolor)
            => EditLineEx(debug, source, [], timeout, forecolor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, TimeSpan timeout, Color forecolor, Color backcolor)
            => EditLineEx(debug, source, [], timeout, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLineEx(
           bool debug,
           string? source, IEnumerable<string> candidates, TimeSpan timeout)
           => EditLineEx(debug, source, candidates, timeout, Console.ForegroundColor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, IEnumerable<string> candidates, TimeSpan timeout, Color forecolor)
            => EditLineEx(debug, source, candidates, timeout, forecolor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string or, if it is null, an empty initial one
        /// is used instead. Returns <see langword="null"/> if such edition was cancelled, or if
        /// the given timeout period expired without activity. Otherwise, returns the editted
        /// value, which may also be an empty one.
        /// <br/> The standard editing keys can be used to move across the value being editted.
        /// [Enter] accepts the value, and [Esc] cancels the edition. Underline mode for block
        /// editing is not supported.
        /// <br/> If the given candidates' collection is not an empty one, [Tab], [Up] and [Down]
        /// are used to enter into candidate selection mode, and to navigate though the options
        /// (which are the candidates filtered using the characters already typed). Then, [Enter]
        /// accepts the selected candidate and [Esc] cancels the selection mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="candidates"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            string? source, IEnumerable<string> candidates,
            TimeSpan timeout, Color forecolor, Color backcolor)
        {
            candidates.ThrowWhenNull();
            PreventBufferOverflow(5); // 5 by convention...

            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            int oldsize = Console.CursorSize;
            int oldleft = Console.CursorLeft;
            bool insert = false;
            bool oldinsert = false; SetInsertMode(oldinsert);

            var sb = new StringBuilder(source ?? string.Empty);
            int pos = sb.Length;
            int used = sb.Length;

            int index = -1;
            string? filter = null;
            List<string>? tentatives = null;

            // Iterating...
            ShowLine();
            while (true)
            {
                var info = ReadKey(timeout, hide: true);
                info ??= new('\0', ConsoleKey.Escape, false, false, false);

                // Special keys...
                switch (info.Value.Key)
                {
                    case ConsoleKey.Enter:
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        ShowLine(); Console.WriteLine();
                        SetInsertMode(oldinsert);
                        TryDebugWriteLine(debug, sb.ToString());
                        return sb.ToString();

                    case ConsoleKey.Escape:
                        if (index >= 0) { ExitCandidateMode(false); break; }
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        sb.Clear();
                        ShowLine(); Console.WriteLine();
                        SetInsertMode(oldinsert);
                        return null;

                    case ConsoleKey.Insert:
                        if (index < 0) SetInsertMode(!insert);
                        break;

                    case ConsoleKey.Home:
                        if (index >= 0) ExitCandidateMode(true);
                        Console.CursorLeft = oldleft;
                        pos = 0;
                        break;

                    case ConsoleKey.End:
                        if (index >= 0) ExitCandidateMode(true);
                        pos = sb.Length;
                        Console.CursorLeft = oldleft + pos;
                        break;

                    case ConsoleKey.Backspace:
                        if (index >= 0) { ExitCandidateMode(false); break; }
                        if (pos == 0) break;
                        sb.Remove(--pos, 1);
                        ShowLine();
                        break;

                    case ConsoleKey.Delete:
                        if (index >= 0) { ExitCandidateMode(false); break; }
                        if (pos >= sb.Length) break;
                        sb.Remove(pos, 1);
                        ShowLine();
                        break;

                    case ConsoleKey.LeftArrow:
                        if (index >= 0) ExitCandidateMode(true);
                        if (pos == 0) break;
                        if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            do
                            {
                                pos--; Console.CursorLeft--; if (pos == 0) break;
                                if (char.IsLetterOrDigit(sb[pos]) &&
                                    !char.IsLetterOrDigit(sb[pos - 1])) break;
                            }
                            while (true);
                        }
                        else { pos--; Console.CursorLeft--; }
                        break;

                    case ConsoleKey.RightArrow:
                        if (index >= 0) ExitCandidateMode(true);
                        if (pos >= sb.Length) break;
                        if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            do
                            {
                                pos++; Console.CursorLeft++; if (pos == sb.Length) break;
                                if (char.IsLetterOrDigit(sb[pos]) &&
                                    !char.IsLetterOrDigit(sb[pos - 1])) break;
                            }
                            while (true);
                        }
                        else { pos++; Console.CursorLeft++; }
                        break;

                    case ConsoleKey.UpArrow:
                        if (index < 0) { EnterCandidateMode(); break; }
                        if (index >= 0)
                        {
                            index--; if (index < 0) index = tentatives!.Count - 1;
                            sb = new StringBuilder(tentatives![index]);
                            pos = sb.Length;
                            ShowLine();
                        }
                        break;

                    case ConsoleKey.Tab:
                    case ConsoleKey.DownArrow:
                        if (index < 0) { EnterCandidateMode(); break; }
                        if (index >= 0)
                        {
                            index++; if (index >= tentatives!.Count) index = 0;
                            sb = new StringBuilder(tentatives![index]);
                            pos = sb.Length;
                            ShowLine();
                        }
                        break;
                }

                // Other keys...
                if (info.Value.KeyChar >= ' ')
                {
                    if (index >= 0) ExitCandidateMode(true);

                    if (insert) // Insert mode...
                    {
                        sb.Insert(pos, info.Value.KeyChar); pos++;
                        ShowLine();
                        continue;
                    }

                    if (pos < sb.Length) // Overwrite mode...
                    {
                        sb[pos] = info.Value.KeyChar; pos++;
                        ShowLine();
                        continue;
                    }
                    else // Add mode...
                    {
                        sb.Append(info.Value.KeyChar); pos++;
                        ShowLine();
                        continue;
                    }
                }
            }

            /// <summary>
            /// Invoked to enter into candidate selection mode, if such is possible.
            /// </summary>
            void EnterCandidateMode()
            {
                if (index >= 0) return;

                var comparison = StringComparison.OrdinalIgnoreCase;
                filter = sb.ToString();
                tentatives = [.. candidates.Select(x => x.NotNullNotEmpty(trim: true))];
                tentatives = filter.Length == 0
                    ? tentatives
                    : [.. tentatives.Where(x => x.Contains(filter, comparison))];

                if (tentatives.Count == 0) return;
                tentatives.Sort(StringComparer.OrdinalIgnoreCase);

                index = 0;
                sb = new StringBuilder(tentatives[0]);
                pos = sb.Length;

                Console.ForegroundColor = AlternateColor(Console.ForegroundColor);
                Console.BackgroundColor = AlternateColor(Console.BackgroundColor);
                ShowLine();
            }

            /// <summary>
            /// Invoked to exit candidate selection mode. If <c>valid</c> is requested, then the
            /// current candidate is copied into the value being editted.
            /// </summary>
            void ExitCandidateMode(bool valid)
            {
                if (index < 0) return;

                Console.ForegroundColor = forecolor;
                Console.BackgroundColor = backcolor;
                sb.Clear();
                ShowLine();

                sb.Append(valid ? tentatives![index] : filter);
                pos = sb.Length;
                ShowLine();

                index = -1;
                filter = null;
                tentatives = null;
            }

            /// <summary>
            /// Invoked to show the current line.
            /// </summary>
            void ShowLine()
            {
                // Prints current contents...
                Console.CursorLeft = oldleft;
                Console.Write(sb.ToString());

                // Clear from current to the used ones...
                var temp = used - sb.Length; if (temp > 0)
                {
                    Color fore = Console.ForegroundColor; Console.ForegroundColor = oldfore;
                    Color back = Console.BackgroundColor; Console.BackgroundColor = oldback;
                    var str = FromSpaces(temp);
                    Console.Write(str);
                    Console.ForegroundColor = fore;
                    Console.BackgroundColor = back;
                }

                // Position cursor amd capture current used characters...
                Console.CursorLeft = oldleft + pos;
                used = sb.Length;
            }

            /// <summary>
            /// Invoked to set the value of the insert mode (just changes the cursor size).
            /// </summary>
            void SetInsertMode(bool value)
            {
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (windows)
                {
                    Console.CursorSize = value ? 100 : oldsize;
                    insert = value;
                }
            }
        }
    }
}