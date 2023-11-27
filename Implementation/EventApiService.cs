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
                            if (TryParseEventsApiResponse(json, out var eventsResponse))
                            {
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
                            }
                            else
                            {
                                // Handle parsing error
                                return new ObjectResult(new { message = "Error parsing API response" })
                                {
                                    StatusCode = (int)HttpStatusCode.InternalServerError
                                };
                            }

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.TooManyRequests:
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.GatewayTimeout:
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            LogApiError(response.StatusCode, errorMessage);
                            return HandleErrorResponse(response.StatusCode, errorMessage);

                        default:
                            LogApiError(response.StatusCode, "Unexpected API response");
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
                LogApiError(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        private bool TryParseEventsApiResponse(string json, out EventsApiResponse eventsResponse)
        {
            try
            {
                eventsResponse = JsonConvert.DeserializeObject<EventsApiResponse>(json);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parsing JSON response: {ex.Message}");
                eventsResponse = null;
                return false;
            }
        }

        private void LogApiError(HttpStatusCode statusCode, string errorMessage)
        {
            _logger.LogError($"API error - Status Code: {statusCode}, Message: {errorMessage}");
        }

        private ActionResult HandleErrorResponse(HttpStatusCode statusCode, string errorMessage)
        {
            var defaultErrorMessage = errorMessage ?? "Unknown error";

            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    return new BadRequestObjectResult(defaultErrorMessage);

                case HttpStatusCode.TooManyRequests:
                    return new ObjectResult(new { message = defaultErrorMessage })
                    {
                        StatusCode = (int)HttpStatusCode.TooManyRequests
                    };

                case HttpStatusCode.InternalServerError:
                    return new ObjectResult(new { message = defaultErrorMessage })
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };

                case HttpStatusCode.GatewayTimeout:
                    return new ObjectResult(new { message = defaultErrorMessage })
                    {
                        StatusCode = (int)HttpStatusCode.GatewayTimeout
                    };

                default:
                    return new ObjectResult(new { message = defaultErrorMessage })
                    {
                        StatusCode = (int)statusCode
                    };
            }
        }
    }
}
