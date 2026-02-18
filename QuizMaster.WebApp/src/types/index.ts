export interface User {
  userId: string;
  username: string;
  email: string;
  createdAt: string;
  lastLoginAt?: string;
}

export interface AuthResponse {
  token: string;
  user: User;
  expiresAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}

export interface Question {
  questionId: string;
  questionText: string;
  category: string;
  difficulty: string;
  type: string;
  options: string[];
  correctAnswer: string;
  userAnswer?: string;
  isCorrect: boolean;
  timeSpent: number;
}

export interface Quiz {
  quizId: string;
  userId: string;
  category: string;
  difficulty: string;
  totalScore: number;
  accuracy: number;
  averageTimePerQuestion: number;
  timestamp: string;
  isCompleted: boolean;
  questions: Question[];
}

export interface QuizSummary {
  quizId: string;
  category: string;
  difficulty: string;
  totalScore: number;
  accuracy: number;
  totalQuestions: number;
  correctAnswers: number;
  timestamp: string;
}

export interface AnswerResult {
  isCorrect: boolean;
  correctAnswer: string;
  pointsEarned: number;
  feedback: string;
  feedbackEmoji: string;
}

export interface DashboardStats {
  totalQuizzesTaken: number;
  averageAccuracy: number;
  totalScore: number;
  totalCorrectAnswers: number;
  totalQuestionsAnswered: number;
  bestCategory: string;
  weakestCategory: string;
  averageTimePerQuestion: number;
  recentQuizzes: QuizSummary[];
}

export interface CreateQuizPayload {
  category: string;
  difficulty: string;
  numberOfQuestions: number;
}

export interface SubmitAnswerPayload {
  quizId: string;
  questionId: string;
  userAnswer: string;
  timeSpent: number;
}