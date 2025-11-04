using ASP_PV411.Services.Random;

namespace ASP_PV411.Services.FolderName
{
    public class FolderNameService(IRandomService _randomService) : IFolderNameService
    {
        public string Name(int? length = null)
        {
            length ??= 16;
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            string allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789-_";

            int lettersCase = _randomService.RandomInt(2); // 0 - lowercase, 1 - uppercase
            char[] chars = new char[length.Value];

            switch (lettersCase)
            {
                case 0:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            chars[i] = (char)(allowedChars[_randomService.RandomInt(allowedChars.Length)]);
                        }
                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            chars[i] = char.ToUpper((allowedChars[_randomService.RandomInt(allowedChars.Length)]));
                        }
                        break;
                    }
            }

            return new string(chars);
        }
    }
}
