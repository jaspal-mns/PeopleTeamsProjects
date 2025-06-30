# People Teams Projects

This repository contains multiple .NET applications for AI-powered chatbots and MCP (Model Context Protocol) integration.

## Projects

### 1. JaspalChatBot
ASP.NET Core Blazor Server application with RAG (Retrieval-Augmented Generation) capabilities.

**Features:**
- Azure OpenAI integration (GPT-4o-mini)
- PDF document ingestion and semantic search
- SQLite vector database
- Interactive web interface

**Setup:**
```bash
cd src/JaspalChatBot
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-endpoint"
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "your-api-key"
dotnet run
```

### 2. PeopleTeamsMCPClient
Console application that connects to MCP servers and provides AI chat functionality.

**Features:**
- MCP server integration
- Azure OpenAI chat client
- Tool invocation support
- Interactive console interface

**Setup:**
```bash
cd src/PeopleTeamsMCPClient
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-endpoint"
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "your-api-key"
dotnet run
```

## Prerequisites

- .NET 9.0 SDK
- Azure OpenAI account with API key
- Visual Studio or VS Code (optional)

## Configuration

Both applications require Azure OpenAI credentials:

1. **User Secrets (Recommended):**
   ```bash
   dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://your-resource.openai.azure.com"
   dotnet user-secrets set "AZURE_OPENAI_API_KEY" "your-api-key"
   ```

2. **Environment Variables:**
   ```bash
   export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com"
   export AZURE_OPENAI_API_KEY="your-api-key"
   ```

## Testing

Run tests for PeopleTeamsMCPClient:
```bash
cd tests/PeopleTeamsMCPClient.Tests
dotnet test
```

## Architecture

- **JaspalChatBot**: Web-based RAG system with document ingestion
- **PeopleTeamsMCPClient**: Console MCP client with AI integration
- **Shared Dependencies**: Azure OpenAI, Microsoft.Extensions.AI

## Quick Start

1. Clone repository
2. Install .NET 9.0 SDK
3. Configure Azure OpenAI credentials
4. Run desired application with `dotnet run`