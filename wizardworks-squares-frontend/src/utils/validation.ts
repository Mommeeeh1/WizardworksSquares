import type { Square } from '../types/square';

/**
 * Validation constants for the application.
 * Centralized for easy maintenance and consistency.
 */
export const VALIDATION_CONFIG = {
  /** Maximum number of squares allowed */
  MAX_SQUARES: 1000,
  
  /** Request timeout in milliseconds */
  REQUEST_TIMEOUT_MS: 10000,
  
  /** Maximum reasonable grid size (for row/column validation) */
  MAX_GRID_SIZE: 1000,
  
  /** Hex color pattern */
  HEX_COLOR_PATTERN: /^#[0-9A-Fa-f]{6}$/,
} as const;

/**
 * Custom error types for better error handling.
 */
export class ValidationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ValidationError';
  }
}

/**
 * Validates that response data is a valid Square object.
 * Checks both types and value ranges.
 */
export function isValidSquare(obj: unknown): obj is Square {
  if (!obj || typeof obj !== 'object') {
    console.warn('Validation failed: not an object', obj);
    return false;
  }
  
  const square = obj as Record<string, unknown>;
  
  // Type checks
  if (typeof square.id !== 'string' || square.id.length === 0) {
    console.warn('Validation failed: invalid id', square.id);
    return false;
  }
  
  if (typeof square.row !== 'number' || square.row < 0 || square.row >= VALIDATION_CONFIG.MAX_GRID_SIZE) {
    console.warn('Validation failed: invalid row', square.row);
    return false;
  }
  
  if (typeof square.column !== 'number' || square.column < 0 || square.column >= VALIDATION_CONFIG.MAX_GRID_SIZE) {
    console.warn('Validation failed: invalid column', square.column);
    return false;
  }
  
  if (typeof square.color !== 'string' || !VALIDATION_CONFIG.HEX_COLOR_PATTERN.test(square.color)) {
    console.warn('Validation failed: invalid color', square.color);
    return false;
  }
  
  if (typeof square.createdAt !== 'string' || isNaN(Date.parse(square.createdAt))) {
    console.warn('Validation failed: invalid createdAt', square.createdAt);
    return false;
  }
  
  return true;
}

/**
 * Validates API URL format.
 */
export function isValidApiUrl(url: string): boolean {
  if (!url) return false;
  
  try {
    const parsed = new URL(url);
    return parsed.protocol === 'http:' || parsed.protocol === 'https:';
  } catch {
    return false;
  }
}


