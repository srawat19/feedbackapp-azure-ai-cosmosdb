using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure;

namespace feedbackApp.Services;
public class SentimentService
{

    //Get Key from Azure KeyVault 
    string keyVaultUrl = "https://vault-app-jsdaxzya42jta.vault.azure.net/";
    string secretName = "textAnalyticsKey-jsdaxzya42jta";

    string textAnalyticsUri = "https://textanalytics-service-feedback-jsdaxzya42jta.cognitiveservices.azure.com/";

    private TextAnalyticsClient textAnalyticsClient;

    public SentimentService()
    {
        SecretClient secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        KeyVaultSecret secret = secretClient.GetSecret(secretName);

        AzureKeyCredential keyCredential = new AzureKeyCredential(secret.Value);

        textAnalyticsClient = new TextAnalyticsClient(new Uri(textAnalyticsUri), keyCredential);

    }


    public async Task<DocumentSentiment> GetSentimentAsync(string text)
    {
        //Detect Language as text can be any language.
        Response<DetectedLanguage> detectedLangResp = await textAnalyticsClient.DetectLanguageAsync(text);
        string language = detectedLangResp.Value.Iso6391Name;

        //Analyse sentiment - Positive, Negative, Neutral considering the language.
        return await textAnalyticsClient.AnalyzeSentimentAsync(text, language);

    }

}