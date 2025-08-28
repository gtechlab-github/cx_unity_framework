using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public class FirebaseUtils {
    private const string PushChars = "-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
    private static long lastPushTime = 0;
    private static readonly int[] lastRandChars = new int[12];

    public static string GenerateKey () {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds ();
        var duplicateTime = (now == lastPushTime);
        lastPushTime = now;

        var sb = new StringBuilder (20);
        var timeStampChars = new char[8];
        for (int i = 7; i >= 0; i--) {
            timeStampChars[i] = PushChars[(int) (now % 64)];
            now = now / 64;
        }
        sb.Append (timeStampChars);

        if (!duplicateTime) {
            for (int i = 0; i < 12; i++) {
                lastRandChars[i] = new Random ().Next (0, 64);
            }
        } else {
            int i;
            for (i = 11; i >= 0 && lastRandChars[i] == 63; i--) {
                lastRandChars[i] = 0;
            }
            lastRandChars[i]++;
        }

        for (int i = 0; i < 12; i++) {
            sb.Append (PushChars[lastRandChars[i]]);
        }

        return sb.ToString ();
    }

    public static Dictionary<string, object> ToDictionary (object inputObj) {
        string jsonString = JsonConvert.SerializeObject (inputObj);
        Dictionary<string, object> inputDict = JsonConvert.DeserializeObject<Dictionary<string, object>> (jsonString);
        Dictionary<string, object> resultDict = new Dictionary<string, object> ();

        foreach (var item in inputDict) {
            if (item.Value == null)
                continue;

            if (item.Value is System.DateTime dateTimeValue) {
                // DateTime을 ISO 8601 문자열로 변환
                string isoDateString = dateTimeValue.ToString ("o"); // "o"는 ISO 8601 형식
                resultDict.Add (item.Key, isoDateString);
            } else {
                resultDict.Add (item.Key, item.Value);
            }
        }

        return resultDict;
    }

    
    public static TSocialType ProviderIdToSocialType(string providerId) {
        if (providerId == "google.com") {
            return TSocialType.Google;
        } else if (providerId == "facebook.com") {
            return TSocialType.Facebook;
        } else if (providerId == "anonymous") {
            return TSocialType.Guest;
        } else if (providerId == "password") {
            return TSocialType.EMail;
        } else if (providerId == "apple.com") {
            return TSocialType.Apple;
        } else if (providerId == "Firebase") {
            return TSocialType.EMail;
        } else {
            Debug.LogWarning("Unknown providerId: " + providerId);
            return TSocialType.Undef;
        }
    }
}
