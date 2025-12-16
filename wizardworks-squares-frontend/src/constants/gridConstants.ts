/**
 * Constants for grid layout and styling.
 */
export const GRID_CONSTANTS = {
  /** Size of each square in pixels. */
  SQUARE_SIZE: 60,
  /** Gap between squares in pixels. */
  GAP: 4,
  /** Padding around the grid container in pixels. */
  CONTAINER_PADDING: 24,
  /** Border radius for individual squares in pixels. */
  SQUARE_BORDER_RADIUS: 7,
  /** Border color for squares. */
  SQUARE_BORDER_COLOR: '#1a1a1a',
  /** Border width for squares. */
  SQUARE_BORDER_WIDTH: '2px',
} as const;

/**
 * Color constants for the application.
 */
export const COLORS = {
  /** Background color for the main container. */
  BACKGROUND: '#6b21a8',
  /** Color for the add button. */
  BUTTON_ADD: '#eab308',
  /** Color for the clear button. */
  BUTTON_CLEAR: '#f87171',
  /** Color for the loading spinner. */
  LOADING_SPINNER: '#6b21a8',
} as const;

/**
 * Typography constants.
 */
export const TYPOGRAPHY = {
  /** Font size for the main title. */
  TITLE_FONT_SIZE: '2.5rem',
  /** Font weight for the main title. */
  TITLE_FONT_WEIGHT: '700',
} as const;

/**
 * Layout constants.
 */
export const LAYOUT = {
  /** Padding for the main container. */
  CONTAINER_PADDING: '2rem',
  /** Top padding for the main container. */
  CONTAINER_PADDING_TOP: '3rem',
  /** Bottom margin for the title. */
  TITLE_MARGIN_BOTTOM: '3rem',
} as const;
