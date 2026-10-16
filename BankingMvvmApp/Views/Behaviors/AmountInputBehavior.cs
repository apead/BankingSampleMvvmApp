namespace BankingApp.Views.Behaviors;

public class AmountInputBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(entry);
    }

    private static void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue) && !IsValidInput(e.NewTextValue))
        {
            entry.Text = e.OldTextValue;
        }
    }

    private static bool IsValidInput(string text)
    {
        int separatorCount = 0;
        int decimalDigits = 0;

        foreach (char character in text)
        {
            if (character == '.' || character == ',')
            {
                if (++separatorCount > 1)
                {
                    return false;
                }
            }
            else if (char.IsDigit(character))
            {
                if (separatorCount == 1 && ++decimalDigits > 2)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
