# ✅ AI-Powered Event Feedback App | .NET 8 Razor Pages + Azure App Service + Cosmos DB + Azure AI + Bicep

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

### High-Level Flow
Users get authenticate via **Azure AD** and submit feedback through the **.NET 8 Razor Pages app hosted on Azure App Service**.
Participants can view the **real-time average rating** of current events.

All feedbacks are securely stored in *Azure Cosmos DB (NoSQL)*, while *Azure AI Text Analytics* performs sentiment analysis on the comments.

Users assigned the Admin app role can access detailed insights for their specific events, including:
- Feedback submitter details
- User comments
- Sentiment analysis results
- Graphical representations of sentiment trends

Application secrets are securely managed with Azure Key Vault, and inter-service communication is handled via Managed Identity.

#### User feedback flow 
https://github.com/username/repo-name/blob/main/docs/demo.mp4

#### Admin feedback flow 

### 🔐 Authentication & Authorization

The web app relies on **OpenID Connect (OIDC)** for Authentication using **Azure AD** as the identity provider.

### 🧭 Azure Setup Instructions

#### 1️⃣ Create a Resource Group
Create a **Resource Group** in Azure - this acts as a container for all the other Azure services required by the app.

#### 2️⃣ Provision Azure Resources
Run the Bicep file from the terminal (ensure you are in the directory where the Bicep file exists):

```bash
# Login to Azure
az login

# Build the Bicep template
az bicep build --file feedback-main.bicep

# Deploy resources to your resource group
az deployment group create \
    --resource-group <your-resource-group-name> \
    --parameters feedback-main.bicepparam \
    --template-file feedback-main.bicep

# Grant Key Vault access for local development
az keyvault set-policy \
    --name <your-vault-name> \
    --object-id <your-user-object-id> \
    --secret-permissions get list
```
💡 Note: The Key Vault policy step is required if running locally so that your VS Code terminal can access Azure Text Analytics key values.

#### 3️⃣ Register web application 
1. Go to **Microsoft Entra ID → App Registrations**
2. Create an app registration with name : `feedbackapp` or anything else of your choice.
3. Under **Manage**:
   - Click **Authentication**
   - Go to **Web Redirect Uri** and add below urls :
     ```bash
     https://feedbackapp-jsdaxzya42jta.azurewebsites.net/signin-oidc
     http://localhost:7081/signin-oidc
     https://localhost:7081/signin-oidc
     ```
4. Under **App Roles**:
   - Create role : `Admin`
   - Enable it.
     
#### 4️⃣ Clone the repository & Prepare your local environment(local system)
1. Clone the repository
2. Restore nuget package `dotnet restore`
3. Build solution `dotnet build`
4. Add a file 'appsettings.Development.json' from path - and replace placeholder values with yours Azure AD & Cosmos DB connection values.
5. Now run `dotnet run`
     
 
  > [!IMPORTANT]
  > The port (`7281`) may vary depending on your local setup. If you plan to change the port number, ensure updating below code lines :
  ```bash
  // Comment out when testing locally. Uncomment when following Step 5️⃣
  app.Urls.Add($"http://+:{port}");

  // Uncomment when testing locally
  app.Urls.Add($"http://localhost:{port}");
  ```

#### 5️⃣ Publish to Azure App Service
  1) `dotnet clean`
  2) `dotnet publish -c Release --framework net8.0`
  3) Download **Azure App Service extension**, if not installed in VS Code
  4) Go to **Command Palette** -> Click **Azure App Service : Deploy to web App**
  5) Configure deployment for below settings :
     - Subscription & Azure App Service path
     - Deploy Sub path : Specify publish folder path which will be used for deployment in Azure App.
 
✅ After deployment, your application will be available on Azure App Service with OIDC-based authentication, Azure Cosmos DB storage, and integration with Azure Text Analytics.
   
## 🤝 Contributing
This is a personal project but any suggestions or recommendations are welcome.

## 📄 License
This project is licensed under the [MIT License](./LICENSE).

