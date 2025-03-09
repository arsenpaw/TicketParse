using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Keyborads
{
    internal class ReplyKeyboards
    {
        public static ReplyKeyboardMarkup BaseHomeKeyboard = new ReplyKeyboardMarkup(new[]
{
            new KeyboardButton[] { "Select office", "Check" },
        });

        public static ReplyKeyboardMarkup RichHomeKeyboard = new ReplyKeyboardMarkup(new[]
{
            new KeyboardButton[] { "Select office", "Check" },
            new KeyboardButton[] { "Unsubscribe" }
        });
    }
}
