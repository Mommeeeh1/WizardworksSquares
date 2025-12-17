import type { Square } from '../types/square';
import { 
  isValidSquare, 
  isValidApiUrl, 
  VALIDATION_CONFIG, 
  ValidationError
} from '../utils/validation';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5268';

// Validate API URL at startup
if (!isValidApiUrl(API_BASE_URL)) {
  throw new ValidationError(`Invalid API URL configuration: ${API_BASE_URL}`);
}

/**
 * API client for communicating with the squares backend.
 * Handles HTTP requests and error handling.
 */
class ApiClient {

  /**
   * Makes an HTTP request to the API.
   * @param endpoint - The API endpoint path.
   * @param options - Optional fetch request options.
   * @returns The response data as unknown (must be validated by caller).
   * @throws Error if the request fails.
   */
  private async request(endpoint: string, options?: RequestInit): Promise<unknown> {
    const url = `${API_BASE_URL}${endpoint}`;
    
    // Timeout protection
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), VALIDATION_CONFIG.REQUEST_TIMEOUT_MS);
    
    try {
      const response = await fetch(url, {
        ...options,
        signal: controller.signal,
        headers: {
          'Content-Type': 'application/json',
          ...options?.headers,
        },
      });

      clearTimeout(timeoutId);

      if (!response.ok) {
        const error = await response.json().catch(() => ({ 
          title: 'Unknown error',
          detail: 'An unexpected error occurred'
        }));
        
        // Handle Problem Details for HTTP APIs (RFC 7807)
        // Make technical error messages more user-friendly
        let errorMessage = error.detail || error.title || error.error;
        
        // Convert technical backend messages to user-friendly ones
        if (errorMessage && (
          errorMessage.includes('null') || 
          errorMessage.includes('Id') ||
          errorMessage.includes('Guid')
        )) {
          errorMessage = 'Something went wrong. Please try again.';
        }
        
        // Fallback messages based on status code
        if (!errorMessage) {
          errorMessage = response.status === 500 
            ? 'An internal server error occurred. Please try again later.'
            : 'Something went wrong. Please try again.';
        }
        
        throw new Error(errorMessage);
      }

      // Handle No Content responses
      if (response.status === 204 || response.headers.get('content-length') === '0') {
        return null;
      }

      // Return as unknown - must be validated by caller
      return await response.json();
    } catch (err) {
      clearTimeout(timeoutId);
      
      // Handle timeout
      if (err instanceof Error && err.name === 'AbortError') {
        throw new Error('Request timeout. Please try again.');
      }
      
      // Handle network errors
      if (err instanceof TypeError && err.message.includes('fetch')) {
        throw new Error('Service temporarily unavailable. Please try again later.');
      }
      
      throw err;
    }
  }

  /**
   * Retrieves all squares from the API.
   * @returns A promise that resolves to an array of squares.
   * @throws ValidationError if the response format is invalid.
   */
  async getAllSquares(): Promise<Square[]> {
    const data = await this.request('/api/squares');
    
    // Validate response is an array
    if (!Array.isArray(data)) {
      console.error('getAllSquares: Expected array, got:', typeof data, data);
      throw new ValidationError('Something went wrong loading squares. Please try again.');
    }
    
    // Validate and filter out invalid squares
    const validSquares = data.filter((square: unknown) => {
      const isValid = isValidSquare(square);
      if (!isValid) {
        console.error('getAllSquares: Invalid square filtered out:', square);
      }
      return isValid;
    });
    
    if (validSquares.length !== data.length) {
      console.warn(`getAllSquares: Filtered out ${data.length - validSquares.length} invalid squares`);
    }
    
    return validSquares;
  }

  /**
   * Creates a new square via the API.
   * @returns A promise that resolves to the created square.
   * @throws ValidationError if the response format is invalid.
   */
  async createSquare(): Promise<Square> {
    const data = await this.request('/api/squares', {
      method: 'POST',
    });
    
    // Validate response is a valid square
    if (!isValidSquare(data)) {
      console.error('createSquare: Invalid square received:', data);
      throw new ValidationError('Something went wrong creating the square. Please try again.');
    }
    
    return data;
  }

  /**
   * Removes all squares from the API.
   * @returns A promise that resolves when the operation completes.
   */
  async clearAllSquares(): Promise<void> {
    // DELETE typically returns 204 No Content, which we handle as null
    await this.request('/api/squares', {
      method: 'DELETE',
    });
    // No validation needed for void operations
  }
}

export const apiClient = new ApiClient();