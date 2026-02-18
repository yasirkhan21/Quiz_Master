import { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import type { AppDispatch, RootState } from '../store/store';
import {
  submitAnswer,
  nextQuestion,
  completeQuiz,
} from '../feature/quiz/quizSlice';
import Button from '../components/common/Button';
import { CheckCircle, XCircle, Clock, Trophy } from 'lucide-react';

const QuizPage = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const {
    currentQuiz,
    currentQuestionIndex,
    answerResult,
    isSubmitting,
    quizCompleted,
  } = useSelector((state: RootState) => state.quiz);

  const [selectedAnswer, setSelectedAnswer] = useState<string | null>(null);
  const [timeSpent, setTimeSpent] = useState(0);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const question = currentQuiz?.questions[currentQuestionIndex];
  const totalQuestions = currentQuiz?.questions.length ?? 0;
  const isLastQuestion = currentQuestionIndex === totalQuestions - 1;
  const progress = ((currentQuestionIndex + 1) / totalQuestions) * 100;

  // Redirect if no quiz
  useEffect(() => {
    if (!currentQuiz) navigate('/quiz/setup');
  }, [currentQuiz, navigate]);

  // Redirect when completed
  useEffect(() => {
    if (quizCompleted) navigate('/quiz/results');
  }, [quizCompleted, navigate]);

  // Timer
  useEffect(() => {
    setTimeSpent(0);
    setSelectedAnswer(null);
    timerRef.current = setInterval(() => {
      setTimeSpent((prev) => prev + 1);
    }, 1000);
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [currentQuestionIndex]);

  const handleAnswer = async (answer: string) => {
    if (selectedAnswer || !question || !currentQuiz) return;

    if (timerRef.current) clearInterval(timerRef.current);
    setSelectedAnswer(answer);

    await dispatch(
      submitAnswer({
        quizId: currentQuiz.quizId,
        questionId: question.questionId,
        userAnswer: answer,
        timeSpent,
      })
    );
  };

  const handleNext = async () => {
    if (isLastQuestion) {
      await dispatch(completeQuiz(currentQuiz!.quizId));
    } else {
      dispatch(nextQuestion());
    }
  };

  const getOptionStyle = (option: string) => {
    if (!selectedAnswer) {
      return 'border-white/10 hover:border-violet-500 hover:bg-violet-500/10 text-white cursor-pointer';
    }
    if (option === answerResult?.correctAnswer) {
      return 'border-green-500 bg-green-500/20 text-green-400';
    }
    if (option === selectedAnswer && !answerResult?.isCorrect) {
      return 'border-red-500 bg-red-500/20 text-red-400';
    }
    return 'border-white/10 text-gray-500';
  };

  if (!question) return null;

  return (
    <div className="min-h-screen flex flex-col">
      {/* Header */}
      <div className="border-b border-white/10 bg-gray-950/80 backdrop-blur-md">
        <div className="max-w-3xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-3">
            <span className="text-sm text-gray-400">
              Question{' '}
              <span className="text-white font-bold">{currentQuestionIndex + 1}</span>
              {' '}of{' '}
              <span className="text-white font-bold">{totalQuestions}</span>
            </span>
            <div className="flex items-center gap-2 text-sm text-gray-400">
              <Clock className="w-4 h-4" />
              <span className="font-mono text-white">{timeSpent}s</span>
            </div>
            <span className={`text-xs font-semibold px-3 py-1 rounded-full capitalize ${
              question.difficulty === 'easy'
                ? 'bg-green-500/20 text-green-400'
                : question.difficulty === 'medium'
                ? 'bg-yellow-500/20 text-yellow-400'
                : 'bg-red-500/20 text-red-400'
            }`}>
              {question.difficulty}
            </span>
          </div>
          {/* Progress Bar */}
          <div className="h-1.5 bg-white/10 rounded-full overflow-hidden">
            <div
              className="h-full bg-gradient-to-r from-violet-500 to-cyan-500 rounded-full transition-all duration-500"
              style={{ width: `${progress}%` }}
            />
          </div>
        </div>
      </div>

      {/* Question */}
      <div className="flex-1 flex items-start justify-center p-4 pt-10">
        <div className="w-full max-w-3xl">
          <div className="glass rounded-2xl p-8 mb-6">
            <span className="text-xs font-medium text-violet-400 uppercase tracking-wider">
              {question.category}
            </span>
            <h2 className="text-xl font-semibold text-white mt-3 leading-relaxed">
              {question.questionText}
            </h2>
          </div>

          {/* Options */}
          <div className="grid gap-3 mb-6">
            {question.options.map((option, idx) => (
              <button
                key={idx}
                onClick={() => handleAnswer(option)}
                disabled={!!selectedAnswer || isSubmitting}
                className={`w-full text-left px-6 py-4 rounded-xl border-2 font-medium transition-all duration-200 flex items-center gap-3 ${getOptionStyle(option)}`}
              >
                <span className="w-7 h-7 rounded-lg bg-white/10 flex items-center justify-center text-xs font-bold shrink-0">
                  {String.fromCharCode(65 + idx)}
                </span>
                <span>{option}</span>
                {selectedAnswer && option === answerResult?.correctAnswer && (
                  <CheckCircle className="w-5 h-5 text-green-400 ml-auto shrink-0" />
                )}
                {selectedAnswer === option && !answerResult?.isCorrect && (
                  <XCircle className="w-5 h-5 text-red-400 ml-auto shrink-0" />
                )}
              </button>
            ))}
          </div>

          {/* Answer Feedback */}
          {answerResult && (
            <div className={`glass rounded-2xl p-5 mb-6 border ${
              answerResult.isCorrect ? 'border-green-500/30' : 'border-red-500/30'
            }`}>
              <div className="flex items-center gap-3">
                <span className="text-3xl">{answerResult.feedbackEmoji}</span>
                <div>
                  <p className={`font-semibold ${
                    answerResult.isCorrect ? 'text-green-400' : 'text-red-400'
                  }`}>
                    {answerResult.feedback}
                  </p>
                  {!answerResult.isCorrect && (
                    <p className="text-sm text-gray-400 mt-1">
                      Correct answer:{' '}
                      <span className="text-green-400 font-medium">
                        {answerResult.correctAnswer}
                      </span>
                    </p>
                  )}
                  {answerResult.isCorrect && (
                    <p className="text-sm text-gray-400">
                      +{answerResult.pointsEarned} points earned
                    </p>
                  )}
                </div>
              </div>
            </div>
          )}

          {/* Next Button */}
          {selectedAnswer && (
            <Button fullWidth onClick={handleNext}>
              {isLastQuestion ? (
                <>
                  <Trophy className="w-4 h-4" /> Finish Quiz
                </>
              ) : (
                'Next Question →'
              )}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
};

export default QuizPage;