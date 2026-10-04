import {
  HttpErrorResponse
} from '@angular/common/http';

import {
  ApiProblemDetails
} from './api-error';

export function getApiErrorMessage(
  error: unknown
): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'An unexpected error occurred.';
  }

  if (error.status === 0) {
    return 'Unable to connect to the API. Please check that the backend is running.';
  }

  const problem =
    error.error as ApiProblemDetails | null;

  if (problem?.detail) {
    return problem.detail;
  }

  if (problem?.title) {
    return problem.title;
  }

  switch (error.status) {
    case 400:
      return 'The submitted data is invalid. Please check the form.';

    case 404:
      return 'The requested API endpoint could not be found.';

    case 408:
      return 'The request timed out. Please try again.';

    case 429:
      return 'Too many requests. Please wait and try again.';

    case 500:
      return 'The server encountered an unexpected error.';

    default:
      return 'Unable to calculate the optimal combination.';
  }
}