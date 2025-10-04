# ✅ Event Feedback App (.NET 8 + Azure App Service + Azure AI + Cosmos DB + Bicep)

This is a **.NET 8 Razor Pages** web application, deployed on **Azure App Service (Linux)** to collect real-time feedbacks from event/session attendees.

The app leverages **Azure AI Text Analytics** to perform sentiment analysis on the feedback/comments which are viewable only to events' admin.
Cosmos DB (SQL) is used to persist events comments and details.

Additionally, all the Azure resources required to make this app work in the most realistic way and close to enterprise grade were provisioned using **bicep**.

**Bonus:**  The App's UI - specifically  colors, formatting, and styling was developed with **Github Copilot**. I have used Github Copilot in **Agent, Edit, and Ask mode** 
treating it as a peer, who can be asked questions to make better design or architecture decision.


---

## Tech Stack

<p>
  <img src= "https://img.shields.io/badge/C%23-003f90?style=for-the-badge" alt="C#"/>
  <img src= "https://img.shields.io/badge/.NET 8-512bd4?style=for-the-badge" alt=".NET"/>
  <img src= "https://img.shields.io/badge/Azure%20AD-1e90ff?style=for-the-badge" alt="Azure AD"/>
  <img src= "https://img.shields.io/badge/Visual%20Studio%20Code-007ACC?style=for-the-badge" alt="Visual Studio Code"/>
  <img src= "https://img.shields.io/badge/Azure%20AI-007FFF?style=for-the-badge" alt="Azure AI"/>
  <img src= "https://img.shields.io/badge/Azure Cosmos%20DB-007FFF?style=for-the-badge" alt="Azure Cosmos DB"/>
  <img src= "https://img.shields.io/badge/Razor%20Pages-663399?style=for-the-badge" alt="Razor Pages"/>
</p>

## 🚀 Key Features

- 🧱 **.NET 8 Razor Pages**
- 🔐 **Azure AD Authentication** - Secure sign-in for users and admins
- 📘 **Azure App Service** with App Settings - Hosting .NET 8 Razor Pages application
-    **Azure Cosmos DB** – Storing feedback data at scale
-   **Azure Key Vault** – Securely managing application secrets and keys
-   **Managed Identity** – Enabling secure, passwordless communication between services
- ⚙️ **Bicep(IaC)** - Provisioning Azure resources as code 
- 🤖 **Azure AI Text Analytics**  - Sentiment analysis of events feedback
- 🧑‍⚖️ **Role-Based Access Control** - Separate user and admin role.Controlled through App roles.
  

## 🎯 Why This Project Was Built

This project was developed to gain **hands-on experience in Azure PaaS services,** with a focus on building modern, secure, cloud-native application.

> 💡 **Inspiration**: Feedbacks for events and sessions are often collected using Google Forms or manually created surveys.
> I wanted to build on that idea and create a modern, secure, cloud-native application hosted on Azure that not only collects feedback but also persists it for future reference.


## 🧱 Project Architecture

## High-Level Flow
Users authenticate via **Azure AD** and submit feedback through the **.NET 8 Razor Pages app hosted on Azure App Service**.
Participants can view the **real-time average rating** of current events.

All feedbacks are securely stored in *Azure Cosmos DB (NoSQL)*, while *Azure AI Text Analytics* performs sentiment analysis on the comments.

Users assigned the Admin app role can access detailed insights for their specific events, including:
- Feedback submitter details
- User comments
- Sentiment analysis results
- Graphical representations of sentiment trends

Application secrets are securely managed with Azure Key Vault, and inter-service communication is handled via Managed Identity.

### User feedback flow 
https://github.com/username/repo-name/blob/main/docs/demo.mp4

# Admin feedback flow 

🔐 Authentication & Authorization
The web app relies on Open ID Connect(OIDC) for Authentication using Azure AD as identity provider.

🧭 Azure Setup Instructions
1️⃣ Create a Resource Group in Azure  - This will act as a unit within which all the other azure services will be placed.
1️⃣ Provision resources required for the web application
Run the bicep file from command terminal (Ensure bicep file is ran from the path where it exists) in VS Code as below :
`az login' - sign in to your azure tenant.
az bicep build --file feedback-main.bicep                                                    
`az deployment group create --resource-group <your-resource-group-name> --parameters feedback-main.bicepparam --template-file feedback-main.bicep`
`az keyvault set-policy --name <your-vault-name> --object-id <your-user-object-id> --secret-permissions get list`   - When running on localhost this is needed as your VS Code terminal should be able to access Azure Key Vault to get Azure Text Analytics Key value.

1️⃣ Register web application 
1. Go to **Microsoft Entra ID → App Registrations**
2. Create an app registration with name : `feedbackapp`
3. Under **Manage**:
   - Click **Authentication**
   - In `Web Redirect Uri`
     -> add urls : 'https://feedbackapp-jsdaxzya42jta.azurewebsites.net/signin-oidc', 'http://localhost:7081/signin-oidc','https://localhost:7081/signin-oidc'
4. Under **App Roles**:
   - Create role : `Admin`
   - Enable it.
Clone the repository
- Prepare your local environment(local system) for running the app
    - Restore nuget package ``` bash  dotnet restore  ```
    - Build solution ``` bash  dotnet build  ```
    - Add a file 'appsettings.Development.json' from path - and replace placeholder values with yours to enable Azure AD & Cosmos DB connection.
 
> [!IMPORTANT]
> The port (`7281`) may vary depending on your local setup. If you plan to change to port number, ensure updating below code lines :
> Comment line - `app.Urls.Add($"http://+:{port}");` in Program.cs  (Enable this when running or testing on Azure app Service)
> Uncomment line : `app.Urls.Add($"http://localhost:{port}");`  (Comment this out when deploying on Azure App service)

- Now run `dotnet run`


  To publish this code on your Azure App Service, proceed with below steps :
  1) dotnet clean
  2) dotnet publish -c Release --framework net8.0
  3) update 
  
   
    










## 🤝 Contributing
This is a personal project but any suggestions or recommendations are welcome.

## 📄 License
This project is licensed under the [MIT License](./LICENSE).

