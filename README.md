# AI Meeting Summariser

A full-stack production-quality SaaS application for AI-powered meeting transcription, summarization, and action item extraction.

## Features

- **User Authentication**: Secure JWT-based registration and login
- **Meeting Upload**: Upload audio/video meeting recordings
- **AI Transcription**: Automatic transcription using AI services
- **AI Summarization**: Generate meeting summaries with key discussion points
- **Action Items**: Extract and track actionable items from meetings
- **Decisions**: Track decisions made during meetings
- **AI Chat**: Ask questions about meeting transcripts
- **Dashboard**: Overview of meetings, tasks, and statistics

## Tech Stack

### Frontend
- React 19 with TypeScript
- Vite for build tooling
- Tailwind CSS for styling
- React Router for navigation
- TanStack Query for data fetching
- Zustand for state management
- Axios for HTTP requests

### Backend
- .NET 9 ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- JWT Authentication
- FluentValidation
- Serilog for logging
- MediatR for CQRS pattern

## Architecture

### Backend (Clean Architecture + Vertical Slice)

```
src/Api/
  Controllers/      # API endpoints
  Middleware/      # Custom middleware
  Application/    # Feature-based CQRS
    Features/
      Auth/        # Register, Login, GetCurrentUser
      Meetings/    # CRUD operations
      AI/          # Processing and Chat
      Dashboard/   # Statistics
      ActionItems/ # Task management
  Infrastructure/   # DB, File storage, Services

src/Domain/
  Entities/        # Domain models
  Enums/          # Domain enumerations
  Common/         # Base classes
```

### Frontend (Feature-based)

```
src/
  components/      # Reusable UI components
  features/       # Feature-specific components
  pages/          # Page components
  services/       # API client
  hooks/          # Custom hooks
  types/          # TypeScript types
  layouts/        # Layout components
```

## Prerequisites

- Node.js 18+
- .NET 9 SDK
- PostgreSQL (Neon for deployment)

## Getting Started

### Backend

```bash
cd server/src/Api
dotnet restore
dotnet build
dotnet run
```

The API will be available at `http://localhost:5000`

### Frontend

```bash
cd client
npm install
npm run dev
```

The app will be available at `http://localhost:5173`

## Environment Variables

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ai_meeting_summariser;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereThatIsAtLeast32Chars!",
    "Issuer": "AiMeetingSummariser",
    "Audience": "AiMeetingSummariser"
  }
}
```

### Frontend (.env)

```
VITE_API_BASE_URL=http://localhost:5000/api
```

## Database Setup

### Local Development

1. Install PostgreSQL locally
2. Create a database named `ai_meeting_summariser`
3. Update connection string in appsettings.json
4. Run the application - EF Core will create tables automatically

### Neon Database (Production)

1. Create a Neon project at https://neon.tech
2. Get the connection string
3. Set as environment variable `ConnectionStrings__DefaultConnection`

## Deployment

### Backend on Render

1. Create a new Web Service on Render
2. Connect your GitHub repository
3. Set environment variables:
   - `ConnectionStrings__DefaultConnection`: Neon connection string
   - `Jwt__Key`: Your secret key
   - `Jwt__Issuer`: AiMeetingSummariser
   - `Jwt__Audience`: AiMeetingSummariser
4. Build command: `dotnet build src/Api/Api.csproj`
5. Start command: `dotnet run --project src/Api/Api.csproj`

### Frontend on Render

1. Create a new Static Site on Render
2. Connect your GitHub repository
3. Set environment variable:
   - `VITE_API_BASE_URL`: Your backend URL
4. Build command: `npm run build`
5. Publish directory: `dist`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user

### Meetings
- `GET /api/meetings` - List meetings (paginated)
- `GET /api/meetings/{id}` - Get meeting details
- `POST /api/meetings` - Create/upload meeting
- `DELETE /api/meetings/{id}` - Delete meeting
- `GET /api/meetings/search` - Search meetings

### AI
- `POST /api/ai/meetings/{id}/process` - Process meeting
- `POST /api/ai/meetings/{id}/chat` - Chat with meeting
- `GET /api/ai/meetings/{id}/chat` - Get chat history

### Dashboard
- `GET /api/dashboard/overview` - Get dashboard statistics

### Action Items
- `GET /api/actionItems` - List action items
- `PUT /api/actionItems/{id}` - Update action item

## Future Improvements

- Real AI integration (OpenAI, Azure OpenAI)
- Speech-to-text services (Whisper, Azure Speech)
- File storage (S3, Azure Blob, Cloudinary)
- Email notifications
- Team collaboration
- Meeting scheduling
- Calendar integration
- Export to Notion, Slack, etc.

## License

MIT
