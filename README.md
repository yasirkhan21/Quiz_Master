# 🧠 QuizMaster - Intelligent Quiz Application

A full-stack quiz application with adaptive recommendations, rule-based scoring, and comprehensive analytics.

## ✨ Features

### Core Features
- ✅ **User Authentication** - JWT-based secure authentication
- ✅ **Dynamic Quiz Generation** - Questions from Open Trivia DB
- ✅ **Real-time Scoring** - Rule-based scoring engine with bonuses
- ✅ **Smart Recommendations** - Content-based question recommendations
- ✅ **Performance Analytics** - Comprehensive dashboard with charts
- ✅ **Weakness Tracking** - Identifies and focuses on weak topics
- ✅ **Adaptive Feedback** - Contextual feedback with emojis

### Technical Highlights
- 🏗️ **Clean Architecture** - Separation of concerns with CQRS
- 🎯 **Repository Pattern** - Unit of Work implementation
- 🔄 **State Management** - Redux Toolkit for React
- 📊 **Recommendation Engine** - Vector similarity algorithm
- 🎮 **Scoring Engine** - Flexible rule-based system
- 🧪 **Unit Tested** - Comprehensive test coverage

## 🛠️ Tech Stack

### Backend
- **.NET 9.0** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **BCrypt** - Password hashing
- **JWT** - Authentication

### Frontend
- **React 18** with TypeScript
- **Redux Toolkit** - State management
- **React Router** - Routing
- **Tailwind CSS** - Styling
- **Vite** - Build tool
- **Recharts** - Data visualization
- **React Hook Form** - Form management
- **Zod** - Schema validation
- **Axios** - HTTP client

## 📋 Prerequisites

- .NET 9.0 SDK
- Node.js 18+
- SQL Server (LocalDB or full instance)
- Visual Studio Code or Visual Studio 2022

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/yasirkhan21/Quiz_Master.git
cd quizmaster
```

### 2. Backend Setup
```bash
# Restore NuGet packages
dotnet restore

# Update connection string in appsettings.json
# File: src/QuizMaster.API/appsettings.json

# Run migrations
dotnet ef database update --project src/QuizMaster.Infrastructure --startup-project src/QuizMaster.API

# Run the API
dotnet run --project src/QuizMaster.API
```

API will be available at: `https://localhost:5079`

### 3. Frontend Setup
```bash
cd QuizMaster.WebApp

# Install dependencies
npm install

# Start development server
npm run dev
```

Frontend will be available at: `http://localhost:3000`

## 📁 Project Structure
```
QuizMaster/
├── src/
│   ├── QuizMaster.API/              # Web API (Controllers, Middleware)
│   ├── QuizMaster.Core/             # Domain Layer (Entities, Interfaces)
│   ├── QuizMaster.Application/      # Application Layer (CQRS, DTOs, Algorithms)
│   ├── QuizMaster.Infrastructure/   # Infrastructure (EF Core, External APIs)
│   └── QuizMaster.WebApp/           # React Frontend
├── tests/
│   ├── QuizMaster.UnitTests/        # Unit tests
│   └── QuizMaster.IntegrationTests/ # Integration tests
└── docs/                            # Documentation
```

## 🧪 Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/QuizMaster.UnitTests
```

## 🗄️ Database Schema

### Core Tables
- **Users** - User accounts and authentication
- **Quizzes** - Quiz sessions and metadata
- **Questions** - Individual questions and user answers
- **UserWeaknessProfiles** - Tracks weak topics per user
- **ScoringRules** - Configurable scoring rules

## 🎯 Key Algorithms

### 1. Content-Based Recommendation Engine
- Uses cosine similarity to match questions with user weaknesses
- Applies exponential decay to older weakness data
- Ensures diversity to avoid repetition

### 2. Rule-Based Scoring Engine
- Flexible rule system with conditions and actions
- Supports speed bonuses, streak multipliers, difficulty bonuses
- Admin-configurable through JSON rules

### 3. Adaptive Feedback System
- Context-aware motivational messages
- Performance-based emoji selection
- Personalized tips for weak areas

## 🌐 API Endpoints

See [API Documentation](docs/API.md) for complete endpoint reference.

**Quick Reference:**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login
- `POST /api/quiz/create` - Create quiz
- `POST /api/quiz/submit-answer` - Submit answer
- `GET /api/stats/dashboard` - Get statistics

## 🎨 UI Features

- 🌙 Dark mode optimized
- 📱 Fully responsive design
- ⚡ Real-time quiz timer
- 🎯 Instant answer feedback
- 📊 Interactive charts
- 🔔 Toast notifications

## 🔐 Security Features

- JWT token-based authentication
- Password hashing with BCrypt
- Protected routes
- CORS configuration
- Input validation on all endpoints

## 📈 Performance Optimizations

- Indexed database queries
- Connection pooling
- Lazy loading navigation properties
- Frontend code splitting
- Optimized re-renders with Redux

## 🐛 Known Issues & Limitations

- Open Trivia DB rate limiting (5 requests/second)
- Limited to pre-defined categories from API
- ML prediction feature not yet implemented
- No real-time multiplayer yet

## 🚀 Future Enhancements

- [ ] Real-time multiplayer quizzes
- [ ] AI-powered difficulty prediction
- [ ] Custom quiz creation
- [ ] Leaderboards
- [ ] Achievement system
- [ ] Export quiz results to PDF
- [ ] Mobile app (React Native)

## 📝 License

This project is licensed under the MIT License.

## 👥 Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## 📧 Contact

For questions or support, please contact: support@quizmaster.com

---

**Made with ❤️ using .NET and React**
