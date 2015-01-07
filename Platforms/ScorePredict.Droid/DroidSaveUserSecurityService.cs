using Android.Preferences;
using ScorePredict.Common.Injection;
using ScorePredict.Data;
using ScorePredict.Services;
using Xamarin.Forms;

namespace ScorePredict.Droid
{
    public class DroidSaveUserSecurityService : ISaveUserSecurityService
    {
        private readonly IEncryptionService _encryptionService;

        public DroidSaveUserSecurityService()
        {
            _encryptionService = Resolver.CurrentResolver.Get<IEncryptionService>();
        }
        public void SaveUser(User user)
        {
            var editor = PreferenceManager.GetDefaultSharedPreferences(Forms.Context).Edit();
            editor.PutString(AndroidConstants.SharedPrefsUserIdKey, 
                _encryptionService.Encrypt(user.UserId));
            editor.PutString(AndroidConstants.SharedPrefsTokenKey,
                _encryptionService.Encrypt(user.AuthToken));
            editor.Commit();
        }
    }
}