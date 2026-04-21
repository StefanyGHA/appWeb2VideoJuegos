using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using appWeb2.Models;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace appWeb2.Services
{
	public class PayPalService
	{
		private readonly HttpClient _httpClient;
		private readonly PayPalSettings _payPalSettings;

		public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> payPalSettings)
		{
			_httpClient = httpClient;
			_payPalSettings = payPalSettings.Value;
		}

		public async Task<string?> GetAccessTokenAsync()
		{
			var authToken = Convert.ToBase64String(
				Encoding.UTF8.GetBytes($"{_payPalSettings.ClientId}:{_payPalSettings.ClientSecret}")
			);

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Basic", authToken);

			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials")
			});

			var response = await _httpClient.PostAsync(
				$"{_payPalSettings.BaseUrl}/v1/oauth2/token",
				content
			);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error obteniendo token de PayPal: {error}");
			}

			var json = await response.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<PayPalTokenResponse>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return result?.AccessToken;
		}
	}
	
public class PayPalTokenResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; } = string.Empty;

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; } = string.Empty;

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }
	}
}