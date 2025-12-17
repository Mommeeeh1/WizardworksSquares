import { useState, useEffect, useCallback, useRef } from 'react';
import type { Square } from '../types/square';
import { apiClient } from '../services/api';
import { VALIDATION_CONFIG } from '../utils/validation';

const LOADING_DELAY_MS = 300; // Only show loading overlay if operation takes longer than this

interface UseSquaresReturn {
  /** Array of squares. */
  squares: Square[];
  /** Loading state. */
  isLoading: boolean;
  /** Error message, if any. */
  error: string | null;
  /** Function to add a new square. */
  addSquare: () => Promise<void>;
  /** Function to clear all squares. */
  clearAll: () => Promise<void>;
  /** Function to reload squares from the API. */
  reload: () => Promise<void>;
  /** Function to clear error message. */
  clearError: () => void;
}

/**
 * Custom hook for managing squares state and API operations.
 * Handles fetching, creating, and clearing squares.
 */
export const useSquares = (): UseSquaresReturn => {
  const [squares, setSquares] = useState<Square[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const loadingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const errorTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Helper to start loading with delay
  const startLoading = useCallback(() => {
    if (loadingTimeoutRef.current) {
      clearTimeout(loadingTimeoutRef.current);
    }
    loadingTimeoutRef.current = setTimeout(() => {
      setIsLoading(true);
    }, LOADING_DELAY_MS);
  }, []);

  // Helper to stop loading and clear timeout
  const stopLoading = useCallback(() => {
    if (loadingTimeoutRef.current) {
      clearTimeout(loadingTimeoutRef.current);
      loadingTimeoutRef.current = null;
    }
    setIsLoading(false);
  }, []);

  // Clear error message
  const clearError = useCallback(() => {
    setError(null);
    if (errorTimeoutRef.current) {
      clearTimeout(errorTimeoutRef.current);
      errorTimeoutRef.current = null;
    }
  }, []);

  // Show error with auto-dismiss
  const showError = useCallback((message: string) => {
    setError(message);
    
    // Auto-dismiss after 5 seconds
    if (errorTimeoutRef.current) {
      clearTimeout(errorTimeoutRef.current);
    }
    errorTimeoutRef.current = setTimeout(() => {
      setError(null);
    }, 5000);
  }, []);

  const loadSquares = useCallback(async () => {
    try {
      startLoading();
      clearError();
      const allSquares = await apiClient.getAllSquares();
      setSquares(allSquares);
    } catch (err) {
      const errorMessage = err instanceof Error 
        ? err.message 
        : 'Unable to load squares. Please try again later.';
      showError(errorMessage);
      console.error('Error loading squares:', err);
      
      // Log unexpected error types for debugging
      if (!(err instanceof Error)) {
        console.warn('Unexpected error type:', typeof err, err);
      }
      setSquares([]);
    } finally {
      stopLoading();
    }
  }, [startLoading, stopLoading, clearError, showError]);

  useEffect(() => {
    loadSquares();
    
    // Cleanup: clear timeouts on unmount
    return () => {
      if (loadingTimeoutRef.current) {
        clearTimeout(loadingTimeoutRef.current);
      }
      if (errorTimeoutRef.current) {
        clearTimeout(errorTimeoutRef.current);
      }
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Run only on mount

  const addSquare = useCallback(async () => {
    try {
      startLoading();
      clearError();
      
      // Check max squares limit
      if (squares.length >= VALIDATION_CONFIG.MAX_SQUARES) {
        throw new Error(`Maximum of ${VALIDATION_CONFIG.MAX_SQUARES} squares reached. Please click Clear first.`);
      }
      
      const newSquare = await apiClient.createSquare();
      setSquares((prev) => [...prev, newSquare]);
    } catch (err) {
      const errorMessage = err instanceof Error 
        ? err.message 
        : 'Unable to create square. Please try again later.';
      showError(errorMessage);
      console.error('Error creating square:', err);
      
      // Log unexpected error types for debugging
      if (!(err instanceof Error)) {
        console.warn('Unexpected error type:', typeof err, err);
      }
    } finally {
      stopLoading();
    }
  }, [squares.length, startLoading, stopLoading, clearError, showError]);

  const clearAll = useCallback(async () => {
    try {
      startLoading();
      clearError();
      await apiClient.clearAllSquares();
      setSquares([]);
    } catch (err) {
      const errorMessage = err instanceof Error 
        ? err.message 
        : 'Unable to clear squares. Please try again later.';
      showError(errorMessage);
      console.error('Error clearing squares:', err);
      
      // Log unexpected error types for debugging
      if (!(err instanceof Error)) {
        console.warn('Unexpected error type:', typeof err, err);
      }
    } finally {
      stopLoading();
    }
  }, [startLoading, stopLoading, clearError, showError]);

  return {
    squares,
    isLoading,
    error,
    addSquare,
    clearAll,
    reload: loadSquares,
    clearError,
  };
};
