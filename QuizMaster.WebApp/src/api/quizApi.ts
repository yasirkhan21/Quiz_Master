import axiosInstance from './axiosInstance';
import type {
    ApiResponse,
    Quiz,
    QuizSummary,
    AnswerResult,
    CreateQuizPayload,
    SubmitAnswerPayload,
} from '../types';

export const quizApi = {
  getCategories: async () => {
    const response = await axiosInstance.get<ApiResponse<string[]>>(
      '/quiz/categories'
    );
    return response.data;
  },

  createQuiz: async (payload: CreateQuizPayload) => {
    const response = await axiosInstance.post<ApiResponse<Quiz>>(
      '/quiz/create',
      payload
    );
    return response.data;
  },

  submitAnswer: async (payload: SubmitAnswerPayload) => {
    const response = await axiosInstance.post<ApiResponse<AnswerResult>>(
      '/quiz/submit-answer',
      payload
    );
    return response.data;
  },

  completeQuiz: async (quizId: string) => {
    const response = await axiosInstance.post<ApiResponse<QuizSummary>>(
      `/quiz/${quizId}/complete`
    );
    return response.data;
  },

  getMyQuizzes: async (pageNumber = 1, pageSize = 10) => {
    const response = await axiosInstance.get<ApiResponse<QuizSummary[]>>(
      `/quiz/my-quizzes?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
    return response.data;
  },

  getQuizById: async (quizId: string) => {
    const response = await axiosInstance.get<ApiResponse<Quiz>>(
      `/quiz/${quizId}`
    );
    return response.data;
  },

  deleteQuiz: async (quizId: string) => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(
      `/quiz/${quizId}`
    );
    return response.data;
  },
};