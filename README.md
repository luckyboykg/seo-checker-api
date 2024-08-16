
**How to start this project?**

You can run this project with the profile: `https` to start the local web API server with port **7286**.

The startup project is `SeoCheckerApi`.

**Introduction:**

I started this ASP.NET Core Web API project with .NET 8, and the unit test tool is xUnit.

I used the Clean Architecture with CQRS pattern.

Yes, I know this project is very small to apply this way, but I want to showcase that when the system grows, we can benefit from this design: Separation of Concerns, Scalability, Flexibility, Improved Testing, and Maintainability.

To optimize performance, I applied the `IMemoryCache` to cache every request to Google or Bing in 1 hour.

I used `IHttpClientFactory` to get `HttpClient` instances because directly instantiating HttpClient objects can lead to socket exhaustion issues due to improper disposal. IHttpClientFactory manages the lifetime of HttpClient handlers to prevent this problem.

I used Regular Expression to fetch matched URLs from the HTML content.
Regular expression is swift and easy to handle for significant text content.

I used Global Error Handling in Asp.net core to avoid putting a try-catch block in many places.

![image](https://github.com/user-attachments/assets/174d3cea-b45c-4068-9ee7-bd0ae5368ef5)

**Direction of expansion**

Currently, I do not yet need a DB server to store any data with these basic features, but in the future, this project has to use the DB server for some features such as:

+Trends/history of where Sympli ranked on a daily/weekly basis.

+Trends/history from other countries.

+Trends/history from other languages.

**Questions?**

If you have any questions, please let me know at my email: trieutulong89kg@gmail.com
