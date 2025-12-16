import React from 'react';

interface ButtonProps {
  /** Callback function triggered when button is clicked. */
  onClick: () => void;
  /** Optional flag to disable the button. */
  disabled?: boolean;
  /** Background color for the button. */
  backgroundColor?: string;
  /** Button text content. */
  children: React.ReactNode;
}

/**
 * Reusable button component with consistent styling.
 * Supports disabled state and customizable background color.
 */
export const Button: React.FC<ButtonProps> = ({
  onClick,
  disabled = false,
  backgroundColor,
  children,
}) => {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      style={{
        padding: '0.75rem 1.5rem',
        backgroundColor: backgroundColor || '#6b7280',
        color: 'white',
        fontWeight: '700',
        borderRadius: '0.375rem',
        border: 'none',
        cursor: disabled ? 'not-allowed' : 'pointer',
        opacity: disabled ? 0.5 : 1,
        margin: '0.5rem',
      }}
    >
      {children}
    </button>
  );
};
