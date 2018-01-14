using MyExtensions;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LineBotWebhookCSharp
{
    public class RescureUrl
    {
        // 「近くの緊急避難所を探します」を利用 : だいたい半径3km以内で近くの避難所を教えてくれます
        private static readonly string _searchBaseUrl = "https://0312.yanoshin.jp/rescue/index";

        // 「ナビタイム災害情報」を利用 : kumamoto prefecture が含まれていたら熊本地震とみなしてURL 追加
        private static readonly string _navitimeUrl = "http://www.navitime.co.jp/saigai/?from=pctop";

        // Google クライシスレスポンスを利用 : https://www.google.org/crisisresponse/japan
        // Map 表示もできるけど、位置関係なしなので一旦TOP のみでMapはなし : https://www.google.org/crisisresponse/japan/maps?hl=ja
        private static readonly string _googleChrisisUrl = "https://www.google.org/crisisresponse/japan";

        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public string Address { get; private set; }
        public bool IsKumamoto => IsKumamotoIncluded(this.Address);

        public RescureUrl(string latitude, string longitude, string address)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Address = address;
        }

        public async Task<string> GetAsync()
        {
            var endMessage = new[] {$"送っていただいた現在地情報は次の通りです。",
            $"経度 : {this.Latitude}",
            $"緯度 : {this.Longitude}",
            $"住所 : {this.Address}",
        }.ToJoinedString(Environment.NewLine);
            var requestUrl = new Uri($"{_searchBaseUrl}/{this.Latitude}/{this.Longitude}");
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
                if (res.IsSuccessStatusCode)
                {
                    string postMessage = endMessage;
                    if (this.IsKumamoto)
                    {
                        // 熊本県の住所が入っていたら差し込む
                        postMessage = new[] {$"熊本県の方ですか？「ナビタイム災害情報」もご参考にどうぞ",
                        _navitimeUrl,
                        $"{Environment.NewLine}",
                        endMessage,
                    }
                        .ToJoinedString(Environment.NewLine);
                    }

                    var message = new[] {$@"「近くの緊急避難所を探します」で、検索結果が見つかりました。",
                    $"最寄りの避難所情報 URL : {requestUrl.AbsoluteUri}",
                    Environment.NewLine,
                    $"安否情報はGoogleクライシスレスポンスもどうぞ URL : {_googleChrisisUrl}",
                    Environment.NewLine,
                    postMessage,
                }.ToJoinedString(Environment.NewLine);

                    return message;
                }
                return $"指定した住所がみつかりませんでした。もう一度試していただけますか？{Environment.NewLine}{endMessage}";
            }
        }

        public static bool IsKumamotoIncluded(string address)
        {
            var isEngCultureInvaliant = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.CultureInvariant);
            var isEngIgnoreCase = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.IgnoreCase);
            var isEngIgnoreWhitespace = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.IgnorePatternWhitespace);
            var isJapaneseCultureInvaliat = Regex.IsMatch(address, "熊本県", RegexOptions.CultureInvariant);
            return isEngCultureInvaliant || isEngIgnoreCase || isEngIgnoreWhitespace || isJapaneseCultureInvaliat;
        }
    }
}