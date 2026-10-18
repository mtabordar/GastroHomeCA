/* eslint-disable */
/* tslint:disable */
/**
 * HTTP Client Singleton for API calls
 * Matches the interface expected by NSwag-generated clients
 */

const baseUrl = import.meta.env.VITE_API_BASE_URL || import.meta.env.VITE_APP_API_URL || 'http://localhost:5000';

let httpClient = null;

export function createSingletonHttpClient() {
  if (!httpClient) {
    httpClient = new HttpClient(baseUrl);
  }
  return httpClient;
}

export default httpClient;

class HttpClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  // This method matches the interface expected by NSwag-generated clients
  // Returns a Promise<Response> - NSwag will handle JSON parsing internally
  fetch(url, options = {}) {
    return window.fetch(`${this.baseUrl}${url}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        ...options.headers
      }
      });
  }

  // Legacy methods for backward compatibility (if needed)
  get(endpoint) {
    return this.fetch(endpoint, { method: 'GET' });
  }

  post(endpoint, data) {
    return this.fetch(endpoint, {
      method: 'POST',
      body: JSON.stringify(data)
      });
  }

  put(endpoint, data) {
    return this.fetch(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data)
      });
  }

  delete(endpoint) {
    return this.fetch(endpoint, { method: 'DELETE' });
  }
}
