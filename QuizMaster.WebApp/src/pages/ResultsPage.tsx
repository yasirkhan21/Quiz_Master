import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import type { AppDispatch, RootState } from '../store/store';
import { resetQuiz } from '../feature/quiz/quizSlice';
import Button from '../components/common/Button';
import { Trophy, Target, Clock, RotateCcw, Home } from 'lucide-react';

const ResultsPage = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const { finalSummary } = useSelector((state: RootState) => state.quiz);

  if (!finalSummary) {
    navigate('/dashboard');
    return null;
  }

  const handlePlayAgain = () => {
    dispatch(resetQuiz());
    navigate('/quiz/setup');
  };

  const handleGoHome = () => {
    dispatch(resetQuiz());
    navigate('/dashboard');
  };

  const accuracy = Number(finalSummary.accuracy);
  const grade =
    accuracy >= 90 ? { label: 'Excellent!', color: 'text-green-400', emoji: '🏆' }
    : accuracy >= 70 ? { label: 'Great Job!', color: 'text-cyan-400', emoji: '🎯' }
    : accuracy >= 50 ? { label: 'Good Effort!', color: 'text-yellow-400', emoji: '💪' }
    : { label: 'Keep Practicing!', color: 'text-orange-400', emoji: '📚' };

  return (
    <div className="min-h-screen flex items-center justify-center p-4">
      <div className="fixed inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-violet-600/10 rounded-full blur-3xl" />
        <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-cyan-600/10 rounded-full blur-3xl" />
      </div>

      <div className="w-full max-w-lg relative">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="text-6xl mb-4">{grade.emoji}</div>
          <h1 className={`text-3xl font-bold ${grade.color}`}>{grade.label}</h1>
          <p className="text-gray-400 mt-2">Quiz Completed</p>
        </div>

        {/* Score Card */}
        <div className="glass rounded-2xl p-8 mb-6">
          {/* Main Score */}
          <div className="text-center mb-8 pb-8 border-b border-white/10">
            <p className="text-gray-400 text-sm mb-2">Total Score</p>
            <div className="text-6xl font-black gradient-text">
              {finalSummary.totalScore}
            </div>
            <p className="text-gray-500 text-sm mt-2">points</p>
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-3 gap-4">
            <div className="text-center">
              <div className="flex items-center justify-center gap-1 text-violet-400 mb-1">
                <Target className="w-4 h-4" />
              </div>
              <p className="text-2xl font-bold text-white">{accuracy.toFixed(1)}%</p>
              <p className="text-xs text-gray-500">Accuracy</p>
            </div>

            <div className="text-center">
              <div className="flex items-center justify-center gap-1 text-cyan-400 mb-1">
                <Trophy className="w-4 h-4" />
              </div>
              <p className="text-2xl font-bold text-white">
                {finalSummary.correctAnswers}/{finalSummary.totalQuestions}
              </p>
              <p className="text-xs text-gray-500">Correct</p>
            </div>

            <div className="text-center">
              <div className="flex items-center justify-center gap-1 text-green-400 mb-1">
                <Clock className="w-4 h-4" />
              </div>
              <p className="text-2xl font-bold text-white">
                {finalSummary.totalQuestions}
              </p>
              <p className="text-xs text-gray-500">Questions</p>
            </div>
          </div>

          {/* Category & Difficulty */}
          <div className="mt-6 pt-6 border-t border-white/10 flex justify-between text-sm">
            <div>
              <span className="text-gray-500">Category: </span>
              <span className="text-white font-medium">{finalSummary.category}</span>
            </div>
            <div>
              <span className="text-gray-500">Difficulty: </span>
              <span className="text-white font-medium capitalize">
                {finalSummary.difficulty}
              </span>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex gap-3">
          <Button variant="secondary" fullWidth onClick={handleGoHome}>
            <Home className="w-4 h-4" />
            Dashboard
          </Button>
          <Button fullWidth onClick={handlePlayAgain}>
            <RotateCcw className="w-4 h-4" />
            Play Again
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ResultsPage;