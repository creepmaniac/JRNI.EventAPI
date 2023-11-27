# JRNI .Net Core API Assignment

## Overview

This .NET Core API application serves as a client for an external API that provides information about events. The goal is to expose a single GET endpoint that takes an email parameter, consumes the external API, and returns a filtered list of future events with statuses "Busy" or "OutOfOffice."

## Features

- **GET Endpoint**:
  - **Endpoint:** `/api/events`
  - **Parameters:** 
    - `email` (string): The email address for which events need to be retrieved. Supported values:
      - dave@example.com
      - steve@example.com
      - chris@example.com
      - jane@example.com
  - **Example Request:** `http://localhost:8080/api/events?email=dave@example.com`

- **Response Format**:
  - The API returns a JSON object with the following keys:
    - `Email` (string): The email parameter passed in the request.
    - `Number_of_events` (int): The count of future events.
    - `Events` (array): An array of events with the keys:
      - `Subject` (string): The subject of the event.
      - `Status` (string): The status of the event.
      - `Start_time` (DateTime): The start time of the event.
      - `End_time` (DateTime): The end time of the event.
      - `Id` (string): The unique identifier of the event.

- **Error Handling**:
  - The API handles various response codes from the external API and provides meaningful error messages.
  - Possible response codes: 200, 400, 429, 500, 503.

## Setup and Usage

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/your-repository.git
2. **Build and Run the Application:**
   ```bash
   cd your-repository
   dotnet build
   dotnet run   
3. **Test the API:**
   - The API will be accessible at http://localhost:8080.
   - Use tools like Postman or curl for testing different email parameters

## Deployment Considerations

 - **Production-Ready Configuration:**
    Update the API base address in appsettings.json or use environment variables for production.
    "BaseUrl" in "EventApiSettings": "https://1uf1fhi7yk.execute-api.eu-west-2.amazonaws.com/default/"
   
 - **SSL Certificate:**
    In a production environment, obtain a valid SSL certificate for secure communication.
   
 - **Logging:**
    Implement proper logging mechanisms for error tracking and monitoring.
