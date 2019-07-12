using Newtonsoft.Json;
using SuperTranslator.TranslatorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string host = "https://api.cognitive.microsofttranslator.com";
        private string route = "/translate?api-version=3.0&to=pl&to=en";
        private string subscriptionKey = "1f614f2cdc8e4e6c856f053d5937beec";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Translate_Button(object sender, RoutedEventArgs e)
        {
            string textToTranslate = uk_text.Text;

            Dictionary<string, string> translatedText = await TranslateTextRequest(textToTranslate);

            pl_text.Text = translatedText["pl"];

            en_text.Text = translatedText["en"];
        }

        private void Create_Button(object sender, RoutedEventArgs e)
        {

        }

        private async Task<Dictionary<string, string>> TranslateTextRequest(string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            Dictionary<string, string> resultText = new Dictionary<string, string>();

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(host + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                foreach (TranslationResult o in deserializedOutput)
                {
                    //Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    foreach (Translation t in o.Translations)
                    {
                        resultText.Add(t.To, t.Text);
                    }
                }
            }

            return resultText;
        }
    }
}
