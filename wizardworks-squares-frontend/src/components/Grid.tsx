import React from 'react';
import type { Square } from '../types/square';
import { GRID_CONSTANTS } from '../constants/gridConstants';

interface GridProps {
  squares: Square[];
}

/**
 * Pure presentation component that renders a grid of squares.
 * Calculates grid size and positions squares dynamically.
 */
export const Grid: React.FC<GridProps> = ({ squares }) => {
  // Calculate grid size: ceil(sqrt(totalSquares))
  // This matches the backend's expanding grid logic
  const size = squares.length === 0 ? 1 : Math.ceil(Math.sqrt(squares.length));

  // Create a 2D array to represent the grid
  const gridArray = Array(size).fill(null).map(() => Array(size).fill(null));
  squares.forEach((square) => {
    gridArray[square.row][square.column] = square;
  });

  // Calculate container dimensions
  const containerWidth = size * GRID_CONSTANTS.SQUARE_SIZE + (size - 1) * GRID_CONSTANTS.GAP + GRID_CONSTANTS.CONTAINER_PADDING * 2;
  const containerHeight = containerWidth;
  const minContainerSize = GRID_CONSTANTS.SQUARE_SIZE + GRID_CONSTANTS.CONTAINER_PADDING * 2;

  return (
    <div
      className="bg-blue-100 border-4 border-blue-300 rounded-lg"
      style={{
        width: `${containerWidth}px`,
        height: `${containerHeight}px`,
        minWidth: `${minContainerSize}px`,
        minHeight: `${minContainerSize}px`,
        margin: '0 auto',
        padding: `${GRID_CONSTANTS.CONTAINER_PADDING}px`,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0,
      }}
    >
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: `repeat(${size}, ${GRID_CONSTANTS.SQUARE_SIZE}px)`,
          gridTemplateRows: `repeat(${size}, ${GRID_CONSTANTS.SQUARE_SIZE}px)`,
          gap: `${GRID_CONSTANTS.GAP}px`,
        }}
      >
        {Array.from({ length: size * size }, (_, index) => {
          const rowIndex = Math.floor(index / size);
          const colIndex = index % size;
          const square = gridArray[rowIndex][colIndex];

          return (
            <div
              key={`${rowIndex}-${colIndex}`}
              style={{
                backgroundColor: square?.color || 'transparent',
                width: `${GRID_CONSTANTS.SQUARE_SIZE}px`,
                height: `${GRID_CONSTANTS.SQUARE_SIZE}px`,
                borderRadius: `${GRID_CONSTANTS.SQUARE_BORDER_RADIUS}px`,
                border: square ? `${GRID_CONSTANTS.SQUARE_BORDER_WIDTH} solid ${GRID_CONSTANTS.SQUARE_BORDER_COLOR}` : 'none',
              }}
            />
          );
        })}
      </div>
    </div>
  );
};
