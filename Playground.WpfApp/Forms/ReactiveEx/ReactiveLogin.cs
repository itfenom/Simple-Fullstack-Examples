using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public class ReactiveLogin
    {
        private Dictionary<string, string> _userCredentials;
        public string Email { get; set; }
        public string Password { get; set; }

        public async Task<bool> DoLogin()
        {
            _userCredentials = new Dictionary<string, string>
            {
                {"kashif@mubarak.com", "let me in"},
                {"user2@test.com", "test2"},
                {"user3@test.com", "test3"},
                {"t@t.com", "ttt"},
            };

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                return false;

            await Task.Delay(2000);

            var userNameExists = _userCredentials.ContainsKey(Email);
            if (!userNameExists) return false;

            var passwordMatched = _userCredentials[Email] == Password;
            return userNameExists && passwordMatched;
        }
    }
}
