using System.Net;
using JRNI.EventAPI.Interface;
using JRNI.EventAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JRNI.EventAPI.Implementation
{
    public class EventApiService : IEventApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EventApiService> _logger;

        public EventApiService(IHttpClientFactory httpClientFactory, ILogger<EventApiService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ActionResult<IEnumerable<Event>>> GetFutureEventsAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new BadRequestObjectResult("Email parameter is required.");
                }

                var httpClient = _httpClientFactory.CreateClient("EventApi");

                using (var response = await httpClient.GetAsync($"events?email={email}"))
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var json = await response.Content.ReadAsStringAsync();
                            var eventsResponse = JsonConvert.DeserializeObject<EventsApiResponse>(json);

                            var futureEvents = eventsResponse.Events
                                .Where(e => e.Status == "Busy" || e.Status == "OutOfOffice")
                                .ToList();

                            var updatedResponse = new EventsApiResponse
                            {
                                Email = email,
                                Number_of_events = futureEvents.Count,
                                Events = futureEvents.ToList(), // Ensure a new list instance
                            };

                            _logger.LogInformation("API call is successful");
                            return new OkObjectResult(updatedResponse);

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.TooManyRequests:
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.GatewayTimeout:
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            //var errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(jsonError);
                            _logger.LogError(
                                $"API error - Status Code: {response.StatusCode}, Message: {errorMessage}");
                            return new ObjectResult(new { message = errorMessage ?? "Unknown error" })
                            {
                                StatusCode = (int)response.StatusCode
                            };

                        default:
                            _logger.LogError($"Unexpected API response - Status Code: {response.StatusCode}");
                            return new ObjectResult(new { message = "Unexpected error" })
                            {
                                StatusCode = (int)response.StatusCode
                            };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception using ILogger
                _logger.LogError($"An error occurred: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
}
