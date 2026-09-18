{
  "testEnvironment": "node",
  "testMatch": [
    "**/tests/**/*.test.ts",
    "**/tests/**/*.spec.ts"
  ],
  "testPathIgnorePatterns": [
    "/node_modules/"
  ],
  "moduleFileExtensions": [
    "ts",
    "tsx",
    "js",
    "jsx",
    "json",
    "node"
  ],
  "transform": {
    "^.+\\.tsx?$": [
      "@swc/jest",
      {
        "jsc": {
          "parser": {
            "syntax": "typescript",
            "tsx": true
          },
          "transform": "typescript"
        }
      }
    ]
  },
  "coverageReporters": [
    "text",
    "lcov"
  ],
  "coverageDirectory": "coverage",
  "collectCoverageFrom": [
    "src/**/*.{ts,tsx}",
    "!src/**/*.d.ts"
  ]
}