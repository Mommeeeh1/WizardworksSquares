import React from 'react';
import { COLORS } from '../../constants/gridConstants';

interface LoadingOverlayProps {
  /** Whether the overlay should be visible. */
  isLoading: boolean;
}

/**
 * Component for displaying a loading overlay with spinner.
 * Shows a semi-transparent overlay with a centered spinner when loading.
 */
export const LoadingOverlay: React.FC<LoadingOverlayProps> = ({ isLoading }) => {
  if (!isLoading) {
    return null;
  }

  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 9999,
      }}
    >
      <div
        style={{
          backgroundColor: 'white',
          padding: '1.5rem',
          borderRadius: '0.5rem',
          boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
        }}
      >
        <div
          style={{
            width: '48px',
            height: '48px',
            border: `3px solid ${COLORS.LOADING_SPINNER}`,
            borderTopColor: 'transparent',
            borderRadius: '50%',
            animation: 'spin 1s linear infinite',
          }}
        />
      </div>
    </div>
  );
};
