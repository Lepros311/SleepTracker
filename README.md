# Sleep Tracker

The [Sleep Tracker](https://www.thecsharpacademy.com/project/33/sleep-tracker) project is the second project I've done that using Angular (aside from the Angular tutorial project). The app allows a user to track their sleep and calculates the duration. It includes a timer they can use which automatically creates a sleep record when stopped. They can also input a sleep record manually. This project is part of [The C# Academy](https://www.thecsharpacademy.com/) curriculum.

## Requirements

- [x] This is an application where you should record sleep time.
- [x] You should create two projects: An ASP.NET Core/EntityFramework WebApi and an Angular app.
- [x] You need to use Angular Material.
- [x] You should have a filter functionality, so I can show records per type and or date.
- [x] Your database should have a single 'Records' table.The objective is to focus on Angular, so we should avoid the complexities of relational data.
- [ ] This app needs to have a timer that will log your sleep once stopped and saved.
- [ ] Users should also be able to input their sleep time using a form.
- [ ] Every action performed in the app should prompt a 'Toast' notification indicating success or failure.
- [ ] Your list of sleep records should have pagination, so you're not loading all records every time you visualise the list.
- [ ] This app should be mobile-first, since realistically , most users will use it from their phones.
- [ ] Your app should contain integration tests for the services that are interacting with the database, and unit tests where you find applicable.
- [ ] Your repository should contain a Postman collection with all endpoints documented for easy testing.

## Running the App

1. In the terminal, navigate to the API folder (SleepTracker > SleepTracker.Api) and execute the following:  
`dotnet run --urls "https://localhost:7288`

2. Then open a separate terminal and navigate to the frontend folder (SleepTracker > SleepTracker.Angular) and execute the following:  
`ng serve`

3. In your web browser, go to http://localhost:4200