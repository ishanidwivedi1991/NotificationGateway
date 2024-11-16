Project structure is divided into four parts:
1. NotificationGateway.Application => This contains business logic
2. NotificationGateway => This contains the microservice WebAPI that various applications and services across infrastructure will be consuming. This also contains file NotificationGateway.http that have sample data and endpoints information
3. NotificationGateway.UI => This contains the UI for the extra bonus section.
4. NotificationGateway.Tests => This contains tests to make sure deliverable is passing the unit tests.


To test the functionality on local, do below steps:
1. Clone the project locally
2. Build and run the solution in visual studio or preferred ID
3. Make sure API project(NotificationGateway) is running along with NotificationGateway.UI. Please make a note of the hostname.
4. Stop the solution
5. Open file Dashboard.cshtml and modify url at line 52.
6. Save, Build and Run the solution.
7. Open Endpoints Explorer in Visual Studio or Postman to do the POST request. Sample data is present in NotificationGateway.http
8. Once POST request is completed successfully, go to front end and enter information for Business Phone Number and Account Number, date/time range
9. The data should be displayed in the charts below
