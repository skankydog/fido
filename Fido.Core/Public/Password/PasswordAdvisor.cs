// http://stackoverflow.com/questions/12899876/checking-strings-for-a-strong-enough-password
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fido.Core
{
    public enum PasswordScore
    {
        Invalid = 0,
        VeryWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }

    public static class PasswordAdvisor
    {
        public static PasswordScore CheckStrength(string Password)
        {
            if (Password == null || Password.Length == 0)
                return PasswordScore.Invalid;

            if (Password.Length < 1 ||
                Password.Contains(' '))
                return PasswordScore.Invalid;

            if (Password.Length < 4)
                return PasswordScore.Invalid;

            int Score = (int)PasswordScore.Invalid;

            if (Password.Length >= 8)
                Score++;
            
            if (Password.Length >= 12)
                Score++;
            
            // Alpha and numeric
            if (Regex.Match(Password, @"^.*[0-9].*$", RegexOptions.ECMAScript).Success &&
                   (Regex.Match(Password, @"^.*[a-z].*$", RegexOptions.ECMAScript).Success ||
                    Regex.Match(Password, @"^.*[A-Z].*$", RegexOptions.ECMAScript).Success))
                Score++;
            
            // Upper and lowercase
            if (Regex.Match(Password, @"^.*[a-z].*$", RegexOptions.ECMAScript).Success &&
                Regex.Match(Password, @"^.*[A-Z].*$", RegexOptions.ECMAScript).Success)
                Score++;
            
            // Contains special characters
            if (Regex.Match(Password, @"^.*[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)].*$", RegexOptions.ECMAScript).Success)
                Score++;

            return (PasswordScore)Score;
        }

        public static bool MediumOrHigher(string Password)
        {
            return CheckStrength(Password) >= PasswordScore.Medium;
        }

        public static bool WeakOrInvalid(string Password)
        {
            return CheckStrength(Password) <= PasswordScore.Weak;
        }
    }
}
