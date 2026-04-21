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

		public async Task<string?> CreateOrderAsync(decimal amount)
		{
			var accessToken = await GetAccessTokenAsync();

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", accessToken);

			var order = new
			{
				intent = "CAPTURE",
				purchase_units = new[]
				{
			new
			{
				amount = new
				{
					currency_code = "USD",
					value = amount.ToString("F2")
				}
			}
		}
			};

			var content = new StringContent(
				JsonSerializer.Serialize(order),
				Encoding.UTF8,
				"application/json"
			);

			var response = await _httpClient.PostAsync(
				$"{_payPalSettings.BaseUrl}/v2/checkout/orders",
				content
			);

			var json = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error creando orden: {json}");
			}

			var result = JsonSerializer.Deserialize<JsonElement>(json);

			return result.GetProperty("id").GetString();
		}

		public async Task<string> CaptureOrderAsync(string orderId)
		{
			var accessToken = await GetAccessTokenAsync();

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", accessToken);

			var response = await _httpClient.PostAsync(
				$"{_payPalSettings.BaseUrl}/v2/checkout/orders/{orderId}/capture",
				new StringContent("", Encoding.UTF8, "application/json")
			);

			var json = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error capturando orden: {json}");
			}

			return json;
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