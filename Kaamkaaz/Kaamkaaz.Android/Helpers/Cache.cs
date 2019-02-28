using Xamarin.Essentials;

namespace Kaamkaaz.Droid.Helpers
{
    /// <summary>
    /// Defines the <see cref="Cache" />
    /// User xamarin essentials preferences to presist user data on the device.
    /// </summary>
    internal static class Cache {
        internal static int GetUserId()
        {
            return Preferences.Get("userId", 0);
        }
        internal static void SetUserId(int userId)
        {
            Preferences.Set("userId", userId);
        }
    }
}
