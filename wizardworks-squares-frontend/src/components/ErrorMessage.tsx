import React from 'react';


interface ErrorMessageProps {
  /** Error message to display. */
  message: string;
  /** Callback function to dismiss the error. */
  onDismiss: () => void;
}

/**
 * Component for displaying error messages to the user.
 * Provides a dismissible error notification.
 */
export const ErrorMessage: React.FC<ErrorMessageProps> = ({ message, onDismiss }) => {
  return (
    <div
      style={{
        position: 'fixed',
        top: '20px',
        right: '20px',
        backgroundColor: '#ef4444',
        color: 'white',
        padding: '1rem 1.5rem',
        borderRadius: '0.5rem',
        boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
        zIndex: 1000,
        maxWidth: '400px',
        display: 'flex',
        alignItems: 'center',
        gap: '1rem',
      }}
    >
      <div style={{ flex: 1 }}>
        <strong style={{ display: 'block', marginBottom: '0.25rem' }}>Error</strong>
        <span style={{ fontSize: '0.875rem' }}>{message}</span>
      </div>
      <button
        onClick={onDismiss}
        style={{
          backgroundColor: 'transparent',
          border: 'none',
          color: 'white',
          cursor: 'pointer',
          fontSize: '1.25rem',
          fontWeight: 'bold',
          padding: '0',
          width: '24px',
          height: '24px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          borderRadius: '0.25rem',
        }}
        aria-label="Dismiss error"
      >
        ×
      </button>
    </div>
  );
};
