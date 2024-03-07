
## The API will start, and you can access the Swagger UI via https://localhost:7138/swagger/index.html or the port configured in your environment.


## Using the API

1) Submit Provider Availability: POST /providers/availability

	Allows providers to submit their available slots.
    **Example Request:
    	{
		  "providerId": 1,
		  "startTime": "2024-03-12T08:00:00Z",
		  "duration": "00:15:00"
		}

2) View Available Appointment Slots: GET /appointmentSlots/available

	Clients can view all available appointment slots.

3) Reserve an Appointment Slot: POST /reservations

	Allows clients to reserve an available slot.
    **Example Request:
	   {
		  "appointmentSlotId": 1,
		  "clientId": 1
		}

4) Confirm a Reservation: PUT /reservations/confirm/{id}

	Clients can confirm their reservation.

Swagger UI is enabled for development, providing an interactive documentation interface at /swagger.

## Development Notes:

- The API is built using .NET 8 minimal apis, and uses an in-memory database for simplicity. DataSeeders are used to seed the database with some initial data. 
- For the 1st endpoint, this could be enhanced to allow providers to submit their availability for a specific date/time range instead of 1 slot at a time. 
- Some initial Model data validation has been added but could be enhanced further. 
- The API is not secured, and would need to be enhanced for production use.
- Additional time would also provide more extensive testing and error handling. 
- Request and Response models are typically used as DTOs to separate the API from the domain models and would have been added utilizing automapper if time permitted. 

