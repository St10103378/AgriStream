# AgriStream IoT – Real-Time Processing System
**Practicum: Building a Real-Time Processing System**

This repository contains a complete starter solution for the AgriStream IoT assignment.

## Solution Structure

```
AgriStream/
├── AgriStream.Web/                 # ASP.NET Core 8 MVC Web Application
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   └── Program.cs
├── AgriStream.Functions/           # Azure Functions (.NET 8 Isolated Worker)
│   ├── Functions/
│   │   ├── BlobTriggerAudit.cs
│   │   ├── HttpGetTelemetry.cs
│   │   ├── HttpPostIrrigation.cs
│   │   └── QueueTriggerIrrigation.cs
│   ├── Models/
│   └── Program.cs
└── docs/                           # Screenshots & notes go here
```

## Required Azure Resources

1. **Resource Group** – e.g. `rg-agristream-studentnumber`
2. **App Service Plan** (B1 or higher recommended for auto-scale demo) + **Web App**
3. **Azure SQL Database** (Basic tier is fine) – database name `AgriStreamDb`
4. **Storage Account** (Standard LRS or GRS)
   - Blob container: `drone-images`
   - Table: `Telemetry`, `AuditLogs`
   - Queue: `irrigation-commands`
5. **Function App** (Consumption or Premium) – linked to the same Storage Account
6. **Application Insights** (recommended)

## Quick Start

### 1. Database
Run the SQL scripts in `docs/sql-setup.sql` against your Azure SQL Database.

### 2. Storage
Create the following in the Storage Account:
- Container: `drone-images` (Blob)
- Table: `Telemetry`
- Table: `AuditLogs`
- Queue: `irrigation-commands`

### 3. Configuration
Update connection strings in:
- `AgriStream.Web/appsettings.json`
- `AgriStream.Functions/local.settings.json` (and later Application Settings in Azure)

### 4. Deploy
- Publish the Web project to the App Service
- Publish the Functions project to the Function App
- Enable Managed Identity or use connection strings securely

### 5. Load Test & Auto-Scale
- Configure Auto-scale rules on the App Service Plan (CPU > 70% → scale out)
- Use Azure Load Testing, JMeter, or the provided PowerShell script
- Capture metrics in Application Insights / Azure Monitor

## Four Azure Functions (as required)

| Function | Trigger | Purpose |
|----------|---------|---------|
| BlobTriggerAudit | Blob (`drone-images`) | Log upload + write audit entry to Table Storage |
| HttpGetTelemetry | HTTP GET | Return latest telemetry from Table Storage |
| HttpPostIrrigation | HTTP POST | Place Irrigation Command on Storage Queue |
| QueueTriggerIrrigation | Queue (`irrigation-commands`) | Process command + update device status in SQL |

## Theory Answers
See the separate Word document or the answers provided in the conversation.

## Submission Checklist
- [ ] Screenshots of all Azure resources
- [ ] Screenshots of Functions running
- [ ] Load test results
- [ ] Auto-scale activity / metrics
- [ ] Application Insights / Monitor graphs
- [ ] Live URL: `https://<your-app>.azurewebsites.net`
- [ ] Source code repository link
- [ ] Theory answers
