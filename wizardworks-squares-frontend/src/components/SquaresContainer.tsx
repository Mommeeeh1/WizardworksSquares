import { Grid } from './Grid';
import { Button } from './Button';
import { ErrorMessage } from './ErrorMessage';
import { LoadingOverlay } from './LoadingOverlay';
import { useSquares } from '../hooks/useSquares';
import { COLORS, TYPOGRAPHY, LAYOUT } from '../constants/gridConstants';
import { VALIDATION_CONFIG } from '../utils/validation';

/**
 * Container component that manages squares state and coordinates all UI.
 */
export const SquaresContainer: React.FC = () => {
  const { squares, isLoading, error, addSquare, clearAll, clearError } = useSquares();

  return (
    <div 
      className="w-full min-h-screen"
      style={{ 
        backgroundColor: COLORS.BACKGROUND, 
        minHeight: '100vh', 
        width: '100%', 
        margin: 0, 
        padding: LAYOUT.CONTAINER_PADDING,
        paddingTop: LAYOUT.CONTAINER_PADDING_TOP,
        boxSizing: 'border-box',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
    >
      {/* Title */}
      <h1 
        style={{ 
          fontSize: TYPOGRAPHY.TITLE_FONT_SIZE, 
          fontWeight: TYPOGRAPHY.TITLE_FONT_WEIGHT, 
          color: 'white', 
          marginBottom: LAYOUT.TITLE_MARGIN_BOTTOM,
          fontFamily: 'sans-serif',
          marginTop: 0,
        }}
      >
        Wizardworks
      </h1>

      {/* Action buttons */}
      <div className="mb-8 flex gap-6 items-center justify-center">
        <Button
          onClick={addSquare}
          disabled={isLoading || squares.length >= VALIDATION_CONFIG.MAX_SQUARES}
          backgroundColor={COLORS.BUTTON_ADD}
        >
          Add square
        </Button>
        <Button
          onClick={clearAll}
          disabled={isLoading}
          backgroundColor={COLORS.BUTTON_CLEAR}
        >
          Clear
        </Button>
      </div>

      {/* Grid - just renders squares */}
      <Grid squares={squares} />

      {/* Error message */}
      {error && (
        <ErrorMessage
          message={error}
          onDismiss={clearError}
        />
      )}

      {/* Loading overlay */}
      <LoadingOverlay isLoading={isLoading} />
    </div>
  );
};
