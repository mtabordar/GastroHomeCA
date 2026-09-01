/* eslint-disable */
/* tslint:disable */
/**
 * HTTP Client Singleton for API calls
 */

const baseUrl = import.meta.env.VITE_API_BASE_URL || import.meta.env.VITE_APP_API_URL || 'http://localhost:5000';

export function createSingletonHttpClient() {
  let httpClient = null;

  return function getHttpClient() {
    if (!httpClient) {
      httpClient = new HttpClient(baseUrl);
    }
    return httpClient;
  };
}
