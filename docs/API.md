# QuizMaster API Documentation

## Base URL
```
https://localhost:5079/api
```

## Authentication

All authenticated endpoints require a JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## Endpoints

### Authentication

#### Register User
```http
POST /auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "Password123",
  "confirmPassword": "Password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGc...",
    "user": {
      "userId": "guid",
      "username": "johndoe",
      "email": "john@example.com"
    },
    "expiresAt": "2024-01-01T12:00:00Z"
  }
}
```

#### Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123"
}
```

### Quiz Management

#### Create Quiz
```http
POST /quiz/create
Authorization: Bearer {token}
Content-Type: application/json

{
  "category": "Science & Nature",
  "difficulty": "medium",
  "numberOfQuestions": 10
}
```

#### Submit Answer
```http
POST /quiz/submit-answer
Authorization: Bearer {token}
Content-Type: application/json

{
  "quizId": "guid",
  "questionId": "guid",
  "userAnswer": "Answer text",
  "timeSpent": 15.5
}
```

#### Complete Quiz
```http
POST /quiz/{quizId}/complete
Authorization: Bearer {token}
```

#### Get My Quizzes
```http
GET /quiz/my-quizzes?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### Get Categories
```http
GET /quiz/categories
```

### Statistics

#### Get Dashboard Stats
```http
GET /stats/dashboard
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalQuizzesTaken": 25,
    "averageAccuracy": 78.5,
    "totalScore": 1250,
    "bestCategory": "Science & Nature",
    "weakestCategory": "History",
    "recentQuizzes": [...]
  }
}
```

## Error Responses

All error responses follow this format:
```json
{
  "success": false,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

### Common Status Codes
- `200 OK` - Request succeeded
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error