/**
 * Represents a square in the grid system.
 */
export interface Square {
  /** Unique identifier for the square. */
  id: string;
  /** Zero-based row position in the grid. */
  row: number;
  /** Zero-based column position in the grid. */
  column: number;
  /** Hexadecimal color code for the square (e.g., "#FF5733"). */
  color: string;
  /** ISO timestamp indicating when the square was created. */
  createdAt: string;
}