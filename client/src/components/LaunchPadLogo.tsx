export function LaunchPadLogo({ size = 32 }: { size?: number }) {
  return (
    <svg 
      width={size} 
      height={size} 
      viewBox="0 0 48 48" 
      fill="none" 
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* Rocket/Launch shape */}
      <path
        d="M24 4L28 12L36 16L28 20L24 28L20 20L12 16L20 12L24 4Z"
        fill="#1a73e8"
      />
      <path
        d="M24 28L26 34L30 38L26 40L24 44L22 40L18 38L22 34L24 28Z"
        fill="#4285f4"
      />
      {/* Book pages effect */}
      <path
        d="M14 18L18 22L14 26L10 22L14 18Z"
        fill="#34a853"
        opacity="0.8"
      />
      <path
        d="M34 18L38 22L34 26L30 22L34 18Z"
        fill="#fbbc04"
        opacity="0.8"
      />
      {/* Center circle */}
      <circle cx="24" cy="24" r="3" fill="#ffffff" />
    </svg>
  );
}
